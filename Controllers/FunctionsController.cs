using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using serveur.Data;
using serveur.Models.Entities;
using serveur.Models.Dtos;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FunctionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FunctionsController> _logger;

        public FunctionsController(AppDbContext context, ILogger<FunctionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir toutes les fonctions
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Function>>> GetAll()
        {
            try
            {
                return await _context.Functions
                    .OrderBy(f => f.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des fonctions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir le nombre de fonctions
        /// </summary>
        [HttpGet("count")]
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                return await _context.Functions.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du comptage des fonctions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une fonction par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Function>> GetById(int id)
        {
            try
            {
                var function = await _context.Functions.FindAsync(id);
                if (function == null)
                {
                    return NotFound();
                }
                return function;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la fonction {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir une fonction par son code
        /// </summary>
        [HttpGet("code/{code}")]
        public async Task<ActionResult<Function>> GetByCode(string code)
        {
            try
            {
                var function = await _context.Functions
                    .FirstOrDefaultAsync(f => f.Code == code);
                if (function == null)
                {
                    return NotFound();
                }
                return function;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la fonction par code {Code}", code);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les fonctions actives
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Function>>> GetActive()
        {
            try
            {
                return await _context.Functions
                    .Where(f => f.IsActive)
                    .OrderBy(f => f.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des fonctions actives");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les enfants d'une fonction
        /// </summary>
        [HttpGet("{id}/children")]
        public async Task<ActionResult<IEnumerable<Function>>> GetChildren(int id)
        {
            try
            {
                return await _context.Functions
                    .Where(f => f.ParentId == id)
                    .OrderBy(f => f.SortOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des enfants de la fonction {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les fonctions en structure arborescente
        /// </summary>
        [HttpGet("tree")]
        public async Task<ActionResult<IEnumerable<FunctionTreeDto>>> GetTree()
        {
            try
            {
                var allFunctions = await _context.Functions
                    .OrderBy(f => f.SortOrder)
                    .ToListAsync();

                _logger.LogInformation("Total fonctions dans la BD: {Count}", allFunctions.Count);
                _logger.LogInformation("Fonctions racines (ParentId=null): {Roots}",
                    string.Join(", ", allFunctions.Where(f => f.ParentId == null).Select(f => f.Code)));

                // Chercher system_admin et afficher son parent
                var systemAdmin = allFunctions.FirstOrDefault(f => f.Code == "system_admin");
                if (systemAdmin != null)
                {
                    var parent = systemAdmin.ParentId.HasValue
                        ? allFunctions.FirstOrDefault(f => f.Id == systemAdmin.ParentId.Value)
                        : null;
                    _logger.LogInformation("system_admin trouvé - Id: {Id}, ParentId: {ParentId}, Parent: {ParentCode}",
                        systemAdmin.Id, systemAdmin.ParentId, parent?.Code ?? "null");
                }
                else
                {
                    _logger.LogWarning("system_admin NON TROUVÉ dans la table Functions!");
                }

                var roots = allFunctions.Where(f => f.ParentId == null);
                return Ok(BuildFunctionTree(roots, allFunctions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'arbre des fonctions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private List<FunctionTreeDto> BuildFunctionTree(IEnumerable<Function> nodes, List<Function> allFunctions)
        {
            return nodes.Select(f => new FunctionTreeDto
            {
                Id = f.Id,
                Code = f.Code,
                LabelFr = f.LabelFr,
                LabelEn = f.LabelEn,
                Icon = f.Icon,
                Route = f.Route,
                SortOrder = f.SortOrder,
                IsActive = f.IsActive,
                Children = BuildFunctionTree(allFunctions.Where(c => c.ParentId == f.Id), allFunctions)
            }).ToList();
        }

        /// <summary>
        /// Créer une nouvelle fonction
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Function>> Create(Function function)
        {
            try
            {
                // Vérifier si le code existe déjà
                if (await _context.Functions.AnyAsync(f => f.Code == function.Code))
                {
                    return BadRequest("Une fonction avec ce code existe déjà");
                }

                _context.Functions.Add(function);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = function.Id }, function);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la fonction");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour une fonction (mise à jour partielle supportée)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                var function = await _context.Functions.FindAsync(id);
                if (function == null)
                {
                    return NotFound($"Fonction avec l'ID {id} non trouvée");
                }

                // Appliquer les mises à jour
                if (updates.ContainsKey("sortOrder"))
                {
                    function.SortOrder = Convert.ToInt32(updates["sortOrder"]);
                }
                if (updates.ContainsKey("parentId"))
                {
                    var parentIdValue = updates["parentId"];
                    function.ParentId = parentIdValue == null ? null : Convert.ToInt32(parentIdValue);
                }
                if (updates.ContainsKey("isActive"))
                {
                    function.IsActive = Convert.ToBoolean(updates["isActive"]);
                }
                if (updates.ContainsKey("code"))
                {
                    var newCode = updates["code"]?.ToString();
                    // Vérifier uniquement si le code change
                    if (newCode != function.Code && await _context.Functions.AnyAsync(f => f.Code == newCode && f.Id != id))
                    {
                        return BadRequest("Une autre fonction avec ce code existe déjà");
                    }
                    function.Code = newCode;
                }
                if (updates.ContainsKey("labelFr"))
                {
                    function.LabelFr = updates["labelFr"]?.ToString();
                }
                if (updates.ContainsKey("labelEn"))
                {
                    function.LabelEn = updates["labelEn"]?.ToString();
                }
                if (updates.ContainsKey("descriptionFr"))
                {
                    function.DescriptionFr = updates["descriptionFr"]?.ToString();
                }
                if (updates.ContainsKey("descriptionEn"))
                {
                    function.DescriptionEn = updates["descriptionEn"]?.ToString();
                }
                if (updates.ContainsKey("icon"))
                {
                    function.Icon = updates["icon"]?.ToString();
                }
                if (updates.ContainsKey("route"))
                {
                    function.Route = updates["route"]?.ToString();
                }

                await _context.SaveChangesAsync();
                return Ok(function);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de la fonction {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour l'ordre d'une fonction (PATCH partiel)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                var function = await _context.Functions.FindAsync(id);
                if (function == null)
                {
                    return NotFound($"Fonction avec l'ID {id} non trouvée");
                }

                // Appliquer les mises à jour partielles
                if (updates.ContainsKey("sortOrder"))
                {
                    function.SortOrder = Convert.ToInt32(updates["sortOrder"]);
                }
                if (updates.ContainsKey("parentId"))
                {
                    var parentIdValue = updates["parentId"];
                    function.ParentId = parentIdValue == null ? null : Convert.ToInt32(parentIdValue);
                }
                if (updates.ContainsKey("isActive"))
                {
                    function.IsActive = Convert.ToBoolean(updates["isActive"]);
                }
                if (updates.ContainsKey("code"))
                {
                    var newCode = updates["code"]?.ToString();
                    // Vérifier uniquement si le code change
                    if (newCode != function.Code && await _context.Functions.AnyAsync(f => f.Code == newCode && f.Id != id))
                    {
                        return BadRequest("Une autre fonction avec ce code existe déjà");
                    }
                    function.Code = newCode;
                }
                if (updates.ContainsKey("labelFr"))
                {
                    function.LabelFr = updates["labelFr"]?.ToString();
                }
                if (updates.ContainsKey("labelEn"))
                {
                    function.LabelEn = updates["labelEn"]?.ToString();
                }
                if (updates.ContainsKey("icon"))
                {
                    function.Icon = updates["icon"]?.ToString();
                }
                if (updates.ContainsKey("route"))
                {
                    function.Route = updates["route"]?.ToString();
                }

                await _context.SaveChangesAsync();
                return Ok(function);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour partielle de la fonction {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer une fonction
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Vérifier s'il y a des enfants
                var hasChildren = await _context.Functions.AnyAsync(f => f.ParentId == id);
                if (hasChildren)
                {
                    return BadRequest("Impossible de supprimer une fonction ayant des enfants");
                }

                var function = await _context.Functions.FindAsync(id);
                if (function == null)
                {
                    return NotFound();
                }

                _context.Functions.Remove(function);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la fonction {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Initialiser les fonctions système par défaut
        /// </summary>
        [HttpPost("initialize")]
        public async Task<ActionResult> Initialize()
        {
            try
            {
                var createdFunctions = new List<Function>();

                // Fonctions racines (menus principaux)
                var rootFunctions = new List<Function>
                {
                    new Function { Code = "DASHBOARD", LabelFr = "Tableau de bord", LabelEn = "Dashboard", DescriptionFr = "Vue d'ensemble", DescriptionEn = "Overview", Icon = "dashboard", Route = "/dashboard", SortOrder = 1, IsActive = true },
                    new Function { Code = "USERS", LabelFr = "Utilisateurs", LabelEn = "Users", DescriptionFr = "Gestion des utilisateurs", DescriptionEn = "User management", Icon = "people", Route = "/users", SortOrder = 2, IsActive = true },
                    new Function { Code = "ROLES", LabelFr = "Rôles", LabelEn = "Roles", DescriptionFr = "Gestion des rôles", DescriptionEn = "Role management", Icon = "admin_panel_settings", Route = "/roles", SortOrder = 3, IsActive = true },
                    new Function { Code = "ORGANISATIONS", LabelFr = "Organisations", LabelEn = "Organisations", DescriptionFr = "Gestion des organisations", DescriptionEn = "Organisation management", Icon = "business", Route = "/organisations", SortOrder = 4, IsActive = true },
                    new Function { Code = "DOCUMENTS", LabelFr = "Documents", LabelEn = "Documents", DescriptionFr = "Gestion des documents", DescriptionEn = "Document management", Icon = "description", Route = "/documents", SortOrder = 5, IsActive = true },
                    new Function { Code = "REPORTS", LabelFr = "Rapports", LabelEn = "Reports", DescriptionFr = "Rapports et statistiques", DescriptionEn = "Reports and statistics", Icon = "assessment", Route = "/reports", SortOrder = 6, IsActive = true },
                    new Function { Code = "SETTINGS", LabelFr = "Paramètres", LabelEn = "Settings", DescriptionFr = "Configuration du système", DescriptionEn = "System configuration", Icon = "settings", Route = "/settings", SortOrder = 7, IsActive = true }
                };

                // Créer les fonctions racines
                foreach (var func in rootFunctions)
                {
                    if (!await _context.Functions.AnyAsync(f => f.Code == func.Code))
                    {
                        _context.Functions.Add(func);
                        createdFunctions.Add(func);
                    }
                }

                await _context.SaveChangesAsync();

                // Récupérer les IDs des fonctions parents créées
                var usersParent = await _context.Functions.FirstOrDefaultAsync(f => f.Code == "USERS");
                var rolesParent = await _context.Functions.FirstOrDefaultAsync(f => f.Code == "ROLES");
                var settingsParent = await _context.Functions.FirstOrDefaultAsync(f => f.Code == "SETTINGS");

                // Sous-fonctions
                var childFunctions = new List<Function>();

                if (usersParent != null)
                {
                    childFunctions.AddRange(new[]
                    {
                        new Function { Code = "USERS_LIST", LabelFr = "Liste des utilisateurs", LabelEn = "User list", ParentId = usersParent.Id, Icon = "list", Route = "/users/list", SortOrder = 1, IsActive = true },
                        new Function { Code = "USERS_CREATE", LabelFr = "Créer un utilisateur", LabelEn = "Create user", ParentId = usersParent.Id, Icon = "person_add", Route = "/users/create", SortOrder = 2, IsActive = true }
                    });
                }

                if (rolesParent != null)
                {
                    childFunctions.AddRange(new[]
                    {
                        new Function { Code = "ROLES_LIST", LabelFr = "Liste des rôles", LabelEn = "Role list", ParentId = rolesParent.Id, Icon = "list", Route = "/roles/list", SortOrder = 1, IsActive = true },
                        new Function { Code = "ROLES_CREATE", LabelFr = "Créer un rôle", LabelEn = "Create role", ParentId = rolesParent.Id, Icon = "add_circle", Route = "/roles/create", SortOrder = 2, IsActive = true }
                    });
                }

                if (settingsParent != null)
                {
                    childFunctions.AddRange(new[]
                    {
                        new Function { Code = "SETTINGS_GENERAL", LabelFr = "Paramètres généraux", LabelEn = "General settings", ParentId = settingsParent.Id, Icon = "tune", Route = "/settings/general", SortOrder = 1, IsActive = true },
                        new Function { Code = "SETTINGS_SECURITY", LabelFr = "Sécurité", LabelEn = "Security", ParentId = settingsParent.Id, Icon = "security", Route = "/settings/security", SortOrder = 2, IsActive = true }
                    });
                }

                foreach (var func in childFunctions)
                {
                    if (!await _context.Functions.AnyAsync(f => f.Code == func.Code))
                    {
                        _context.Functions.Add(func);
                        createdFunctions.Add(func);
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("{Count} fonctions initialisées", createdFunctions.Count);
                return Ok(new { message = $"{createdFunctions.Count} fonction(s) créée(s)", functions = createdFunctions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'initialisation des fonctions");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> FunctionExists(int id)
        {
            return await _context.Functions.AnyAsync(e => e.Id == id);
        }
    }
}
