-- ============================================
-- Script de création des objets SQL pour la gestion des Permissions
-- Base de données: paem
-- Date: 2024
-- ============================================

USE paem;
GO

-- ============================================
-- INDEX
-- ============================================

-- Index pour recherche par code
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_permission_code' AND object_id = OBJECT_ID('permission'))
BEGIN
    CREATE UNIQUE INDEX IX_permission_code ON permission(code);
END
GO

-- Index pour recherche par action
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_permission_action' AND object_id = OBJECT_ID('permission'))
BEGIN
    CREATE INDEX IX_permission_action ON permission(action);
END
GO

-- Index composé action + ressource_id
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_permission_action_ressource' AND object_id = OBJECT_ID('permission'))
BEGIN
    CREATE INDEX IX_permission_action_ressource ON permission(action, ressource_id);
END
GO

-- ============================================
-- VUES
-- ============================================

-- Vue: v_permission_complete
-- Description: Vue consolidant les permissions avec leurs statistiques d'utilisation
IF OBJECT_ID('v_permission_complete', 'V') IS NOT NULL
    DROP VIEW v_permission_complete;
GO

CREATE VIEW v_permission_complete AS
SELECT
    p.id AS permission_id,
    p.code,
    p.action,
    p.description,
    p.cree_le,
    r.id AS ressource_id,
    r.type AS ressource_type,
    r.code AS ressource_code,
    r.nom AS ressource_nom,
    COUNT(DISTINCT rp.role_id) AS nb_roles,
    COUNT(DISTINCT ur.utilisateur_id) AS nb_utilisateurs
FROM permission p
LEFT JOIN ressource r ON r.id = p.ressource_id
LEFT JOIN role_permission rp ON rp.permission_id = p.id
LEFT JOIN utilisateur_role ur ON ur.role_id = rp.role_id
GROUP BY p.id, p.code, p.action, p.description, p.cree_le,
         r.id, r.type, r.code, r.nom;
GO

-- Vue: v_permission_par_action
-- Description: Regroupement des permissions par action avec comptage
IF OBJECT_ID('v_permission_par_action', 'V') IS NOT NULL
    DROP VIEW v_permission_par_action;
GO

CREATE VIEW v_permission_par_action AS
SELECT
    action,
    COUNT(*) AS nb_permissions,
    COUNT(DISTINCT ressource_id) AS nb_ressources_distinctes,
    STRING_AGG(code, ', ') WITHIN GROUP (ORDER BY code) AS codes
FROM permission
GROUP BY action;
GO

-- Vue: v_permission_role_utilisateur
-- Description: Permissions avec leurs rôles et utilisateurs associés
IF OBJECT_ID('v_permission_role_utilisateur', 'V') IS NOT NULL
    DROP VIEW v_permission_role_utilisateur;
GO

CREATE VIEW v_permission_role_utilisateur AS
SELECT
    p.id AS permission_id,
    p.code AS permission_code,
    p.action,
    r.id AS ressource_id,
    r.code AS ressource_code,
    r.type AS ressource_type,
    ro.id AS role_id,
    ro.nom AS role_nom,
    u.id AS utilisateur_id,
    u.nom AS utilisateur_nom,
    u.prenom AS utilisateur_prenom,
    u.courriel AS utilisateur_courriel,
    u.nom_utilisateur AS utilisateur_nom_utilisateur
FROM permission p
LEFT JOIN ressource r ON r.id = p.ressource_id
INNER JOIN role_permission rp ON rp.permission_id = p.id
INNER JOIN role ro ON ro.id = rp.role_id
INNER JOIN utilisateur_role ur ON ur.role_id = ro.id
INNER JOIN utilisateur u ON u.id = ur.utilisateur_id;
GO

-- Vue: v_permission_sans_ressource
-- Description: Permissions non liées à une ressource
IF OBJECT_ID('v_permission_sans_ressource', 'V') IS NOT NULL
    DROP VIEW v_permission_sans_ressource;
GO

CREATE VIEW v_permission_sans_ressource AS
SELECT
    p.id AS permission_id,
    p.code,
    p.action,
    p.description,
    p.cree_le,
    COUNT(DISTINCT rp.role_id) AS nb_roles
