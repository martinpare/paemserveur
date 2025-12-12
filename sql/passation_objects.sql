-- ============================================================================
-- SCRIPT DE CRÉATION DES OBJETS SQL POUR LA GESTION DES PASSATIONS
-- Base de données : MS SQL Server
-- ============================================================================

-- ============================================================================
-- TABLES
-- ============================================================================

-- Table principale des passations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Passations')
BEGIN
    CREATE TABLE Passations (
        Id                      NVARCHAR(36)    NOT NULL PRIMARY KEY,
        Version                 INT             NOT NULL DEFAULT 0,
        ExamenId                NVARCHAR(36)    NOT NULL,
        EtudiantId              NVARCHAR(36)    NOT NULL,
        Statut                  NVARCHAR(20)    NOT NULL DEFAULT 'non_demarre',
        DateDebut               DATETIME2       NULL,
        DateFin                 DATETIME2       NULL,
        TempsPauseTotalSec      INT             NOT NULL DEFAULT 0,
        NombrePauses            INT             NOT NULL DEFAULT 0,
        TempsActifSec           INT             NOT NULL DEFAULT 0,
        NombreDeconnexions      INT             NOT NULL DEFAULT 0,
        Reponses                NVARCHAR(MAX)   NULL,
        Configuration           NVARCHAR(MAX)   NULL,
        HistoriquePauses        NVARCHAR(MAX)   NULL,
        HistoriqueEvenements    NVARCHAR(MAX)   NULL,
        DerniereActivite        DATETIME2       NULL,
        DerniereSauvegarde      DATETIME2       NULL,
        NombreSauvegardes       INT             NOT NULL DEFAULT 0,
        DateCreation            DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        DateModification        DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT CK_Passations_Statut CHECK (
            Statut IN ('non_demarre', 'en_cours', 'pause', 'termine', 'soumis', 'annule')
        )
    );
END
GO

-- Table de file d'attente des opérations de synchronisation
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FileOperationsSynchronisation')
BEGIN
    CREATE TABLE FileOperationsSynchronisation (
        Id                      NVARCHAR(36)    NOT NULL PRIMARY KEY,
        PassationId             NVARCHAR(36)    NOT NULL,
        TypeOperation           NVARCHAR(50)    NOT NULL,
        VersionSource           INT             NOT NULL,
        Donnees                 NVARCHAR(MAX)   NOT NULL,
        HorodatageClient        DATETIME2       NOT NULL,
        HorodatageServeur       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        Statut                  NVARCHAR(20)    NOT NULL DEFAULT 'recu',

        CONSTRAINT FK_FileOperations_Passation
            FOREIGN KEY (PassationId) REFERENCES Passations(Id),

        CONSTRAINT CK_FileOperations_TypeOperation CHECK (
            TypeOperation IN ('REPONSE', 'PAUSE', 'REPRISE', 'EVENEMENT', 'SOUMISSION', 'DEMARRAGE', 'TERMINAISON')
        ),

        CONSTRAINT CK_FileOperations_Statut CHECK (
            Statut IN ('recu', 'traite', 'ignore')
        )
    );
END
GO


-- ============================================================================
-- INDEX
-- ============================================================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Passations_EtudiantId_Statut')
    CREATE INDEX IX_Passations_EtudiantId_Statut ON Passations(EtudiantId, Statut);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Passations_ExamenId_Statut')
    CREATE INDEX IX_Passations_ExamenId_Statut ON Passations(ExamenId, Statut);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Passations_Statut_DateDebut')
    CREATE INDEX IX_Passations_Statut_DateDebut ON Passations(Statut, DateDebut);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Passations_Version')
    CREATE INDEX IX_Passations_Version ON Passations(Id, Version);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FileOperations_PassationId_Version')
    CREATE INDEX IX_FileOperations_PassationId_Version ON FileOperationsSynchronisation(PassationId, VersionSource);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FileOperations_Statut')
    CREATE INDEX IX_FileOperations_Statut ON FileOperationsSynchronisation(Statut);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FileOperations_HorodatageServeur')
    CREATE INDEX IX_FileOperations_HorodatageServeur ON FileOperationsSynchronisation(HorodatageServeur);
GO


-- ============================================================================
-- TRIGGER
-- ============================================================================

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_Passations_MiseAJourDate')
    DROP TRIGGER TR_Passations_MiseAJourDate;
