using Microsoft.EntityFrameworkCore;
using serveur.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Ressource> Ressources { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UtilisateurRole> UtilisateurRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<AttributionRolePortee> AttributionRolePortees { get; set; }

    // Tables de passation d'examen
    public DbSet<Passation> Passations { get; set; }
    public DbSet<FileOperationSynchronisation> FileOperationsSynchronisation { get; set; }

    // Vues en lecture seule
    public DbSet<UtilisateurComplet> UtilisateursComplets { get; set; }
    public DbSet<RoleComplet> RolesComplets { get; set; }
    public DbSet<PermissionRessource> PermissionsRessources { get; set; }
    public DbSet<PermissionUsage> PermissionsUsage { get; set; }
    public DbSet<PermissionOrpheline> PermissionsOrphelines { get; set; }
    public DbSet<RessourcePermissions> RessourcesPermissions { get; set; }

    // Vues pour la gestion des ressources
    public DbSet<RessourceComplete> RessourcesCompletes { get; set; }
    public DbSet<RessourceParType> RessourcesParType { get; set; }
    public DbSet<RessourceUtilisateur> RessourcesUtilisateurs { get; set; }

    // Vues pour la gestion avancée des permissions
    public DbSet<PermissionComplete> PermissionsCompletes { get; set; }
    public DbSet<PermissionParAction> PermissionsParAction { get; set; }
    public DbSet<PermissionRoleUtilisateur> PermissionsRoleUtilisateurs { get; set; }
    public DbSet<PermissionSansRessource> PermissionsSansRessource { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Clé composite pour UtilisateurRole
        modelBuilder.Entity<UtilisateurRole>()
            .HasKey(ur => new { ur.UtilisateurId, ur.RoleId });

        // Clé composite pour RolePermission
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Configuration des vues (lecture seule, pas de migrations)
        modelBuilder.Entity<UtilisateurComplet>()
            .ToView("v_utilisateur_complet")
            .HasNoKey();

        modelBuilder.Entity<RoleComplet>()
            .ToView("v_role_complet")
            .HasNoKey();

        modelBuilder.Entity<PermissionRessource>()
            .ToView("v_permission_ressource")
            .HasNoKey();

        modelBuilder.Entity<PermissionUsage>()
            .ToView("v_permission_usage")
            .HasNoKey();

        modelBuilder.Entity<PermissionOrpheline>()
            .ToView("v_permission_orpheline")
            .HasNoKey();

        modelBuilder.Entity<RessourcePermissions>()
            .ToView("v_ressource_permissions")
            .HasNoKey();

        // Vues pour la gestion des ressources
        modelBuilder.Entity<RessourceComplete>()
            .ToView("v_ressource_complete")
            .HasNoKey();

        modelBuilder.Entity<RessourceParType>()
            .ToView("v_ressource_par_type")
            .HasNoKey();

        modelBuilder.Entity<RessourceUtilisateur>()
            .ToView("v_ressource_utilisateur")
            .HasNoKey();

        // Vues pour la gestion avancée des permissions
        modelBuilder.Entity<PermissionComplete>()
            .ToView("v_permission_complete")
            .HasNoKey();

        modelBuilder.Entity<PermissionParAction>()
            .ToView("v_permission_par_action")
            .HasNoKey();

        modelBuilder.Entity<PermissionRoleUtilisateur>()
            .ToView("v_permission_role_utilisateur")
            .HasNoKey();

        modelBuilder.Entity<PermissionSansRessource>()
            .ToView("v_permission_sans_ressource")
            .HasNoKey();
    }
}