FROM permission p
LEFT JOIN role_permission rp ON rp.permission_id = p.id
WHERE p.ressource_id IS NULL
GROUP BY p.id, p.code, p.action, p.description, p.cree_le;
GO

-- ============================================
-- FONCTIONS
-- ============================================

-- Fonction: fn_permission_est_utilisee
-- Description: Vérifie si une permission est assignée à au moins un rôle
IF OBJECT_ID('fn_permission_est_utilisee', 'FN') IS NOT NULL
    DROP FUNCTION fn_permission_est_utilisee;
GO

CREATE FUNCTION fn_permission_est_utilisee(@permission_id BIGINT)
RETURNS BIT
AS
BEGIN
    DECLARE @est_utilisee BIT = 0;

    IF EXISTS (SELECT 1 FROM role_permission WHERE permission_id = @permission_id)
        SET @est_utilisee = 1;

    RETURN @est_utilisee;
END;
GO

-- Fonction: fn_permission_nb_roles
-- Description: Retourne le nombre de rôles ayant cette permission
IF OBJECT_ID('fn_permission_nb_roles', 'FN') IS NOT NULL
    DROP FUNCTION fn_permission_nb_roles;
GO

CREATE FUNCTION fn_permission_nb_roles(@permission_id BIGINT)
RETURNS INT
AS
BEGIN
    DECLARE @nb INT;
    SELECT @nb = COUNT(*) FROM role_permission WHERE permission_id = @permission_id;
    RETURN ISNULL(@nb, 0);
END;
GO

-- Fonction: fn_permission_nb_utilisateurs
-- Description: Retourne le nombre d'utilisateurs ayant cette permission (via leurs rôles)
IF OBJECT_ID('fn_permission_nb_utilisateurs', 'FN') IS NOT NULL
    DROP FUNCTION fn_permission_nb_utilisateurs;
GO

CREATE FUNCTION fn_permission_nb_utilisateurs(@permission_id BIGINT)
RETURNS INT
AS
BEGIN
    DECLARE @nb INT;
    SELECT @nb = COUNT(DISTINCT ur.utilisateur_id)
    FROM role_permission rp
    INNER JOIN utilisateur_role ur ON ur.role_id = rp.role_id
    WHERE rp.permission_id = @permission_id;
    RETURN ISNULL(@nb, 0);
END;
GO

