using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serveur.Models;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UtilisateursController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            return await _context.Utilisateurs.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(long id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            return utilisateur;
        }

        [HttpPost]
        public async Task<ActionResult<Utilisateur>> PostUtilisateur(Utilisateur utilisateur)
        {
            _context.Utilisateurs.Add(utilisateur);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUtilisateur), new { id = utilisateur.Id }, utilisateur);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilisateur(long id, UtilisateurUpdateDto dto)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            utilisateur.NomUtilisateur = dto.NomUtilisateur;
            utilisateur.Courriel = dto.Courriel;
            utilisateur.Prenom = dto.Prenom;
            utilisateur.Nom = dto.Nom;
            utilisateur.EstActif = dto.EstActif;

            // Ne pas modifier le mot de passe si non fourni
            if (!string.IsNullOrEmpty(dto.MotDePasseHash))
            {
                utilisateur.MotDePasseHash = dto.MotDePasseHash;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilisateur(long id)
        {
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            _context.Utilisateurs.Remove(utilisateur);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