GO

CREATE TRIGGER TR_Passations_MiseAJourDate
ON Passations
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Passations
    SET DateModification = GETUTCDATE()
    FROM Passations p
    INNER JOIN inserted i ON p.Id = i.Id;
END;
GO


-- ============================================================================
-- PROCÉDURES STOCKÉES
-- ============================================================================

-- Procédure : Sauvegarder ou mettre à jour une passation
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_SauvegarderPassation')
    DROP PROCEDURE SP_SauvegarderPassation;
GO

CREATE PROCEDURE SP_SauvegarderPassation
    @Id                     NVARCHAR(36),
    @Version                INT,
    @ExamenId               NVARCHAR(36),
    @EtudiantId             NVARCHAR(36),
    @Statut                 NVARCHAR(20),
    @DateDebut              DATETIME2 = NULL,
    @DateFin                DATETIME2 = NULL,
    @TempsPauseTotalSec     INT = 0,
    @NombrePauses           INT = 0,
    @TempsActifSec          INT = 0,
    @NombreDeconnexions     INT = 0,
    @Reponses               NVARCHAR(MAX) = NULL,
    @Configuration          NVARCHAR(MAX) = NULL,
    @HistoriquePauses       NVARCHAR(MAX) = NULL,
    @HistoriqueEvenements   NVARCHAR(MAX) = NULL,
    @DerniereActivite       DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @VersionActuelle INT;

    -- Récupérer la version actuelle
    SELECT @VersionActuelle = Version
    FROM Passations
    WHERE Id = @Id;

    -- Si la passation n'existe pas, l'insérer
    IF @VersionActuelle IS NULL
    BEGIN
        INSERT INTO Passations (
            Id, Version, ExamenId, EtudiantId, Statut,
            DateDebut, DateFin, TempsPauseTotalSec, NombrePauses,
            TempsActifSec, NombreDeconnexions, Reponses, Configuration,
            HistoriquePauses, HistoriqueEvenements, DerniereActivite,
            DerniereSauvegarde, NombreSauvegardes
        )
        VALUES (
            @Id, @Version, @ExamenId, @EtudiantId, @Statut,
            @DateDebut, @DateFin, @TempsPauseTotalSec, @NombrePauses,
            @TempsActifSec, @NombreDeconnexions, @Reponses, @Configuration,
            @HistoriquePauses, @HistoriqueEvenements, @DerniereActivite,
            GETUTCDATE(), 1
        );

        SELECT @Version AS NouvelleVersion, 'INSERE' AS Resultat;
        RETURN;
    END

    -- Vérifier que la version client est supérieure
    IF @Version <= @VersionActuelle
    BEGIN
        SELECT @VersionActuelle AS NouvelleVersion, 'CONFLIT_VERSION' AS Resultat;
        RETURN;
    END

    -- Mettre à jour la passation
    UPDATE Passations
    SET
        Version = @Version,
        Statut = @Statut,
        DateDebut = @DateDebut,
        DateFin = @DateFin,
        TempsPauseTotalSec = @TempsPauseTotalSec,
        NombrePauses = @NombrePauses,
        TempsActifSec = @TempsActifSec,
        NombreDeconnexions = @NombreDeconnexions,
        Reponses = @Reponses,
        Configuration = @Configuration,
        HistoriquePauses = @HistoriquePauses,
        HistoriqueEvenements = @HistoriqueEvenements,
        DerniereActivite = @DerniereActivite,
        DerniereSauvegarde = GETUTCDATE(),
        NombreSauvegardes = NombreSauvegardes + 1
    WHERE Id = @Id;

    SELECT @Version AS NouvelleVersion, 'MIS_A_JOUR' AS Resultat;
END;
GO


-- Procédure : Récupérer une passation en cours pour un étudiant
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_ObtenirPassationEnCours')
    DROP PROCEDURE SP_ObtenirPassationEnCours;
GO

CREATE PROCEDURE SP_ObtenirPassationEnCours
    @EtudiantId NVARCHAR(36),
    @ExamenId   NVARCHAR(36) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        Id,
        Version,
        ExamenId,
        EtudiantId,
        Statut,
        DateDebut,
        DateFin,
        TempsPauseTotalSec,
        NombrePauses,
        TempsActifSec,
        NombreDeconnexions,
        Reponses,
        Configuration,
        HistoriquePauses,
        HistoriqueEvenements,
        DerniereActivite,
        DerniereSauvegarde,
        NombreSauvegardes,
        DateCreation,
        DateModification
    FROM Passations
    WHERE EtudiantId = @EtudiantId
      AND (@ExamenId IS NULL OR ExamenId = @ExamenId)
      AND Statut IN ('en_cours', 'pause')
    ORDER BY DateDebut DESC;
