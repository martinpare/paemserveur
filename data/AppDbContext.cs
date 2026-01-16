using Microsoft.EntityFrameworkCore;
using serveur.Models.Entities;

namespace serveur.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Core entities
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<RoleFunction> RoleFunctions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Title> Titles { get; set; }

        // Pedagogical structure
        public DbSet<PedagogicalElementType> PedagogicalElementTypes { get; set; }
        public DbSet<PedagogicalStructure> PedagogicalStructures { get; set; }
        public DbSet<LearningCenter> LearningCenters { get; set; }
        public DbSet<AdministrationCenter> AdministrationCenters { get; set; }

        // Classifications
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<ClassificationNode> ClassificationNodes { get; set; }
        public DbSet<ClassificationNodeCriteria> ClassificationNodeCriteria { get; set; }

        // Documents
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentElement> DocumentElements { get; set; }
        public DbSet<DocumentMedia> DocumentMedias { get; set; }
        public DbSet<TemplatePage> TemplatePages { get; set; }

        // Items
        public DbSet<ItemBank> ItemBanks { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemBankClassification> ItemBankClassifications { get; set; }
        public DbSet<ItemVersion> ItemVersions { get; set; }
        public DbSet<ItemClassificationNode> ItemClassificationNodes { get; set; }
        public DbSet<Revision> Revisions { get; set; }
        public DbSet<Modification> Modifications { get; set; }
        public DbSet<Analysis> Analyses { get; set; }

        // Reports
        public DbSet<Report> Reports { get; set; }

        // Value domains
        public DbSet<ValueDomain> ValueDomains { get; set; }
        public DbSet<ValueDomainItem> ValueDomainItems { get; set; }

        // Authentication
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // Prompts
        public DbSet<Prompt> Prompts { get; set; }
        public DbSet<PromptVersion> PromptVersions { get; set; }
        public DbSet<PromptVersionComment> PromptVersionComments { get; set; }

        // Sessions
        public DbSet<Session> Sessions { get; set; }

        // Page contents
        public DbSet<PageContent> PageContents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Mail)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<Function>()
                .HasIndex(f => f.Code)
                .IsUnique();

            modelBuilder.Entity<ValueDomain>()
                .HasIndex(v => v.Tag)
                .IsUnique();

            // Configure self-referencing relationships
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Parent)
                .WithMany()
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Function>()
                .HasOne(f => f.Parent)
                .WithMany()
                .HasForeignKey(f => f.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Title>()
                .HasOne(t => t.Parent)
                .WithMany()
                .HasForeignKey(t => t.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PedagogicalStructure>()
                .HasOne(p => p.Parent)
                .WithMany()
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClassificationNode>()
                .HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
