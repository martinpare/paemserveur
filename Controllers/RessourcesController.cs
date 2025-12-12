using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serveur.Models;
using serveur.Services;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RessourcesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRessourceService _ressourceService;

        public RessourcesController(AppDbContext context, IRessourceService ressourceService)
        {
            _context = context;
            _ressourceService = ressourceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ressource>>> GetRessources()
        {
            return await _context.Ressources.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ressource>> GetRessource(long id)
        {
            var ressource = await _context.Ressources.FindAsync(id);
            if (ressource == null)
            {
                return NotFound();
            }
            return ressource;
        }

        [HttpPost]
        public async Task<ActionResult<Ressource>> PostRessource(Ressource ressource)
        {
            _context.Ressources.Add(ressource);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRessource), new { id = ressource.Id }, ressource);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRessource(long id, Ressource ressource)
        {
            if (id != ressource.Id)
            {
                return BadRequest();
            }

            _context.Entry(ressource).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRessource(long id)
        {
            var ressource = await _context.Ressources.FindAsync(id);
            if (ressource == null)
            {
                return NotFound();
            }

            _context.Ressources.Remove(ressource);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        #region Endpoints utilisant les vues

        /// <summary>
        /// Obtient toutes les ressources avec leurs statistiques (via vue v_ressource_complete)
        /// </summary>
        [HttpGet("completes")]
        public async Task<ActionResult<IEnumerable<RessourceComplete>>> GetRessourcesCompletes()
        {
            return await _context.RessourcesCompletes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les ressources regroupées par type (via vue v_ressource_par_type)
        /// </summary>
        [HttpGet("par-type")]
        public async Task<ActionResult<IEnumerable<RessourceParType>>> GetRessourcesParType()
        {
            return await _context.RessourcesParType.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient les ressources accessibles par un utilisateur (via vue v_ressource_utilisateur)
        /// </summary>
        [HttpGet("utilisateur/{utilisateurId}")]
        public async Task<ActionResult<IEnumerable<RessourceUtilisateur>>> GetRessourcesUtilisateur(long utilisateurId)
        {
            return await _context.RessourcesUtilisateurs
                .AsNoTracking()
                .Where(r => r.UtilisateurId == utilisateurId)
                .ToListAsync();
        }

        #endregion

        #region Endpoints utilisant les procédures stockées

        /// <summary>
        /// Crée une ressource via procédure stockée avec validation
        /// </summary>
        [HttpPost("creer")]
        public async Task<ActionResult<long>> CreerRessource(RessourceCreationDto dto)
        {
            try
            {
                var id = await _ressourceService.CreerRessource(dto);
                if (id <= 0)
                {
                    return BadRequest("Erreur lors de la création de la ressource");
                }
                return CreatedAtAction(nameof(GetRessource), new { id }, id);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Modifie une ressource via procédure stockée
        /// </summary>
        [HttpPut("{id}/modifier")]
        public async Task<IActionResult> ModifierRessource(long id, RessourceCreationDto dto)
        {
            try
            {
                var success = await _ressourceService.ModifierRessource(id, dto);
                if (!success)
                {
                    return BadRequest("Erreur lors de la modification de la ressource");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Supprime une ressource de manière sécurisée via procédure stockée
        /// </summary>
        [HttpDelete("{id}/supprimer")]
        public async Task<IActionResult> SupprimerRessource(long id, [FromQuery] bool force = false)
        {
            try
            {
                var success = await _ressourceService.SupprimerRessource(id, force);
                if (!success)
                {
                    return BadRequest("Erreur lors de la suppression de la ressource");
                }
                return NoContent();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Duplique une ressource avec ses permissions
        /// </summary>
        [HttpPost("{id}/dupliquer")]
        public async Task<ActionResult<long>> DupliquerRessource(long id, RessourceDuplicationDto dto)
        {
            try
            {
                var nouvelId = await _ressourceService.DupliquerRessource(id, dto);
                if (nouvelId <= 0)
                {
                    return BadRequest("Erreur lors de la duplication de la ressource");
                }
                return CreatedAtAction(nameof(GetRessource), new { id = nouvelId }, nouvelId);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtient une ressource avec toutes ses statistiques
        /// </summary>
        [HttpGet("{id}/complete")]
        public async Task<ActionResult<RessourceAvecStats>> GetRessourceComplete(long id)
        {
            var ressource = await _ressourceService.ObtenirRessourceComplete(id);
            if (ressource == null)
            {
                return NotFound();
            }
            return ressource;
        }

        /// <summary>
        /// Recherche des ressources avec filtres
        /// </summary>
        [HttpPost("rechercher")]
        public async Task<ActionResult<IEnumerable<RessourceAvecStats>>> RechercherRessources(RessourceRechercheParams parametres)
        {
            var ressources = await _ressourceService.RechercherRessources(parametres);
            return Ok(ressources);
        }

        #endregion

        #region Endpoints utilisant les fonctions

        /// <summary>
        /// Vérifie si une ressource est utilisée
        /// </summary>
        [HttpGet("{id}/est-utilisee")]
        public async Task<ActionResult<bool>> RessourceEstUtilisee(long id)
        {
            return await _ressourceService.RessourceEstUtilisee(id);
        }

        /// <summary>
        /// Obtient le nombre de permissions liées à une ressource
        /// </summary>
        [HttpGet("{id}/nb-permissions")]
        public async Task<ActionResult<int>> GetNbPermissions(long id)
        {
            return await _ressourceService.ObtenirNbPermissions(id);
        }

        /// <summary>
        /// Obtient les ressources accessibles par un utilisateur (via fonction TVF)
        /// </summary>
        [HttpGet("par-utilisateur/{utilisateurId}")]
        public async Task<ActionResult<IEnumerable<RessourceUtilisateurDto>>> GetRessourcesParUtilisateur(long utilisateurId)
        {
            var ressources = await _ressourceService.ObtenirRessourcesUtilisateur(utilisateurId);
            return Ok(ressources);
        }

        /// <summary>
        /// Obtient les ressources filtrées par type (via fonction TVF)
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<RessourceAvecStats>>> GetRessourcesParTypeFiltre(string type)
        {
            var ressources = await _ressourceService.ObtenirRessourcesParType(type);
            return Ok(ressources);
        }

        #endregion
    }
}
