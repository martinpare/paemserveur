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

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(AppDbContext context, ILogger<FavoritesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtenir tous les favoris
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetAll()
        {
            try
            {
                return await _context.Favorites
                    .OrderBy(f => f.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des favoris");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir un favori par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Favorite>> GetById(int id)
        {
            try
            {
                var favorite = await _context.Favorites.FindAsync(id);
                if (favorite == null)
                {
                    return NotFound();
                }
                return favorite;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du favori {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Obtenir les favoris d'un utilisateur
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetByUser(int userId)
        {
            try
            {
                return await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .OrderBy(f => f.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des favoris de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Créer un favori
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Favorite>> Create(Favorite favorite)
        {
            try
            {
                // Vérifier si le favori existe déjà
                var exists = await _context.Favorites
                    .AnyAsync(f => f.UserId == favorite.UserId && f.FunctionId == favorite.FunctionId);

                if (exists)
                {
                    return BadRequest("Cette fonction est déjà en favori");
                }

                // Calculer l'ordre si non spécifié
                if (!favorite.Order.HasValue || favorite.Order == 0)
                {
                    var maxOrder = await _context.Favorites
                        .Where(f => f.UserId == favorite.UserId)
                        .MaxAsync(f => (int?)f.Order) ?? 0;
                    favorite.Order = maxOrder + 1;
                }

                _context.Favorites.Add(favorite);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = favorite.Id }, favorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du favori");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Mettre à jour un favori
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Favorite favorite)
        {
            if (id != favorite.Id)
            {
                return BadRequest("L'ID ne correspond pas");
            }

            try
            {
                _context.Entry(favorite).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FavoriteExists(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du favori {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Réorganiser les favoris d'un utilisateur
        /// </summary>
        [HttpPut("user/{userId}/reorder")]
        public async Task<IActionResult> Reorder(int userId, [FromBody] List<int> favoriteIds)
        {
            try
            {
                var favorites = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .ToListAsync();

                for (int i = 0; i < favoriteIds.Count; i++)
                {
                    var favorite = favorites.FirstOrDefault(f => f.Id == favoriteIds[i]);
                    if (favorite != null)
                    {
                        favorite.Order = i + 1;
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réorganisation des favoris de l'utilisateur {UserId}", userId);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        /// <summary>
        /// Supprimer un favori
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var favorite = await _context.Favorites.FindAsync(id);
                if (favorite == null)
                {
                    return NotFound();
                }

                _context.Favorites.Remove(favorite);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du favori {Id}", id);
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        private async Task<bool> FavoriteExists(int id)
        {
            return await _context.Favorites.AnyAsync(e => e.Id == id);
        }
    }
}
