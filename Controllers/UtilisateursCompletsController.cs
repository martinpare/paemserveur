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
    public class UtilisateursCompletsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPermissionService _permissionService;

        public UtilisateursCompletsController(AppDbContext context, IPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Obtient tous les utilisateurs avec leurs rôles (via la vue v_utilisateur_complet)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilisateurComplet>>> GetUtilisateursComplets()
        {
            return await _context.UtilisateursComplets.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient un utilisateur complet par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UtilisateurComplet>>> GetUtilisateurComplet(long id)
        {
            var utilisateur = await _context.UtilisateursComplets
                .AsNoTracking()
                .Where(u => u.Id == id)
                .ToListAsync();

            if (!utilisateur.Any())
            {
                return NotFound();
            }
            return utilisateur;
        }

        /// <summary>
        /// Obtient les permissions d'un utilisateur (via procédure stockée)
        /// </summary>
        [HttpGet("{id}/permissions")]
        public async Task<ActionResult<IEnumerable<PermissionUtilisateur>>> GetPermissionsUtilisateur(long id)
        {
            var permissions = await _permissionService.ObtenirPermissionsUtilisateur(id);
            return Ok(permissions);
        }

        /// <summary>
        /// Vérifie si un utilisateur a une permission spécifique
        /// </summary>
        [HttpGet("{id}/a-permission/{codePermission}")]
        public async Task<ActionResult<bool>> UtilisateurAPermission(long id, string codePermission)
        {
            var aPermission = await _permissionService.UtilisateurAPermission(id, codePermission);
            return Ok(aPermission);
        }

        /// <summary>
        /// Vérifie si un utilisateur a une permission sur une ressource
        /// </summary>
        [HttpGet("{id}/a-permission-ressource")]
        public async Task<ActionResult<bool>> UtilisateurAPermissionRessource(
            long id,
            [FromQuery] string action,
            [FromQuery] string ressourceCode)
        {
            var aPermission = await _permissionService.UtilisateurAPermissionRessource(id, action, ressourceCode);
            return Ok(aPermission);
        }
    }
}
