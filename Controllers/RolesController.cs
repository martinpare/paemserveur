using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dapper;
using serveur.Models;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtient tous les rôles (données de base)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        /// <summary>
        /// Obtient tous les rôles avec leurs permissions (via v_role_complet)
        /// </summary>
        [HttpGet("complets")]
        public async Task<ActionResult<IEnumerable<RoleComplet>>> GetRolesComplets()
        {
            return await _context.RolesComplets.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtient un rôle par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(long id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return role;
        }

        /// <summary>
        /// Obtient un rôle complet avec ses permissions (via v_role_complet)
        /// </summary>
        [HttpGet("{id}/complet")]
        public async Task<ActionResult<IEnumerable<RoleComplet>>> GetRoleComplet(long id)
        {
            var role = await _context.RolesComplets
                .AsNoTracking()
                .Where(r => r.RoleId == id)
                .ToListAsync();

            if (!role.Any())
            {
                return NotFound();
            }
            return role;
        }

        /// <summary>
        /// Crée un nouveau rôle
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        /// <summary>
        /// Copie un rôle existant avec ses permissions (via sp_copier_role)
        /// </summary>
        [HttpPost("{id}/copier")]
        public async Task<ActionResult<object>> CopierRole(long id, [FromBody] CopierRoleDto dto)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<CopierRoleResult>(
                "sp_copier_role",
                new { role_source_id = id, nouveau_nom = dto.NouveauNom, nouvelle_description = dto.NouvelleDescription },
                commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                return CreatedAtAction(nameof(GetRole), new { id = result.NouveauRoleId }, result);
            }
            return BadRequest("Erreur lors de la copie du rôle");
        }

        /// <summary>
        /// Met à jour un rôle
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(long id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Supprime un rôle
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(long id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class CopierRoleDto
    {
        public string NouveauNom { get; set; }
        public string NouvelleDescription { get; set; }
    }

    public class CopierRoleResult
    {
        public long NouveauRoleId { get; set; }
        public string Nom { get; set; }
    }
}
