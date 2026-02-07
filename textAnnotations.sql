IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [functions] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(50) NOT NULL,
        [labelFr] nvarchar(100) NOT NULL,
        [labelEn] nvarchar(100) NOT NULL,
        [descriptionFr] nvarchar(500) NULL,
        [descriptionEn] nvarchar(500) NULL,
        [parentId] int NULL,
        [sortOrder] int NOT NULL,
        [icon] nvarchar(50) NULL,
        [route] nvarchar(255) NULL,
        [isActive] bit NOT NULL,
        CONSTRAINT [PK_functions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_functions_functions_parentId] FOREIGN KEY ([parentId]) REFERENCES [functions] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [organisations] (
        [id] int NOT NULL IDENTITY,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NULL,
        [description] nvarchar(max) NULL,
        [isActive] bit NOT NULL,
        [acronym] nvarchar(50) NULL,
        [address] nvarchar(255) NULL,
        [city] nvarchar(100) NULL,
        [webSite] nvarchar(255) NULL,
        CONSTRAINT [PK_organisations] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [pageContents] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(50) NULL,
        [pedagogicalStrudtureId] int NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NULL,
        [descriptionFr] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        [contentFr] nvarchar(max) NOT NULL,
        [contentEn] nvarchar(max) NULL,
        [isEndPage] bit NOT NULL,
        [requiresConsent] bit NOT NULL,
        [consentTextFr] nvarchar(max) NULL,
        [consentTextEn] nvarchar(max) NULL,
        CONSTRAINT [PK_pageContents] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [prompts] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(255) NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        [content] nvarchar(max) NULL,
        CONSTRAINT [PK_prompts] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [sessions] (
        [id] int NOT NULL IDENTITY,
        [pedagogicalStructureId] int NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NULL,
        [testId] int NOT NULL,
        [scheduledAt] datetime2 NOT NULL,
        [authTypeId] int NOT NULL,
        [durationMinutes] int NOT NULL,
        [allowLanguageChange] bit NULL,
        [allowItemMarking] bit NULL,
        [allowNoteTaking] bit NULL,
        [allowItemScrambling] bit NULL,
        [allowHighContrast] bit NULL,
        [forceFullscreen] bit NULL,
        [enableKeyboardShortcuts] bit NULL,
        [enableGuidedTour] bit NULL,
        [questionSummaryNotice] int NULL,
        [enableRemoteMode] bit NULL,
        [debugMode] bit NULL,
        CONSTRAINT [PK_sessions] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [technologicalTools] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(50) NOT NULL,
        [nameFr] nvarchar(100) NOT NULL,
        [nameEn] nvarchar(100) NOT NULL,
        [descriptionFr] nvarchar(500) NULL,
        [descriptionEn] nvarchar(500) NULL,
        [icon] nvarchar(100) NULL,
        [displayOrder] int NOT NULL,
        [isActive] bit NOT NULL,
        [adaptiveMeasure] bit NOT NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_technologicalTools] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [titles] (
        [id] int NOT NULL IDENTITY,
        [parentId] int NULL,
        [code] nvarchar(50) NOT NULL,
        [maleLabelFr] nvarchar(100) NOT NULL,
        [femaleLabelFr] nvarchar(100) NOT NULL,
        [maleLabelEn] nvarchar(100) NOT NULL,
        [femaleLabelEn] nvarchar(100) NOT NULL,
        [order] int NOT NULL,
        [isActive] bit NOT NULL,
        CONSTRAINT [PK_titles] PRIMARY KEY ([id]),
        CONSTRAINT [FK_titles_titles_parentId] FOREIGN KEY ([parentId]) REFERENCES [titles] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [valueDomains] (
        [id] int NOT NULL IDENTITY,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [tag] nvarchar(100) NOT NULL,
        [isOrdered] bit NOT NULL,
        [isPublic] bit NOT NULL,
        [descriptionFr] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        CONSTRAINT [PK_valueDomains] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [administrationCenters] (
        [id] int NOT NULL IDENTITY,
        [organisationId] int NULL,
        [code] nvarchar(10) NOT NULL,
        [shortName] nvarchar(100) NOT NULL,
        [officialName] nvarchar(255) NULL,
        [address] nvarchar(255) NULL,
        [city] nvarchar(100) NULL,
        [province] int NULL,
        [country] int NULL,
        [postalCode] nvarchar(10) NULL,
        [email] nvarchar(255) NULL,
        [contactPhone] nvarchar(20) NULL,
        [contactExtension] nvarchar(10) NULL,
        CONSTRAINT [PK_administrationCenters] PRIMARY KEY ([id]),
        CONSTRAINT [FK_administrationCenters_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [learningCenters] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(10) NOT NULL,
        [internalCode] nvarchar(50) NULL,
        [shortName] nvarchar(100) NOT NULL,
        [officialName] nvarchar(255) NULL,
        [email] nvarchar(255) NULL,
        [website] nvarchar(255) NULL,
        [phone] nvarchar(20) NULL,
        [phoneExtension] nvarchar(10) NULL,
        [address] nvarchar(255) NULL,
        [city] nvarchar(100) NULL,
        [province] int NULL,
        [postalCode] nvarchar(10) NULL,
        [administrativeRegion] int NULL,
        [educationNetwork] int NULL,
        [educationLevel] nvarchar(max) NULL,
        [teachingLanguage] int NULL,
        [organisationId] int NULL,
        CONSTRAINT [PK_learningCenters] PRIMARY KEY ([id]),
        CONSTRAINT [FK_learningCenters_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [pedagogicalElementTypes] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(50) NOT NULL,
        [descriptionFr] nvarchar(255) NULL,
        [descriptionEn] nvarchar(255) NULL,
        [organisationId] int NULL,
        CONSTRAINT [PK_pedagogicalElementTypes] PRIMARY KEY ([id]),
        CONSTRAINT [FK_pedagogicalElementTypes_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [roles] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(50) NOT NULL,
        [nameFr] nvarchar(100) NOT NULL,
        [nameEn] nvarchar(100) NOT NULL,
        [descriptionFr] nvarchar(500) NULL,
        [descriptionEn] nvarchar(500) NULL,
        [organisationId] int NULL,
        [parentId] int NULL,
        [level] int NOT NULL,
        [isSystem] bit NOT NULL,
        [isActive] bit NOT NULL,
        [hasAllPermissions] bit NOT NULL,
        CONSTRAINT [PK_roles] PRIMARY KEY ([id]),
        CONSTRAINT [FK_roles_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_roles_roles_parentId] FOREIGN KEY ([parentId]) REFERENCES [roles] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [promptVersions] (
        [id] int NOT NULL IDENTITY,
        [promptId] int NOT NULL,
        [version] nvarchar(50) NOT NULL,
        [newContent] nvarchar(max) NULL,
        [active] bit NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_promptVersions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_promptVersions_prompts_promptId] FOREIGN KEY ([promptId]) REFERENCES [prompts] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [valueDomainItems] (
        [id] int NOT NULL IDENTITY,
        [valueDomainId] int NOT NULL,
        [order] int NOT NULL,
        [code] nvarchar(50) NULL,
        [valueFr] nvarchar(255) NOT NULL,
        [valueEn] nvarchar(255) NOT NULL,
        [descriptionFR] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        CONSTRAINT [PK_valueDomainItems] PRIMARY KEY ([id]),
        CONSTRAINT [FK_valueDomainItems_valueDomains_valueDomainId] FOREIGN KEY ([valueDomainId]) REFERENCES [valueDomains] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [pedagogicalStructures] (
        [id] int NOT NULL IDENTITY,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [pedagogicalElementTypeId] int NOT NULL,
        [sectorCode] nvarchar(10) NULL,
        [parentId] int NULL,
        [organisationId] int NULL,
        [sortOrder] int NULL,
        [createdAt] datetime2 NULL,
        CONSTRAINT [PK_pedagogicalStructures] PRIMARY KEY ([id]),
        CONSTRAINT [FK_pedagogicalStructures_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_pedagogicalStructures_pedagogicalElementTypes_pedagogicalElementTypeId] FOREIGN KEY ([pedagogicalElementTypeId]) REFERENCES [pedagogicalElementTypes] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_pedagogicalStructures_pedagogicalStructures_parentId] FOREIGN KEY ([parentId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [roleFunctions] (
        [id] int NOT NULL IDENTITY,
        [roleId] int NOT NULL,
        [functionCode] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_roleFunctions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_roleFunctions_roles_roleId] FOREIGN KEY ([roleId]) REFERENCES [roles] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [classifications] (
        [id] int NOT NULL IDENTITY,
        [pedagogicalStrudtureId] int NULL,
        [tag] nvarchar(100) NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(500) NOT NULL,
        [descriptionEn] nvarchar(500) NOT NULL,
        [isRequired] bit NOT NULL,
        [allowMultiple] bit NOT NULL,
        [hasDescription] bit NOT NULL,
        [isActive] bit NOT NULL,
        CONSTRAINT [PK_classifications] PRIMARY KEY ([id]),
        CONSTRAINT [FK_classifications_pedagogicalStructures_pedagogicalStrudtureId] FOREIGN KEY ([pedagogicalStrudtureId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [documentTypes] (
        [id] int NOT NULL IDENTITY,
        [pedagogicalStrudtureId] int NULL,
        [code] nvarchar(50) NOT NULL,
        [titleFr] nvarchar(255) NOT NULL,
        [titleEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        CONSTRAINT [PK_documentTypes] PRIMARY KEY ([id]),
        CONSTRAINT [FK_documentTypes_pedagogicalStructures_pedagogicalStrudtureId] FOREIGN KEY ([pedagogicalStrudtureId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [itemBanks] (
        [id] int NOT NULL IDENTITY,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(1000) NULL,
        [descriptionEn] nvarchar(1000) NULL,
        [pedagogicalStrudtureId] int NULL,
        [isActive] bit NULL,
        [hasFeedback] bit NULL,
        [hasFeedbackForChoices] bit NULL,
        [hasDocumentation] bit NULL,
        [hasDocumentationForChoices] bit NULL,
        CONSTRAINT [PK_itemBanks] PRIMARY KEY ([id]),
        CONSTRAINT [FK_itemBanks_pedagogicalStructures_pedagogicalStrudtureId] FOREIGN KEY ([pedagogicalStrudtureId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [reports] (
        [id] int NOT NULL IDENTITY,
        [code] nvarchar(100) NOT NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(1000) NULL,
        [descriptionEn] nvarchar(1000) NULL,
        [sortOrder] int NOT NULL,
        [isActive] bit NOT NULL,
        [organisationId] int NOT NULL,
        [pedagogicalStrudturId] int NULL,
        [useCompression] bit NOT NULL,
        [compressionMethod] nvarchar(100) NULL,
        [compressionPage] nvarchar(100) NULL,
        [widgetName] nvarchar(100) NOT NULL,
        [controllerMethod] nvarchar(100) NULL,
        CONSTRAINT [PK_reports] PRIMARY KEY ([id]),
        CONSTRAINT [FK_reports_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_reports_pedagogicalStructures_pedagogicalStrudturId] FOREIGN KEY ([pedagogicalStrudturId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [users] (
        [id] int NOT NULL IDENTITY,
        [firstName] nvarchar(100) NOT NULL,
        [lastName] nvarchar(100) NOT NULL,
        [sexe] nvarchar(1) NULL,
        [username] nvarchar(50) NOT NULL,
        [mail] nvarchar(255) NOT NULL,
        [password] nvarchar(255) NOT NULL,
        [darkMode] bit NOT NULL,
        [avatar] nvarchar(max) NULL,
        [accentColor] nvarchar(20) NULL,
        [language] nvarchar(10) NULL,
        [organisationId] int NULL,
        [pedagogicalStructureId] int NULL,
        [learningCenterId] int NULL,
        [titleId] int NULL,
        [activeRoleId] int NULL,
        [resetToken] nvarchar(255) NULL,
        [resetTokenExpiry] datetime2 NULL,
        [active] bit NOT NULL,
        CONSTRAINT [PK_users] PRIMARY KEY ([id]),
        CONSTRAINT [FK_users_learningCenters_learningCenterId] FOREIGN KEY ([learningCenterId]) REFERENCES [learningCenters] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_users_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_users_pedagogicalStructures_pedagogicalStructureId] FOREIGN KEY ([pedagogicalStructureId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_users_roles_activeRoleId] FOREIGN KEY ([activeRoleId]) REFERENCES [roles] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_users_titles_titleId] FOREIGN KEY ([titleId]) REFERENCES [titles] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [classificationNodes] (
        [id] int NOT NULL IDENTITY,
        [classificationId] int NOT NULL,
        [parentId] int NULL,
        [label] nvarchar(100) NOT NULL,
        [nameFr] nvarchar(255) NOT NULL,
        [nameEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(1000) NULL,
        [descriptionEn] nvarchar(1000) NULL,
        [sortOrder] int NOT NULL,
        [weight] nvarchar(50) NULL,
        [referencesJuridiques] nvarchar(2000) NULL,
        CONSTRAINT [PK_classificationNodes] PRIMARY KEY ([id]),
        CONSTRAINT [FK_classificationNodes_classificationNodes_parentId] FOREIGN KEY ([parentId]) REFERENCES [classificationNodes] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_classificationNodes_classifications_classificationId] FOREIGN KEY ([classificationId]) REFERENCES [classifications] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [itemBankClassifications] (
        [id] int NOT NULL IDENTITY,
        [itemBankId] int NOT NULL,
        [classificationId] int NOT NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_itemBankClassifications] PRIMARY KEY ([id]),
        CONSTRAINT [FK_itemBankClassifications_classifications_classificationId] FOREIGN KEY ([classificationId]) REFERENCES [classifications] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_itemBankClassifications_itemBanks_itemBankId] FOREIGN KEY ([itemBankId]) REFERENCES [itemBanks] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [documents] (
        [id] int NOT NULL IDENTITY,
        [pedagogicalStrudtureId] int NULL,
        [documentTypeId] int NULL,
        [externalReferenceCode] nvarchar(100) NULL,
        [version] nvarchar(20) NULL,
        [isActive] bit NOT NULL,
        [status] int NULL,
        [titleFr] nvarchar(255) NOT NULL,
        [titleEn] nvarchar(255) NOT NULL,
        [descriptionFr] nvarchar(max) NULL,
        [descriptionEn] nvarchar(max) NULL,
        [welcomeMessageFr] nvarchar(max) NULL,
        [welcomeMessageEn] nvarchar(max) NULL,
        [copyrightFr] nvarchar(max) NULL,
        [copyrightEn] nvarchar(max) NULL,
        [urlFr] nvarchar(max) NULL,
        [urlEn] nvarchar(max) NULL,
        [isDownloadable] bit NOT NULL,
        [isPublic] bit NOT NULL,
        [createdAt] datetime2 NULL,
        [isEditable] bit NOT NULL,
        [editorSettings] nvarchar(max) NULL,
        [authorId] int NULL,
        [isTemplate] bit NOT NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_documents] PRIMARY KEY ([id]),
        CONSTRAINT [FK_documents_documentTypes_documentTypeId] FOREIGN KEY ([documentTypeId]) REFERENCES [documentTypes] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_documents_pedagogicalStructures_pedagogicalStrudtureId] FOREIGN KEY ([pedagogicalStrudtureId]) REFERENCES [pedagogicalStructures] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_documents_users_authorId] FOREIGN KEY ([authorId]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [favorites] (
        [id] int NOT NULL IDENTITY,
        [userId] int NOT NULL,
        [functionId] int NOT NULL,
        [order] int NULL,
        CONSTRAINT [PK_favorites] PRIMARY KEY ([id]),
        CONSTRAINT [FK_favorites_functions_functionId] FOREIGN KEY ([functionId]) REFERENCES [functions] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_favorites_users_userId] FOREIGN KEY ([userId]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [groups] (
        [id] int NOT NULL IDENTITY,
        [name] nvarchar(100) NOT NULL,
        [learningCenterId] int NOT NULL,
        [gradeId] int NOT NULL,
        [teacherId] int NULL,
        [proctorId] int NULL,
        [languageId] int NOT NULL,
        [academicYearId] int NOT NULL,
        [allowManualEntry] bit NOT NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_groups] PRIMARY KEY ([id]),
        CONSTRAINT [FK_groups_learningCenters_learningCenterId] FOREIGN KEY ([learningCenterId]) REFERENCES [learningCenters] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_groups_users_proctorId] FOREIGN KEY ([proctorId]) REFERENCES [users] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_groups_users_teacherId] FOREIGN KEY ([teacherId]) REFERENCES [users] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_groups_valueDomainItems_academicYearId] FOREIGN KEY ([academicYearId]) REFERENCES [valueDomainItems] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_groups_valueDomainItems_gradeId] FOREIGN KEY ([gradeId]) REFERENCES [valueDomainItems] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_groups_valueDomainItems_languageId] FOREIGN KEY ([languageId]) REFERENCES [valueDomainItems] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [promptVersionComments] (
        [id] int NOT NULL IDENTITY,
        [promptVersionId] int NOT NULL,
        [userId] int NOT NULL,
        [content] nvarchar(max) NOT NULL,
        [createdAt] datetime2 NOT NULL,
        [updatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_promptVersionComments] PRIMARY KEY ([id]),
        CONSTRAINT [FK_promptVersionComments_promptVersions_promptVersionId] FOREIGN KEY ([promptVersionId]) REFERENCES [promptVersions] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_promptVersionComments_users_userId] FOREIGN KEY ([userId]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [refresh_tokens] (
        [id] int NOT NULL IDENTITY,
        [token] nvarchar(500) NOT NULL,
        [user_id] int NOT NULL,
        [expires_at] datetime2 NOT NULL,
        [created_at] datetime2 NOT NULL,
        [revoked_at] datetime2 NULL,
        [is_revoked] bit NOT NULL,
        [replaced_by_token] nvarchar(500) NULL,
        CONSTRAINT [PK_refresh_tokens] PRIMARY KEY ([id]),
        CONSTRAINT [FK_refresh_tokens_users_user_id] FOREIGN KEY ([user_id]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [userRoles] (
        [id] int NOT NULL IDENTITY,
        [userId] int NOT NULL,
        [roleId] int NOT NULL,
        [organisationId] int NULL,
        CONSTRAINT [PK_userRoles] PRIMARY KEY ([id]),
        CONSTRAINT [FK_userRoles_organisations_organisationId] FOREIGN KEY ([organisationId]) REFERENCES [organisations] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_userRoles_roles_roleId] FOREIGN KEY ([roleId]) REFERENCES [roles] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_userRoles_users_userId] FOREIGN KEY ([userId]) REFERENCES [users] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [classificationNodeCriteria] (
        [id] int NOT NULL IDENTITY,
        [classificationNodeId] int NOT NULL,
        [nameFr] nvarchar(500) NOT NULL,
        [nameEn] nvarchar(500) NULL,
        [isLegalObligation] bit NOT NULL,
        [examplesFr] nvarchar(max) NULL,
        [examplesEn] nvarchar(max) NULL,
        [sortOrder] int NOT NULL,
        CONSTRAINT [PK_classificationNodeCriteria] PRIMARY KEY ([id]),
        CONSTRAINT [FK_classificationNodeCriteria_classificationNodes_classificationNodeId] FOREIGN KEY ([classificationNodeId]) REFERENCES [classificationNodes] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [documentMedias] (
        [id] int NOT NULL IDENTITY,
        [documentId] int NOT NULL,
        [fileName] nvarchar(255) NOT NULL,
        [mimeType] nvarchar(100) NULL,
        [fileSize] int NULL,
        [dataUrl] nvarchar(max) NULL,
        [width] int NULL,
        [height] int NULL,
        [uploadedAt] datetime2 NULL,
        CONSTRAINT [PK_documentMedias] PRIMARY KEY ([id]),
        CONSTRAINT [FK_documentMedias_documents_documentId] FOREIGN KEY ([documentId]) REFERENCES [documents] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [items] (
        [id] int NOT NULL IDENTITY,
        [typeId] int NOT NULL,
        [label] nvarchar(500) NOT NULL,
        [statusId] int NOT NULL,
        [userId] int NULL,
        [documentId] int NULL,
        [itemBankId] int NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        [number] int NULL,
        [context] nvarchar(max) NULL,
        [statement] nvarchar(max) NOT NULL,
        [points] decimal(10,2) NULL,
        [required] bit NULL,
        [hint] nvarchar(max) NULL,
        [feedback] nvarchar(max) NULL,
        [media] nvarchar(max) NULL,
        [data] nvarchar(max) NULL,
        CONSTRAINT [PK_items] PRIMARY KEY ([id]),
        CONSTRAINT [FK_items_documents_documentId] FOREIGN KEY ([documentId]) REFERENCES [documents] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_items_itemBanks_itemBankId] FOREIGN KEY ([itemBankId]) REFERENCES [itemBanks] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_items_users_userId] FOREIGN KEY ([userId]) REFERENCES [users] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [templatePages] (
        [id] int NOT NULL IDENTITY,
        [templateDocumentId] int NOT NULL,
        [sortOrder] int NOT NULL,
        [pageNameFr] nvarchar(100) NULL,
        [pageNameEn] nvarchar(100) NULL,
        CONSTRAINT [PK_templatePages] PRIMARY KEY ([id]),
        CONSTRAINT [FK_templatePages_documents_templateDocumentId] FOREIGN KEY ([templateDocumentId]) REFERENCES [documents] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [groupTechnologicalTools] (
        [id] int NOT NULL IDENTITY,
        [groupId] int NOT NULL,
        [technologicalToolId] int NOT NULL,
        [createdAt] datetime2 NULL,
        CONSTRAINT [PK_groupTechnologicalTools] PRIMARY KEY ([id]),
        CONSTRAINT [FK_groupTechnologicalTools_groups_groupId] FOREIGN KEY ([groupId]) REFERENCES [groups] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_groupTechnologicalTools_technologicalTools_technologicalToolId] FOREIGN KEY ([technologicalToolId]) REFERENCES [technologicalTools] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [learners] (
        [id] int NOT NULL IDENTITY,
        [permanentCode] nvarchar(20) NULL,
        [firstName] nvarchar(100) NOT NULL,
        [lastName] nvarchar(100) NOT NULL,
        [dateOfBirth] datetime2 NOT NULL,
        [email] nvarchar(255) NULL,
        [nativeUserCode] nvarchar(50) NULL,
        [nativePassword] nvarchar(255) NULL,
        [learningCenterId] int NOT NULL,
        [groupId] int NULL,
        [languageId] int NOT NULL,
        [hasAccommodations] bit NOT NULL,
        [isManualEntry] bit NOT NULL,
        [isActive] bit NOT NULL,
        [createdAt] datetime2 NOT NULL,
        [modifiedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_learners] PRIMARY KEY ([id]),
        CONSTRAINT [FK_learners_groups_groupId] FOREIGN KEY ([groupId]) REFERENCES [groups] ([id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_learners_learningCenters_learningCenterId] FOREIGN KEY ([learningCenterId]) REFERENCES [learningCenters] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_learners_valueDomainItems_languageId] FOREIGN KEY ([languageId]) REFERENCES [valueDomainItems] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [analyses] (
        [id] int NOT NULL IDENTITY,
        [itemId] int NULL,
        [date] datetime2 NULL,
        [data] nvarchar(max) NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_analyses] PRIMARY KEY ([id]),
        CONSTRAINT [FK_analyses_items_itemId] FOREIGN KEY ([itemId]) REFERENCES [items] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [itemClassificationNodes] (
        [id] int NOT NULL IDENTITY,
        [itemId] int NOT NULL,
        [classificationNodeId] int NOT NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_itemClassificationNodes] PRIMARY KEY ([id]),
        CONSTRAINT [FK_itemClassificationNodes_classificationNodes_classificationNodeId] FOREIGN KEY ([classificationNodeId]) REFERENCES [classificationNodes] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_itemClassificationNodes_items_itemId] FOREIGN KEY ([itemId]) REFERENCES [items] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [itemVersions] (
        [id] int NOT NULL IDENTITY,
        [itemId] int NOT NULL,
        [version] int NOT NULL,
        [data] nvarchar(max) NULL,
        [date] datetime2 NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_itemVersions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_itemVersions_items_itemId] FOREIGN KEY ([itemId]) REFERENCES [items] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [modifications] (
        [id] int NOT NULL IDENTITY,
        [itemId] int NULL,
        [date] datetime2 NULL,
        [data] nvarchar(max) NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_modifications] PRIMARY KEY ([id]),
        CONSTRAINT [FK_modifications_items_itemId] FOREIGN KEY ([itemId]) REFERENCES [items] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [revisions] (
        [id] int NOT NULL IDENTITY,
        [itemId] int NULL,
        [date] datetime2 NULL,
        [data] nvarchar(max) NULL,
        [createdAt] datetime2 NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_revisions] PRIMARY KEY ([id]),
        CONSTRAINT [FK_revisions_items_itemId] FOREIGN KEY ([itemId]) REFERENCES [items] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [documentElements] (
        [id] int NOT NULL IDENTITY,
        [documentId] int NOT NULL,
        [sortOrder] int NULL,
        [elementType] nvarchar(50) NOT NULL,
        [content] nvarchar(max) NULL,
        [templatePageId] int NULL,
        [styleProps] nvarchar(max) NULL,
        CONSTRAINT [PK_documentElements] PRIMARY KEY ([id]),
        CONSTRAINT [FK_documentElements_documents_documentId] FOREIGN KEY ([documentId]) REFERENCES [documents] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_documentElements_templatePages_templatePageId] FOREIGN KEY ([templatePageId]) REFERENCES [templatePages] ([id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [learnerTechnologicalTools] (
        [id] int NOT NULL IDENTITY,
        [learnerId] int NOT NULL,
        [technologicalToolId] int NOT NULL,
        [createdAt] datetime2 NULL,
        CONSTRAINT [PK_learnerTechnologicalTools] PRIMARY KEY ([id]),
        CONSTRAINT [FK_learnerTechnologicalTools_learners_learnerId] FOREIGN KEY ([learnerId]) REFERENCES [learners] ([id]) ON DELETE CASCADE,
        CONSTRAINT [FK_learnerTechnologicalTools_technologicalTools_technologicalToolId] FOREIGN KEY ([technologicalToolId]) REFERENCES [technologicalTools] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE TABLE [textAnnotations] (
        [id] int NOT NULL IDENTITY,
        [learnerId] int NOT NULL,
        [contextId] nvarchar(255) NOT NULL,
        [annotationType] nvarchar(50) NOT NULL,
        [color] nvarchar(20) NOT NULL,
        [textContent] nvarchar(max) NOT NULL,
        [xPath] nvarchar(1000) NOT NULL,
        [startOffset] int NOT NULL,
        [endOffset] int NOT NULL,
        [createdAt] datetime2 NOT NULL,
        [updatedAt] datetime2 NULL,
        CONSTRAINT [PK_textAnnotations] PRIMARY KEY ([id]),
        CONSTRAINT [FK_textAnnotations_learners_learnerId] FOREIGN KEY ([learnerId]) REFERENCES [learners] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_administrationCenters_organisationId] ON [administrationCenters] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_analyses_itemId] ON [analyses] ([itemId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_classificationNodeCriteria_classificationNodeId] ON [classificationNodeCriteria] ([classificationNodeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_classificationNodes_classificationId] ON [classificationNodes] ([classificationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_classificationNodes_parentId] ON [classificationNodes] ([parentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_classifications_pedagogicalStrudtureId] ON [classifications] ([pedagogicalStrudtureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documentElements_documentId] ON [documentElements] ([documentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documentElements_templatePageId] ON [documentElements] ([templatePageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documentMedias_documentId] ON [documentMedias] ([documentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documents_authorId] ON [documents] ([authorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documents_documentTypeId] ON [documents] ([documentTypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documents_pedagogicalStrudtureId] ON [documents] ([pedagogicalStrudtureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_documentTypes_pedagogicalStrudtureId] ON [documentTypes] ([pedagogicalStrudtureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_favorites_functionId] ON [favorites] ([functionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_favorites_userId] ON [favorites] ([userId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE UNIQUE INDEX [IX_functions_code] ON [functions] ([code]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_functions_parentId] ON [functions] ([parentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_academicYearId] ON [groups] ([academicYearId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_gradeId] ON [groups] ([gradeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_languageId] ON [groups] ([languageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_learningCenterId] ON [groups] ([learningCenterId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_proctorId] ON [groups] ([proctorId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groups_teacherId] ON [groups] ([teacherId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groupTechnologicalTools_groupId] ON [groupTechnologicalTools] ([groupId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_groupTechnologicalTools_technologicalToolId] ON [groupTechnologicalTools] ([technologicalToolId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemBankClassifications_classificationId] ON [itemBankClassifications] ([classificationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemBankClassifications_itemBankId] ON [itemBankClassifications] ([itemBankId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemBanks_pedagogicalStrudtureId] ON [itemBanks] ([pedagogicalStrudtureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemClassificationNodes_classificationNodeId] ON [itemClassificationNodes] ([classificationNodeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemClassificationNodes_itemId] ON [itemClassificationNodes] ([itemId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_items_documentId] ON [items] ([documentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_items_itemBankId] ON [items] ([itemBankId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_items_userId] ON [items] ([userId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_itemVersions_itemId] ON [itemVersions] ([itemId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_learners_email] ON [learners] ([email]) WHERE [email] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learners_groupId] ON [learners] ([groupId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learners_languageId] ON [learners] ([languageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learners_learningCenterId] ON [learners] ([learningCenterId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_learners_permanentCode] ON [learners] ([permanentCode]) WHERE [permanentCode] IS NOT NULL');
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learnerTechnologicalTools_learnerId] ON [learnerTechnologicalTools] ([learnerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learnerTechnologicalTools_technologicalToolId] ON [learnerTechnologicalTools] ([technologicalToolId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_learningCenters_organisationId] ON [learningCenters] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_modifications_itemId] ON [modifications] ([itemId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_pedagogicalElementTypes_organisationId] ON [pedagogicalElementTypes] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_pedagogicalStructures_organisationId] ON [pedagogicalStructures] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_pedagogicalStructures_parentId] ON [pedagogicalStructures] ([parentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_pedagogicalStructures_pedagogicalElementTypeId] ON [pedagogicalStructures] ([pedagogicalElementTypeId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_promptVersionComments_promptVersionId] ON [promptVersionComments] ([promptVersionId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_promptVersionComments_userId] ON [promptVersionComments] ([userId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_promptVersions_promptId] ON [promptVersions] ([promptId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_refresh_tokens_user_id] ON [refresh_tokens] ([user_id]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_reports_organisationId] ON [reports] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_reports_pedagogicalStrudturId] ON [reports] ([pedagogicalStrudturId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_revisions_itemId] ON [revisions] ([itemId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_roleFunctions_roleId] ON [roleFunctions] ([roleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE UNIQUE INDEX [IX_roles_code] ON [roles] ([code]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_roles_organisationId] ON [roles] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_roles_parentId] ON [roles] ([parentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_templatePages_templateDocumentId] ON [templatePages] ([templateDocumentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_textAnnotations_learnerId] ON [textAnnotations] ([learnerId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_titles_parentId] ON [titles] ([parentId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_userRoles_organisationId] ON [userRoles] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_userRoles_roleId] ON [userRoles] ([roleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_userRoles_userId] ON [userRoles] ([userId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_users_activeRoleId] ON [users] ([activeRoleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_users_learningCenterId] ON [users] ([learningCenterId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE UNIQUE INDEX [IX_users_mail] ON [users] ([mail]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_users_organisationId] ON [users] ([organisationId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_users_pedagogicalStructureId] ON [users] ([pedagogicalStructureId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_users_titleId] ON [users] ([titleId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE UNIQUE INDEX [IX_users_username] ON [users] ([username]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE INDEX [IX_valueDomainItems_valueDomainId] ON [valueDomainItems] ([valueDomainId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    CREATE UNIQUE INDEX [IX_valueDomains_tag] ON [valueDomains] ([tag]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260122153637_AddTextAnnotations')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260122153637_AddTextAnnotations', N'5.0.17');
END;
GO

COMMIT;
GO

