using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using serveur.Data;

namespace serveur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HealthController> _logger;

        public HealthController(
            AppDbContext context,
            IConfiguration configuration,
            ILogger<HealthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Vérifie l'état de santé de l'API et de la connexion à la base de données
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new HealthCheckResult
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Database = await CheckDatabaseAsync()
            };

            result.Status = result.Database.IsConnected ? "Healthy" : "Unhealthy";

            return result.Database.IsConnected ? Ok(result) : StatusCode(503, result);
        }

        /// <summary>
        /// Vérifie uniquement la connexion à la base de données
        /// </summary>
        [HttpGet("database")]
        public async Task<IActionResult> CheckDatabase()
        {
            var dbStatus = await CheckDatabaseAsync();
            return dbStatus.IsConnected ? Ok(dbStatus) : StatusCode(503, dbStatus);
        }

        private async Task<DatabaseStatus> CheckDatabaseAsync()
        {
            var status = new DatabaseStatus
            {
                Server = GetServerName(),
                Database = GetDatabaseName()
            };

            try
            {
                // Tester la connexion avec un timeout court
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    status.IsConnected = true;
                    status.Message = "Connexion réussie";
                    _logger.LogInformation("Connexion à la base de données réussie: {Database}", status.Database);
                }
                else
                {
                    status.IsConnected = false;
                    status.Message = "Impossible de se connecter à la base de données";
                    _logger.LogWarning("Échec de connexion à la base de données: {Database}", status.Database);
                }
            }
            catch (Exception ex)
            {
                status.IsConnected = false;
                status.Message = $"Erreur: {ex.Message}";
                _logger.LogError(ex, "Erreur lors de la vérification de la connexion BD");
            }

            return status;
        }

        private string GetServerName()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString)) return "Non configuré";

            // Extraire le nom du serveur de la chaîne de connexion
            foreach (var part in connectionString.Split(';'))
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2 &&
                    keyValue[0].Trim().Equals("Server", StringComparison.OrdinalIgnoreCase))
                {
                    return keyValue[1].Trim();
                }
            }
            return "Inconnu";
        }

        private string GetDatabaseName()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString)) return "Non configuré";

            // Extraire le nom de la BD de la chaîne de connexion
            foreach (var part in connectionString.Split(';'))
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2 &&
                    keyValue[0].Trim().Equals("Database", StringComparison.OrdinalIgnoreCase))
                {
                    return keyValue[1].Trim();
                }
            }
            return "Inconnu";
        }
    }

    public class HealthCheckResult
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public DatabaseStatus Database { get; set; }
    }

    public class DatabaseStatus
    {
        public bool IsConnected { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Message { get; set; }
    }
}
