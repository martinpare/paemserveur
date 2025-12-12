using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serveur.Models;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttributionRolePorteesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttributionRolePorteesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttributionRolePortee>>> GetAttributionRolePortees()
        {
            return await _context.AttributionRolePortees
                .Include(a => a.Utilisateur)
                .Include(a => a.Role)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AttributionRolePortee>> GetAttributionRolePortee(long id)
        {
            var attribution = await _context.AttributionRolePortees
                .Include(a => a.Utilisateur)
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attribution == null)
            {
                return NotFound();
            }
            return attribution;
        }

        [HttpGet("utilisateur/{utilisateurId}")]
        public async Task<ActionResult<IEnumerable<AttributionRolePortee>>> GetByUtilisateur(long utilisateurId)
        {
            return await _context.AttributionRolePortees
                .Where(a => a.UtilisateurId == utilisateurId)
                .Include(a => a.Role)
                .ToListAsync();
        }

        [HttpGet("portee/{typePortee}/{porteeId}")]
        public async Task<ActionResult<IEnumerable<AttributionRolePortee>>> GetByPortee(string typePortee, long porteeId)
        {
            return await _context.AttributionRolePortees
                .Where(a => a.TypePortee == typePortee && a.PorteeId == porteeId)
                .Include(a => a.Utilisateur)
                .Include(a => a.Role)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<AttributionRolePortee>> PostAttributionRolePortee(AttributionRolePortee attribution)
        {
            _context.AttributionRolePortees.Add(attribution);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAttributionRolePortee), new { id = attribution.Id }, attribution);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAttributionRolePortee(long id, AttributionRolePortee attribution)
        {
            if (id != attribution.Id)
            {
                return BadRequest();
            }

            _context.Entry(attribution).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttributionRolePortee(long id)
        {
            var attribution = await _context.AttributionRolePortees.FindAsync(id);
            if (attribution == null)
            {
                return NotFound();
            }

            _context.AttributionRolePortees.Remove(attribution);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