-- Fonction TVF: fn_obtenir_permissions_role
-- Description: Retourne toutes les permissions d'un rôle
IF OBJECT_ID('fn_obtenir_permissions_role', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_permissions_role;
GO

CREATE FUNCTION fn_obtenir_permissions_role(@role_id BIGINT)
RETURNS TABLE
AS
RETURN
(
    SELECT
        p.id AS permission_id,
        p.code,
        p.action,
        p.description,
        r.id AS ressource_id,
        r.type AS ressource_type,
        r.code AS ressource_code,
        r.nom AS ressource_nom,
        rp.accorde_le
    FROM permission p
    LEFT JOIN ressource r ON r.id = p.ressource_id
    INNER JOIN role_permission rp ON rp.permission_id = p.id
    WHERE rp.role_id = @role_id
);
GO

-- Fonction TVF: fn_obtenir_permissions_par_action
-- Description: Retourne les permissions filtrées par action
IF OBJECT_ID('fn_obtenir_permissions_par_action', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_permissions_par_action;
GO

CREATE FUNCTION fn_obtenir_permissions_par_action(@action VARCHAR(50))
RETURNS TABLE
AS
RETURN
(
    SELECT
        p.id AS permission_id,
        p.code,
        p.action,
        p.description,
        p.cree_le,
        r.id AS ressource_id,
        r.type AS ressource_type,
        r.code AS ressource_code,
        r.nom AS ressource_nom,
        COUNT(DISTINCT rp.role_id) AS nb_roles
    FROM permission p
    LEFT JOIN ressource r ON r.id = p.ressource_id
    LEFT JOIN role_permission rp ON rp.permission_id = p.id
    WHERE p.action = @action
    GROUP BY p.id, p.code, p.action, p.description, p.cree_le,
             r.id, r.type, r.code, r.nom
);
GO

-- Fonction TVF: fn_obtenir_roles_permission
-- Description: Retourne tous les rôles ayant une permission spécifique
IF OBJECT_ID('fn_obtenir_roles_permission', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_roles_permission;
GO

CREATE FUNCTION fn_obtenir_roles_permission(@permission_id BIGINT)
RETURNS TABLE
AS
RETURN
(
    SELECT
        ro.id AS role_id,
        ro.nom AS role_nom,
        ro.description AS role_description,
        rp.accorde_le,
        COUNT(DISTINCT ur.utilisateur_id) AS nb_utilisateurs
    FROM role ro
    INNER JOIN role_permission rp ON rp.role_id = ro.id
    LEFT JOIN utilisateur_role ur ON ur.role_id = ro.id
    WHERE rp.permission_id = @permission_id
    GROUP BY ro.id, ro.nom, ro.description, rp.accorde_le
);
GO

-- Fonction TVF: fn_obtenir_utilisateurs_permission
-- Description: Retourne tous les utilisateurs ayant une permission spécifique
IF OBJECT_ID('fn_obtenir_utilisateurs_permission', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_utilisateurs_permission;
GO

CREATE FUNCTION fn_obtenir_utilisateurs_permission(@permission_id BIGINT)
RETURNS TABLE
AS
RETURN
(
    SELECT DISTINCT
        u.id AS utilisateur_id,
        u.nom,
        u.prenom,
        u.courriel,
        u.nom_utilisateur,
        u.est_actif,
        ro.id AS role_id,
        ro.nom AS role_nom
    FROM utilisateur u
    INNER JOIN utilisateur_role ur ON ur.utilisateur_id = u.id
    INNER JOIN role ro ON ro.id = ur.role_id
    INNER JOIN role_permission rp ON rp.role_id = ro.id
    WHERE rp.permission_id = @permission_id
);
GO

-- ============================================
-- PROCEDURES STOCKEES
-- ============================================

-- Procédure: sp_creer_permission
-- Description: Création d'une permission avec validation
IF OBJECT_ID('sp_creer_permission', 'P') IS NOT NULL
    DROP PROCEDURE sp_creer_permission;
GO

CREATE PROCEDURE sp_creer_permission
    @code VARCHAR(150),
    @action VARCHAR(50),
    @ressource_id BIGINT = NULL,
    @description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier si le code existe déjà
    IF EXISTS (SELECT 1 FROM permission WHERE code = @code)
    BEGIN
        RAISERROR('Une permission avec ce code existe déjà.', 16, 1);
        RETURN;
    END

    -- Vérifier si la ressource existe (si spécifiée)
    IF @ressource_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ressource WHERE id = @ressource_id)
    BEGIN
        RAISERROR('La ressource spécifiée n''existe pas.', 16, 1);
        RETURN;
    END

    -- Vérifier si une permission avec la même action existe déjà pour cette ressource
    IF @ressource_id IS NOT NULL AND EXISTS (
        SELECT 1 FROM permission WHERE action = @action AND ressource_id = @ressource_id
    )
    BEGIN
        RAISERROR('Une permission avec cette action existe déjà pour cette ressource.', 16, 1);
        RETURN;
    END

    INSERT INTO permission (code, action, ressource_id, description, cree_le)
    VALUES (@code, @action, @ressource_id, @description, SYSDATETIMEOFFSET());

    SELECT
        SCOPE_IDENTITY() AS PermissionId,
        @code AS Code,
        @action AS Action,
        @ressource_id AS RessourceId;
END;
GO

-- Procédure: sp_modifier_permission
-- Description: Modification d'une permission existante
IF OBJECT_ID('sp_modifier_permission', 'P') IS NOT NULL
    DROP PROCEDURE sp_modifier_permission;
GO

CREATE PROCEDURE sp_modifier_permission
    @id BIGINT,
    @code VARCHAR(150),
    @action VARCHAR(50),
    @ressource_id BIGINT = NULL,
    @description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la permission existe
    IF NOT EXISTS (SELECT 1 FROM permission WHERE id = @id)
    BEGIN
        RAISERROR('Permission introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si le nouveau code existe déjà pour une autre permission
    IF EXISTS (SELECT 1 FROM permission WHERE code = @code AND id <> @id)
    BEGIN
        RAISERROR('Une autre permission avec ce code existe déjà.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si la ressource existe (si spécifiée)
    IF @ressource_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM ressource WHERE id = @ressource_id)
    BEGIN
        RAISERROR('La ressource spécifiée n''existe pas.', 16, 1);
        RETURN -1;
    END

    UPDATE permission
    SET code = @code,
        action = @action,
        ressource_id = @ressource_id,
        description = @description
    WHERE id = @id;

    RETURN 0;
END;
GO

-- Procédure: sp_supprimer_permission
-- Description: Suppression sécurisée d'une permission avec ses associations
IF OBJECT_ID('sp_supprimer_permission', 'P') IS NOT NULL
    DROP PROCEDURE sp_supprimer_permission;
GO

CREATE PROCEDURE sp_supprimer_permission
    @permission_id BIGINT,
    @force BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la permission existe
    IF NOT EXISTS (SELECT 1 FROM permission WHERE id = @permission_id)
    BEGIN
        RAISERROR('Permission introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si des rôles utilisent cette permission
    DECLARE @nb_roles INT;
    SELECT @nb_roles = COUNT(*) FROM role_permission WHERE permission_id = @permission_id;

    IF @nb_roles > 0 AND @force = 0
    BEGIN
        DECLARE @msg NVARCHAR(200);
        SET @msg = 'Cette permission est assignée à ' + CAST(@nb_roles AS NVARCHAR(10)) + ' rôle(s). Utilisez @force=1 pour forcer la suppression.';
        RAISERROR(@msg, 16, 1);
        RETURN -1;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Supprimer les associations role_permission
        DELETE FROM role_permission WHERE permission_id = @permission_id;

        -- Supprimer la permission
        DELETE FROM permission WHERE id = @permission_id;

        COMMIT;
        RETURN 0;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

-- Procédure: sp_creer_permissions_crud
-- Description: Crée les 4 permissions CRUD pour une ressource
IF OBJECT_ID('sp_creer_permissions_crud', 'P') IS NOT NULL
    DROP PROCEDURE sp_creer_permissions_crud;
GO

CREATE PROCEDURE sp_creer_permissions_crud
    @ressource_id BIGINT,
    @prefixe_code VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ressource_code VARCHAR(150);
    SELECT @ressource_code = code FROM ressource WHERE id = @ressource_id;

    IF @ressource_code IS NULL
    BEGIN
        RAISERROR('Ressource introuvable.', 16, 1);
        RETURN;
    END

    -- Utiliser le code de la ressource comme préfixe si non spécifié
    SET @prefixe_code = ISNULL(@prefixe_code, @ressource_code);

    DECLARE @actions TABLE (action VARCHAR(50), description NVARCHAR(255));
    INSERT INTO @actions VALUES
        ('CREER', 'Créer'),
        ('LIRE', 'Lire/Consulter'),
        ('MODIFIER', 'Modifier'),
        ('SUPPRIMER', 'Supprimer');

    DECLARE @resultats TABLE (
        PermissionId BIGINT,
        Code VARCHAR(150),
        Action VARCHAR(50),
        Statut VARCHAR(20)
    );

    DECLARE @action VARCHAR(50), @desc NVARCHAR(255), @code VARCHAR(150), @new_id BIGINT;

    DECLARE action_cursor CURSOR FOR SELECT action, description FROM @actions;
    OPEN action_cursor;
    FETCH NEXT FROM action_cursor INTO @action, @desc;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @code = @prefixe_code + '_' + @action;

        -- Vérifier si cette permission existe déjà
        IF NOT EXISTS (SELECT 1 FROM permission WHERE code = @code)
        BEGIN
            INSERT INTO permission (code, action, ressource_id, description, cree_le)
            VALUES (@code, @action, @ressource_id, @desc + ' ' + @ressource_code, SYSDATETIMEOFFSET());

            SET @new_id = SCOPE_IDENTITY();
            INSERT INTO @resultats VALUES (@new_id, @code, @action, 'Créée');
        END
        ELSE
        BEGIN
            SELECT @new_id = id FROM permission WHERE code = @code;
            INSERT INTO @resultats VALUES (@new_id, @code, @action, 'Existante');
        END

        FETCH NEXT FROM action_cursor INTO @action, @desc;
    END

    CLOSE action_cursor;
    DEALLOCATE action_cursor;

    SELECT * FROM @resultats;
END;
GO

-- Procédure: sp_dupliquer_permission
-- Description: Duplique une permission avec un nouveau code
IF OBJECT_ID('sp_dupliquer_permission', 'P') IS NOT NULL
    DROP PROCEDURE sp_dupliquer_permission;
GO

CREATE PROCEDURE sp_dupliquer_permission
    @permission_id BIGINT,
    @nouveau_code VARCHAR(150),
    @nouvelle_ressource_id BIGINT = NULL,
    @nouvelle_permission_id BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la permission source existe
    IF NOT EXISTS (SELECT 1 FROM permission WHERE id = @permission_id)
    BEGIN
        RAISERROR('Permission source introuvable.', 16, 1);
        SET @nouvelle_permission_id = -1;
        RETURN -1;
    END

    -- Vérifier si le nouveau code existe déjà
    IF EXISTS (SELECT 1 FROM permission WHERE code = @nouveau_code)
    BEGIN
        RAISERROR('Une permission avec ce code existe déjà.', 16, 1);
        SET @nouvelle_permission_id = -1;
        RETURN -1;
    END

    INSERT INTO permission (code, action, ressource_id, description, cree_le)
    SELECT
        @nouveau_code,
        action,
        COALESCE(@nouvelle_ressource_id, ressource_id),
        description,
        SYSDATETIMEOFFSET()
    FROM permission WHERE id = @permission_id;

    SET @nouvelle_permission_id = SCOPE_IDENTITY();
    RETURN 0;
END;
GO

-- Procédure: sp_obtenir_permission_complete
-- Description: Retourne une permission avec toutes ses statistiques
IF OBJECT_ID('sp_obtenir_permission_complete', 'P') IS NOT NULL
    DROP PROCEDURE sp_obtenir_permission_complete;
GO

CREATE PROCEDURE sp_obtenir_permission_complete
    @permission_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.id AS permission_id,
        p.code,
        p.action,
        p.description,
        p.cree_le,
        r.id AS ressource_id,
        r.type AS ressource_type,
        r.code AS ressource_code,
        r.nom AS ressource_nom,
        dbo.fn_permission_nb_roles(p.id) AS nb_roles,
        dbo.fn_permission_nb_utilisateurs(p.id) AS nb_utilisateurs,
        dbo.fn_permission_est_utilisee(p.id) AS est_utilisee
    FROM permission p
    LEFT JOIN ressource r ON r.id = p.ressource_id
    WHERE p.id = @permission_id;
END;
GO

-- Procédure: sp_rechercher_permissions
-- Description: Recherche de permissions avec filtres optionnels
IF OBJECT_ID('sp_rechercher_permissions', 'P') IS NOT NULL
    DROP PROCEDURE sp_rechercher_permissions;
GO

CREATE PROCEDURE sp_rechercher_permissions
    @action VARCHAR(50) = NULL,
    @ressource_type VARCHAR(100) = NULL,
    @code_contient VARCHAR(150) = NULL,
    @uniquement_utilisees BIT = 0,
    @uniquement_orphelines BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.id AS permission_id,
        p.code,
        p.action,
        p.description,
        p.cree_le,
        r.id AS ressource_id,
        r.type AS ressource_type,
        r.code AS ressource_code,
        r.nom AS ressource_nom,
        dbo.fn_permission_nb_roles(p.id) AS nb_roles,
        dbo.fn_permission_nb_utilisateurs(p.id) AS nb_utilisateurs,
        dbo.fn_permission_est_utilisee(p.id) AS est_utilisee
    FROM permission p
    LEFT JOIN ressource r ON r.id = p.ressource_id
    WHERE (@action IS NULL OR p.action = @action)
      AND (@ressource_type IS NULL OR r.type = @ressource_type)
      AND (@code_contient IS NULL OR p.code LIKE '%' + @code_contient + '%')
      AND (@uniquement_utilisees = 0 OR dbo.fn_permission_est_utilisee(p.id) = 1)
      AND (@uniquement_orphelines = 0 OR dbo.fn_permission_est_utilisee(p.id) = 0)
    ORDER BY p.action, r.type, p.code;
END;
GO

-- Procédure: sp_assigner_permission_role
-- Description: Assigne une permission à un rôle
IF OBJECT_ID('sp_assigner_permission_role', 'P') IS NOT NULL
    DROP PROCEDURE sp_assigner_permission_role;
GO

CREATE PROCEDURE sp_assigner_permission_role
    @permission_id BIGINT,
    @role_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la permission existe
    IF NOT EXISTS (SELECT 1 FROM permission WHERE id = @permission_id)
    BEGIN
        RAISERROR('Permission introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier que le rôle existe
    IF NOT EXISTS (SELECT 1 FROM role WHERE id = @role_id)
    BEGIN
        RAISERROR('Rôle introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si l'association existe déjà
    IF EXISTS (SELECT 1 FROM role_permission WHERE role_id = @role_id AND permission_id = @permission_id)
    BEGIN
        RAISERROR('Cette permission est déjà assignée à ce rôle.', 16, 1);
        RETURN -1;
    END

    INSERT INTO role_permission (role_id, permission_id, accorde_le)
    VALUES (@role_id, @permission_id, SYSDATETIMEOFFSET());

    RETURN 0;
END;
GO

-- Procédure: sp_retirer_permission_role
-- Description: Retire une permission d'un rôle
IF OBJECT_ID('sp_retirer_permission_role', 'P') IS NOT NULL
    DROP PROCEDURE sp_retirer_permission_role;
GO

CREATE PROCEDURE sp_retirer_permission_role
    @permission_id BIGINT,
    @role_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que l'association existe
    IF NOT EXISTS (SELECT 1 FROM role_permission WHERE role_id = @role_id AND permission_id = @permission_id)
    BEGIN
        RAISERROR('Cette permission n''est pas assignée à ce rôle.', 16, 1);
        RETURN -1;
    END

    DELETE FROM role_permission
    WHERE role_id = @role_id AND permission_id = @permission_id;

    RETURN 0;
END;
GO

-- Procédure: sp_copier_permissions_vers_role
-- Description: Copie toutes les permissions d'un rôle source vers un rôle cible
IF OBJECT_ID('sp_copier_permissions_vers_role', 'P') IS NOT NULL
    DROP PROCEDURE sp_copier_permissions_vers_role;
GO

CREATE PROCEDURE sp_copier_permissions_vers_role
    @role_source_id BIGINT,
    @role_cible_id BIGINT,
    @remplacer BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que les rôles existent
    IF NOT EXISTS (SELECT 1 FROM role WHERE id = @role_source_id)
    BEGIN
        RAISERROR('Rôle source introuvable.', 16, 1);
        RETURN -1;
    END

    IF NOT EXISTS (SELECT 1 FROM role WHERE id = @role_cible_id)
    BEGIN
        RAISERROR('Rôle cible introuvable.', 16, 1);
        RETURN -1;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Si remplacer, supprimer d'abord les permissions existantes du rôle cible
        IF @remplacer = 1
        BEGIN
            DELETE FROM role_permission WHERE role_id = @role_cible_id;
        END

        -- Copier les permissions (ignorer celles qui existent déjà)
        INSERT INTO role_permission (role_id, permission_id, accorde_le)
        SELECT @role_cible_id, rp.permission_id, SYSDATETIMEOFFSET()
        FROM role_permission rp
        WHERE rp.role_id = @role_source_id
          AND NOT EXISTS (
              SELECT 1 FROM role_permission
              WHERE role_id = @role_cible_id AND permission_id = rp.permission_id
          );

        DECLARE @nb_copiees INT = @@ROWCOUNT;

        COMMIT;

        SELECT @nb_copiees AS PermissionsCopiees;
        RETURN 0;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

PRINT 'Tous les objets SQL pour la gestion des permissions ont été créés avec succès.';
GO
