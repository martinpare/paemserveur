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

        // Exams
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamPeriod> ExamPeriods { get; set; }
        public DbSet<ExamDocument> ExamDocuments { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<ExamParticipant> ExamParticipants { get; set; }
        public DbSet<ExamLog> ExamLogs { get; set; }
        public DbSet<ExamIncident> ExamIncidents { get; set; }
        public DbSet<ExamMessage> ExamMessages { get; set; }

        // Items
        public DbSet<ItemBank> ItemBanks { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemBankClassification> ItemBankClassifications { get; set; }
        public DbSet<ItemVersion> ItemVersions { get; set; }
        public DbSet<ItemClassificationNode> ItemClassificationNodes { get; set; }
        public DbSet<DocumentItemBank> DocumentItemBanks { get; set; }
        // Validation processes
        public DbSet<ValidationProcess> ValidationProcesses { get; set; }
        public DbSet<ValidationProcessStep> ValidationProcessSteps { get; set; }
        public DbSet<ValidationInstance> ValidationInstances { get; set; }
        public DbSet<ValidationInstanceStep> ValidationInstanceSteps { get; set; }
        public DbSet<ValidationInstanceStepValidation> ValidationInstanceStepValidations { get; set; }
        public DbSet<ValidationInstanceRole> ValidationInstanceRoles { get; set; }
        public DbSet<ValidationInstanceRoleUser> ValidationInstanceRoleUsers { get; set; }
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
        public DbSet<Convocation> Convocations { get; set; }

        // Groups
        public DbSet<Group> Groups { get; set; }

        // Learners
        public DbSet<Learner> Learners { get; set; }

        // Page contents
        public DbSet<PageContent> PageContents { get; set; }

        // Technological tools
        public DbSet<TechnologicalTool> TechnologicalTools { get; set; }
        public DbSet<GroupTechnologicalTool> GroupTechnologicalTools { get; set; }
        public DbSet<LearnerTechnologicalTool> LearnerTechnologicalTools { get; set; }

        // Text annotations (Highlighter)
        public DbSet<TextAnnotation> TextAnnotations { get; set; }

        // Words (for predictions)
        public DbSet<Word> Words { get; set; }

        // Dictionary sync (versioning)
        public DbSet<DictionaryMetadata> DictionaryMetadata { get; set; }
        public DbSet<DictionaryVersion> DictionaryVersions { get; set; }

        // Generic discussions (polymorphic)
        public DbSet<DiscussionThread> DiscussionThreads { get; set; }
        public DbSet<DiscussionMessage> DiscussionMessages { get; set; }
        public DbSet<DiscussionMention> DiscussionMentions { get; set; }

        // Dashboard layouts
        public DbSet<DashboardLayout> DashboardLayouts { get; set; }
        public DbSet<DashboardLayoutSlot> DashboardLayoutSlots { get; set; }

        // Teaching subjects
        public DbSet<TeachingSubject> TeachingSubjects { get; set; }

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

            modelBuilder.Entity<Learner>()
                .HasIndex(l => l.PermanentCode)
                .IsUnique();

            modelBuilder.Entity<Learner>()
                .HasIndex(l => l.Email)
                .IsUnique();

            // Validation process configurations
            modelBuilder.Entity<ValidationProcess>()
                .HasIndex(vp => new { vp.Code, vp.OrganisationId })
                .IsUnique();

            modelBuilder.Entity<ValidationProcessStep>()
                .HasIndex(vps => new { vps.ValidationProcessId, vps.StepOrder })
                .IsUnique();

            modelBuilder.Entity<ValidationInstanceStep>()
                .HasIndex(vis => new { vis.ValidationInstanceId, vis.ValidationProcessStepId })
                .IsUnique();

            modelBuilder.Entity<ValidationInstanceStepValidation>()
                .HasIndex(visv => new { visv.ValidationInstanceStepId, visv.ValidatedByUserId })
                .IsUnique();

            modelBuilder.Entity<ValidationInstanceRole>()
                .HasIndex(vir => new { vir.ValidationInstanceId, vir.RoleId })
                .IsUnique();

            modelBuilder.Entity<ValidationInstanceRoleUser>()
                .HasIndex(viru => new { viru.ValidationInstanceRoleId, viru.UserId })
                .IsUnique();

            // Configure ValidationInstance relationships
            // Steps collection (one-to-many)
            modelBuilder.Entity<ValidationInstance>()
                .HasMany(vi => vi.Steps)
                .WithOne(vis => vis.ValidationInstance)
                .HasForeignKey(vis => vis.ValidationInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            // CurrentStep (one-to-one, separate from Steps collection)
            modelBuilder.Entity<ValidationInstance>()
                .HasOne(vi => vi.CurrentStep)
                .WithOne()
                .HasForeignKey<ValidationInstance>(vi => vi.CurrentStepId)
                .OnDelete(DeleteBehavior.Restrict);

            // Word decimal precision configuration
            modelBuilder.Entity<Word>()
                .Property(w => w.FrequenceCp)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Word>()
                .Property(w => w.FrequenceCe1)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Word>()
                .Property(w => w.FrequenceCe2Cm2)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Word>()
                .Property(w => w.FrequenceGlobale)
                .HasColumnType("decimal(18,2)");

            // Generic discussion configurations
            modelBuilder.Entity<DiscussionThread>()
                .HasIndex(dt => new { dt.EntityType, dt.EntityId });

            modelBuilder.Entity<DiscussionMention>()
                .HasIndex(dm => new { dm.MessageId, dm.MentionedUserId })
                .IsUnique();

            // Self-referencing for discussion messages (replies)
            modelBuilder.Entity<DiscussionMessage>()
                .HasOne(m => m.ParentMessage)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.ParentMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamSession configurations
            modelBuilder.Entity<ExamSession>()
                .HasIndex(es => es.GroupCode)
                .IsUnique();

            modelBuilder.Entity<ExamSession>()
                .HasMany(es => es.Participants)
                .WithOne(ep => ep.ExamSession)
                .HasForeignKey(ep => ep.ExamSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSession>()
                .HasMany(es => es.Logs)
                .WithOne(el => el.ExamSession)
                .HasForeignKey(el => el.ExamSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamSession>()
                .HasMany(es => es.Messages)
                .WithOne(em => em.ExamSession)
                .HasForeignKey(em => em.ExamSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExamParticipant>()
                .HasMany(ep => ep.Incidents)
                .WithOne(ei => ei.ExamParticipant)
                .HasForeignKey(ei => ei.ExamParticipantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Exam self-referencing for multi-part exams
            modelBuilder.Entity<Exam>()
                .HasOne(e => e.ParentExam)
                .WithMany()
                .HasForeignKey(e => e.ParentExamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
