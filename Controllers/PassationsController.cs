using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using serveur.Models;
using serveur.Services;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPassationService _passationService;

        public PassationsController(AppDbContext context, IPassationService passationService)
        {
            _context = context;
            _passationService = passationService;
        }

        #region CRUD de base

        /// <summary>
        /// Obtient toutes les passations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passation>>> GetPassations()
        {
            return await _context.Passations.ToListAsync();
        }

        /// <summary>
        /// Obtient une passation par son ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PassationCompleteDto>> GetPassation(string id)
        {
            var passation = await _passationService.ObtenirPassation(id);
            if (passation == null)
            {
                return NotFound();
            }
            return passation;
        }

        #endregion

        #region Sauvegarde et synchronisation

        /// <summary>
        /// Sauvegarde une passation (création ou mise à jour)
        /// Endpoint principal utilisé par le client pour la persistance
        /// </summary>
        [HttpPost("sauvegarder")]
        public async Task<ActionResult<PassationSauvegardeResultat>> SauvegarderPassation(PassationSauvegardeDto dto)
        {
            try
            {
                var resultat = await _passationService.SauvegarderPassation(dto);

                if (!resultat.Succes && resultat.Resultat == "CONFLIT_VERSION")
                {
                    return Conflict(resultat);
                }

                return Ok(resultat);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(new PassationSauvegardeResultat
                {
                    Succes = false,
                    Resultat = "ERREUR",
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Synchronise un lot d'opérations provenant du client hors-ligne
        /// </summary>
        [HttpPost("synchroniser")]
        public async Task<ActionResult<SynchronisationResultat>> SynchroniserOperations(LotOperationsSyncDto lot)
        {
            try
            {
                var resultat = await _passationService.SynchroniserOperations(lot);
                return Ok(resultat);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                return BadRequest(new SynchronisationResultat
                {
                    Succes = false,
                    OperationsEnErreur = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Vérifie l'état de synchronisation entre le client et le serveur
        /// </summary>
        [HttpGet("{id}/etat-sync")]
        public async Task<ActionResult<EtatSynchronisationDto>> VerifierEtatSynchronisation(
            string id,
            [FromQuery] int versionClient)
        {
            var etat = await _passationService.VerifierEtatSynchronisation(id, versionClient);
            return Ok(etat);
        }

        /// <summary>
        /// Obtient la version actuelle du serveur pour une passation
        /// </summary>
        [HttpGet("{id}/version")]
        public async Task<ActionResult<int>> ObtenirVersion(string id)
        {
            var version = await _passationService.ObtenirVersionServeur(id);
            return Ok(version);
        }

        #endregion

        #region Reprise et récupération

        /// <summary>
        /// Vérifie si une passation en cours existe pour un étudiant
        /// Utilisé pour la reprise après déconnexion
        /// </summary>
        [HttpGet("verifier-reprise")]
        public async Task<ActionResult<PassationRepriseResultat>> VerifierReprise(
            [FromQuery] string etudiantId,
            [FromQuery] string examenId = null)
        {
            if (string.IsNullOrEmpty(etudiantId))
            {
                return BadRequest("L'identifiant de l'étudiant est requis.");
            }

            var resultat = await _passationService.VerifierReprise(etudiantId, examenId);
            return Ok(resultat);
        }

        /// <summary>
        /// Obtient la passation en cours pour un étudiant
        /// </summary>
        [HttpGet("en-cours")]
        public async Task<ActionResult<PassationCompleteDto>> GetPassationEnCours(
            [FromQuery] string etudiantId,
            [FromQuery] string examenId = null)
        {
            if (string.IsNullOrEmpty(etudiantId))
            {
                return BadRequest("L'identifiant de l'étudiant est requis.");
            }

            var passation = await _passationService.ObtenirPassationEnCours(etudiantId, examenId);
            if (passation == null)
            {
                return NotFound("Aucune passation en cours trouvée.");
            }
            return Ok(passation);
        }

        #endregion

        #region Recherche

        /// <summary>
        /// Recherche des passations avec filtres
        /// </summary>
        [HttpPost("rechercher")]
        public async Task<ActionResult<IEnumerable<PassationCompleteDto>>> RechercherPassations(
            PassationRechercheParams parametres)
        {
            var passations = await _passationService.RechercherPassations(parametres);
            return Ok(passations);
        }

        /// <summary>
        /// Obtient toutes les passations d'un étudiant
        /// </summary>
        [HttpGet("etudiant/{etudiantId}")]
        public async Task<ActionResult<IEnumerable<PassationCompleteDto>>> GetPassationsEtudiant(string etudiantId)
        {
            var passations = await _passationService.RechercherPassations(
                new PassationRechercheParams { EtudiantId = etudiantId });
            return Ok(passations);
        }

        /// <summary>
        /// Obtient toutes les passations d'un examen
        /// </summary>
        [HttpGet("examen/{examenId}")]
        public async Task<ActionResult<IEnumerable<PassationCompleteDto>>> GetPassationsExamen(string examenId)
        {
            var passations = await _passationService.RechercherPassations(
                new PassationRechercheParams { ExamenId = examenId });
            return Ok(passations);
        }

        #endregion

        #region Opérations spécifiques

        /// <summary>
        /// Enregistre une réponse individuelle
        /// </summary>
        [HttpPost("reponse")]
        public async Task<ActionResult> EnregistrerReponse(ReponseDto dto)
        {
            var succes = await _passationService.EnregistrerReponse(dto);
            if (!succes)
            {
                return Conflict("Conflit de version ou passation introuvable.");
            }
            return Ok();
        }

        /// <summary>
        /// Change le statut d'une passation
        /// </summary>
        [HttpPut("{id}/statut")]
        public async Task<ActionResult> ChangerStatut(
            string id,
            [FromQuery] string statut,
            [FromQuery] int version)
        {
            // Valider le statut
            var statutsValides = new[]
            {
                StatutPassation.NonDemarre,
                StatutPassation.EnCours,
                StatutPassation.Pause,
                StatutPassation.Termine,
                StatutPassation.Soumis,
                StatutPassation.Annule
            };

            if (!System.Array.Exists(statutsValides, s => s == statut))
            {
                return BadRequest($"Statut invalide. Valeurs acceptées: {string.Join(", ", statutsValides)}");
            }

            var succes = await _passationService.ChangerStatut(id, statut, version);
            if (!succes)
            {
                return Conflict("Conflit de version ou passation introuvable.");
            }
            return Ok();
        }

        /// <summary>
        /// Soumet une passation (termine et marque comme soumise)
        /// </summary>
        [HttpPost("{id}/soumettre")]
        public async Task<ActionResult<PassationSauvegardeResultat>> SoumettrePassation(
            string id,
            [FromBody] PassationSauvegardeDto dto)
        {
            // Forcer le statut à "soumis"
            dto.Statut = StatutPassation.Soumis;
            dto.DateFin = System.DateTime.UtcNow;

            return await SauvegarderPassation(dto);
        }

        #endregion
    }
}