END;
GO


-- Procédure : Enregistrer une opération de synchronisation
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_EnregistrerOperationSync')
    DROP PROCEDURE SP_EnregistrerOperationSync;
GO

CREATE PROCEDURE SP_EnregistrerOperationSync
    @Id                 NVARCHAR(36),
    @PassationId        NVARCHAR(36),
    @TypeOperation      NVARCHAR(50),
    @VersionSource      INT,
    @Donnees            NVARCHAR(MAX),
    @HorodatageClient   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier si la passation existe
    IF NOT EXISTS (SELECT 1 FROM Passations WHERE Id = @PassationId)
    BEGIN
        RAISERROR('Passation introuvable', 16, 1);
        RETURN;
    END

    -- Insérer l'opération
    INSERT INTO FileOperationsSynchronisation (
        Id, PassationId, TypeOperation, VersionSource, Donnees, HorodatageClient, Statut
    )
    VALUES (
        @Id, @PassationId, @TypeOperation, @VersionSource, @Donnees, @HorodatageClient, 'recu'
    );

    SELECT @Id AS OperationId, 'ENREGISTRE' AS Resultat;
END;
GO


-- Procédure : Obtenir les statistiques d'une passation
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'SP_ObtenirStatistiquesPassation')
    DROP PROCEDURE SP_ObtenirStatistiquesPassation;
GO

CREATE PROCEDURE SP_ObtenirStatistiquesPassation
    @PassationId NVARCHAR(36)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.Id,
        p.Statut,
        p.Version,
        p.NombreSauvegardes,
        p.NombreDeconnexions,
        p.NombrePauses,
        p.TempsPauseTotalSec,
        p.TempsActifSec,
        DATEDIFF(SECOND, p.DateDebut, ISNULL(p.DateFin, GETUTCDATE())) AS DureeTotaleSec,
        (SELECT COUNT(*) FROM FileOperationsSynchronisation WHERE PassationId = p.Id) AS NombreOperationsSync,
        (SELECT COUNT(*) FROM FileOperationsSynchronisation WHERE PassationId = p.Id AND Statut = 'traite') AS OperationsTraitees,
        (SELECT COUNT(*) FROM FileOperationsSynchronisation WHERE PassationId = p.Id AND Statut = 'ignore') AS OperationsIgnorees
    FROM Passations p
    WHERE p.Id = @PassationId;
END;
GO


-- ============================================================================
-- FONCTIONS
-- ============================================================================

-- Fonction : Vérifier si une passation est en cours pour un étudiant
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'fn_etudiant_a_passation_en_cours' AND type = 'FN')
    DROP FUNCTION fn_etudiant_a_passation_en_cours;
GO

CREATE FUNCTION fn_etudiant_a_passation_en_cours
(
    @EtudiantId NVARCHAR(36),
    @ExamenId   NVARCHAR(36) = NULL
)
RETURNS BIT
AS
BEGIN
    DECLARE @Resultat BIT = 0;

    IF EXISTS (
        SELECT 1
        FROM Passations
        WHERE EtudiantId = @EtudiantId
          AND (@ExamenId IS NULL OR ExamenId = @ExamenId)
          AND Statut IN ('en_cours', 'pause')
    )
        SET @Resultat = 1;

    RETURN @Resultat;
END;
GO


-- Fonction : Obtenir le temps restant pour une passation
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'fn_obtenir_temps_restant' AND type = 'FN')
    DROP FUNCTION fn_obtenir_temps_restant;
GO

CREATE FUNCTION fn_obtenir_temps_restant
(
    @PassationId NVARCHAR(36)
)
RETURNS INT
AS
BEGIN
    DECLARE @TempsRestantSec INT = NULL;
    DECLARE @DureeMinutes INT;
    DECLARE @DateDebut DATETIME2;
    DECLARE @TempsPauseTotalSec INT;
    DECLARE @Configuration NVARCHAR(MAX);

    SELECT
        @DateDebut = DateDebut,
        @TempsPauseTotalSec = TempsPauseTotalSec,
        @Configuration = Configuration
    FROM Passations
    WHERE Id = @PassationId;

    -- Extraire la durée de la configuration (supposant un format JSON)
    -- Note: Cette logique dépend du format exact de la configuration
    IF @DateDebut IS NOT NULL AND @Configuration IS NOT NULL
    BEGIN
        -- Tenter d'extraire dureeMinutes du JSON
        SET @DureeMinutes = TRY_CAST(JSON_VALUE(@Configuration, '$.dureeMinutes') AS INT);

        IF @DureeMinutes IS NOT NULL
        BEGIN
            DECLARE @DureeTotaleSec INT = @DureeMinutes * 60;
            DECLARE @TempsEcouleSec INT = DATEDIFF(SECOND, @DateDebut, GETUTCDATE()) - @TempsPauseTotalSec;
            SET @TempsRestantSec = @DureeTotaleSec - @TempsEcouleSec;

            IF @TempsRestantSec < 0
                SET @TempsRestantSec = 0;
        END
    END

    RETURN @TempsRestantSec;
END;
GO


-- Fonction Table : Obtenir les passations d'un examen avec statistiques
IF EXISTS (SELECT * FROM sys.objects WHERE name = 'fn_obtenir_passations_examen' AND type = 'IF')
    DROP FUNCTION fn_obtenir_passations_examen;
GO

CREATE FUNCTION fn_obtenir_passations_examen
(
    @ExamenId NVARCHAR(36)
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        p.Id,
        p.EtudiantId,
        p.Statut,
        p.Version,
        p.DateDebut,
        p.DateFin,
        p.TempsActifSec,
        p.TempsPauseTotalSec,
        p.NombrePauses,
        p.NombreDeconnexions,
        p.NombreSauvegardes,
        p.DerniereSauvegarde,
        DATEDIFF(SECOND, p.DateDebut, ISNULL(p.DateFin, GETUTCDATE())) AS DureeTotaleSec
    FROM Passations p
    WHERE p.ExamenId = @ExamenId
);
GO


-- ============================================================================
-- VUES
-- ============================================================================

-- Vue : Passations actives (en cours ou en pause)
IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_passations_actives')
    DROP VIEW v_passations_actives;
GO

CREATE VIEW v_passations_actives
AS
SELECT
    p.Id,
    p.ExamenId,
    p.EtudiantId,
    p.Statut,
    p.Version,
    p.DateDebut,
    p.TempsActifSec,
    p.TempsPauseTotalSec,
    p.NombrePauses,
    p.NombreDeconnexions,
    p.DerniereActivite,
    p.DerniereSauvegarde,
    DATEDIFF(SECOND, p.DateDebut, GETUTCDATE()) - p.TempsPauseTotalSec AS TempsEcouleSec
FROM Passations p
WHERE p.Statut IN ('en_cours', 'pause');
GO


-- Vue : Résumé des passations par examen
IF EXISTS (SELECT * FROM sys.views WHERE name = 'v_resume_passations_examen')
    DROP VIEW v_resume_passations_examen;
GO

CREATE VIEW v_resume_passations_examen
AS
SELECT
    ExamenId,
    COUNT(*) AS NombrePassations,
    SUM(CASE WHEN Statut = 'non_demarre' THEN 1 ELSE 0 END) AS NonDemarrees,
    SUM(CASE WHEN Statut = 'en_cours' THEN 1 ELSE 0 END) AS EnCours,
    SUM(CASE WHEN Statut = 'pause' THEN 1 ELSE 0 END) AS EnPause,
    SUM(CASE WHEN Statut = 'termine' THEN 1 ELSE 0 END) AS Terminees,
    SUM(CASE WHEN Statut = 'soumis' THEN 1 ELSE 0 END) AS Soumises,
    SUM(CASE WHEN Statut = 'annule' THEN 1 ELSE 0 END) AS Annulees,
    AVG(TempsActifSec) AS TempsActifMoyenSec,
    AVG(NombreDeconnexions) AS DeconnexionsMoyennes,
    MAX(DerniereSauvegarde) AS DerniereSauvegarde
FROM Passations
GROUP BY ExamenId;
GO


PRINT 'Script passation_objects.sql exécuté avec succès.';
GO
