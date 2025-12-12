-- ============================================
-- Script de création des objets SQL pour la gestion des Ressources
-- Base de données: paem
-- Date: 2024
-- ============================================

USE paem;
GO

-- ============================================
-- INDEX
-- ============================================

-- Index pour recherche par type
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ressource_type' AND object_id = OBJECT_ID('ressource'))
BEGIN
    CREATE INDEX IX_ressource_type ON ressource(type);
END
GO

-- Index unique pour éviter les doublons type+code
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ressource_type_code' AND object_id = OBJECT_ID('ressource'))
BEGIN
    CREATE UNIQUE INDEX IX_ressource_type_code ON ressource(type, code);
END
GO

-- Index sur permission.ressource_id pour les jointures
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_permission_ressource_id' AND object_id = OBJECT_ID('permission'))
BEGIN
    CREATE INDEX IX_permission_ressource_id ON permission(ressource_id);
END
GO

-- ============================================
-- VUES
-- ============================================

-- Vue: v_ressource_complete
-- Description: Vue consolidant les ressources avec leurs statistiques d'utilisation
IF OBJECT_ID('v_ressource_complete', 'V') IS NOT NULL
    DROP VIEW v_ressource_complete;
GO

CREATE VIEW v_ressource_complete AS
SELECT
    r.id AS ressource_id,
    r.type,
    r.code,
    r.nom,
    r.description,
    r.cree_le,
    COUNT(DISTINCT p.id) AS nb_permissions,
    COUNT(DISTINCT rp.role_id) AS nb_roles_utilisant,
    COUNT(DISTINCT ur.utilisateur_id) AS nb_utilisateurs_ayant_acces
FROM ressource r
LEFT JOIN permission p ON p.ressource_id = r.id
LEFT JOIN role_permission rp ON rp.permission_id = p.id
LEFT JOIN utilisateur_role ur ON ur.role_id = rp.role_id
GROUP BY r.id, r.type, r.code, r.nom, r.description, r.cree_le;
GO

-- Vue: v_ressource_par_type
-- Description: Regroupement des ressources par type avec comptage
IF OBJECT_ID('v_ressource_par_type', 'V') IS NOT NULL
    DROP VIEW v_ressource_par_type;
GO

CREATE VIEW v_ressource_par_type AS
SELECT
    type,
    COUNT(*) AS nb_ressources,
    STRING_AGG(code, ', ') WITHIN GROUP (ORDER BY code) AS codes
FROM ressource
GROUP BY type;
GO

-- Vue: v_ressource_utilisateur
-- Description: Ressources accessibles par utilisateur avec actions
IF OBJECT_ID('v_ressource_utilisateur', 'V') IS NOT NULL
    DROP VIEW v_ressource_utilisateur;
GO

CREATE VIEW v_ressource_utilisateur AS
SELECT DISTINCT
    ur.utilisateur_id,
    r.id AS ressource_id,
    r.type AS ressource_type,
    r.code AS ressource_code,
    r.nom AS ressource_nom,
    p.action,
    p.code AS permission_code
FROM ressource r
INNER JOIN permission p ON p.ressource_id = r.id
INNER JOIN role_permission rp ON rp.permission_id = p.id
INNER JOIN utilisateur_role ur ON ur.role_id = rp.role_id;
GO

-- ============================================
-- FONCTIONS
-- ============================================

-- Fonction: fn_ressource_est_utilisee
-- Description: Vérifie si une ressource est utilisée dans le système
IF OBJECT_ID('fn_ressource_est_utilisee', 'FN') IS NOT NULL
    DROP FUNCTION fn_ressource_est_utilisee;
GO

CREATE FUNCTION fn_ressource_est_utilisee(@ressource_id BIGINT)
RETURNS BIT
AS
BEGIN
    DECLARE @est_utilisee BIT = 0;

    IF EXISTS (
        SELECT 1 FROM permission p
        INNER JOIN role_permission rp ON rp.permission_id = p.id
        WHERE p.ressource_id = @ressource_id
    )
        SET @est_utilisee = 1;

    RETURN @est_utilisee;
END;
GO

-- Fonction: fn_ressource_nb_permissions
-- Description: Retourne le nombre de permissions liées à une ressource
IF OBJECT_ID('fn_ressource_nb_permissions', 'FN') IS NOT NULL
    DROP FUNCTION fn_ressource_nb_permissions;
GO

CREATE FUNCTION fn_ressource_nb_permissions(@ressource_id BIGINT)
RETURNS INT
AS
BEGIN
    DECLARE @nb INT;
    SELECT @nb = COUNT(*) FROM permission WHERE ressource_id = @ressource_id;
    RETURN ISNULL(@nb, 0);
END;
GO

-- Fonction TVF: fn_obtenir_ressources_utilisateur
-- Description: Retourne toutes les ressources accessibles par un utilisateur
IF OBJECT_ID('fn_obtenir_ressources_utilisateur', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_ressources_utilisateur;
GO

CREATE FUNCTION fn_obtenir_ressources_utilisateur(@utilisateur_id BIGINT)
RETURNS TABLE
AS
RETURN
(
    SELECT DISTINCT
        r.id AS ressource_id,
        r.type,
        r.code,
        r.nom,
        r.description,
        p.action
    FROM ressource r
    INNER JOIN permission p ON p.ressource_id = r.id
    INNER JOIN role_permission rp ON rp.permission_id = p.id
    INNER JOIN utilisateur_role ur ON ur.role_id = rp.role_id
    WHERE ur.utilisateur_id = @utilisateur_id
);
GO

-- Fonction TVF: fn_obtenir_ressources_par_type
-- Description: Retourne les ressources filtrées par type
IF OBJECT_ID('fn_obtenir_ressources_par_type', 'IF') IS NOT NULL
    DROP FUNCTION fn_obtenir_ressources_par_type;
GO

CREATE FUNCTION fn_obtenir_ressources_par_type(@type VARCHAR(100))
RETURNS TABLE
AS
RETURN
(
    SELECT
        r.id AS ressource_id,
        r.type,
        r.code,
        r.nom,
        r.description,
        r.cree_le,
        COUNT(p.id) AS nb_permissions
    FROM ressource r
    LEFT JOIN permission p ON p.ressource_id = r.id
    WHERE r.type = @type
    GROUP BY r.id, r.type, r.code, r.nom, r.description, r.cree_le
);
GO

-- ============================================
-- PROCEDURES STOCKEES
-- ============================================

-- Procédure: sp_creer_ressource
-- Description: Création d'une ressource avec validation et retour de l'ID
IF OBJECT_ID('sp_creer_ressource', 'P') IS NOT NULL
    DROP PROCEDURE sp_creer_ressource;
GO

CREATE PROCEDURE sp_creer_ressource
    @type VARCHAR(100),
    @code VARCHAR(150),
    @nom VARCHAR(255) = NULL,
    @description NVARCHAR(MAX) = NULL,
    @id BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier si le code existe déjà pour ce type
    IF EXISTS (SELECT 1 FROM ressource WHERE type = @type AND code = @code)
    BEGIN
        RAISERROR('Une ressource avec ce type et code existe déjà.', 16, 1);
        SET @id = -1;
        RETURN -1;
    END

    INSERT INTO ressource (type, code, nom, description, cree_le)
    VALUES (@type, @code, @nom, @description, SYSDATETIMEOFFSET());

    SET @id = SCOPE_IDENTITY();
    RETURN 0;
END;
GO

-- Procédure: sp_modifier_ressource
-- Description: Modification d'une ressource existante
IF OBJECT_ID('sp_modifier_ressource', 'P') IS NOT NULL
    DROP PROCEDURE sp_modifier_ressource;
GO

CREATE PROCEDURE sp_modifier_ressource
    @id BIGINT,
    @type VARCHAR(100),
    @code VARCHAR(150),
    @nom VARCHAR(255) = NULL,
    @description NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la ressource existe
    IF NOT EXISTS (SELECT 1 FROM ressource WHERE id = @id)
    BEGIN
        RAISERROR('Ressource introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si le nouveau code existe déjà pour un autre enregistrement
    IF EXISTS (SELECT 1 FROM ressource WHERE type = @type AND code = @code AND id <> @id)
    BEGIN
        RAISERROR('Une autre ressource avec ce type et code existe déjà.', 16, 1);
        RETURN -1;
    END

    UPDATE ressource
    SET type = @type,
        code = @code,
        nom = @nom,
        description = @description
    WHERE id = @id;

    RETURN 0;
END;
GO

-- Procédure: sp_supprimer_ressource
-- Description: Suppression sécurisée avec vérification des dépendances
IF OBJECT_ID('sp_supprimer_ressource', 'P') IS NOT NULL
    DROP PROCEDURE sp_supprimer_ressource;
GO

CREATE PROCEDURE sp_supprimer_ressource
    @ressource_id BIGINT,
    @force BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Vérifier que la ressource existe
    IF NOT EXISTS (SELECT 1 FROM ressource WHERE id = @ressource_id)
    BEGIN
        RAISERROR('Ressource introuvable.', 16, 1);
        RETURN -1;
    END

    -- Vérifier si des permissions sont liées
    DECLARE @nb_permissions INT;
    SELECT @nb_permissions = COUNT(*) FROM permission WHERE ressource_id = @ressource_id;

    IF @nb_permissions > 0 AND @force = 0
    BEGIN
        DECLARE @msg NVARCHAR(200);
        SET @msg = 'Cette ressource a ' + CAST(@nb_permissions AS NVARCHAR(10)) + ' permission(s) liée(s). Utilisez @force=1 pour forcer la suppression.';
        RAISERROR(@msg, 16, 1);
        RETURN -1;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Supprimer les role_permission liées aux permissions de cette ressource
        DELETE rp FROM role_permission rp
        INNER JOIN permission p ON rp.permission_id = p.id
        WHERE p.ressource_id = @ressource_id;

        -- Supprimer les permissions liées
        DELETE FROM permission WHERE ressource_id = @ressource_id;

        -- Supprimer la ressource
        DELETE FROM ressource WHERE id = @ressource_id;

        COMMIT;
        RETURN 0;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        THROW;
    END CATCH
END;
GO

-- Procédure: sp_dupliquer_ressource
-- Description: Duplique une ressource avec toutes ses permissions
IF OBJECT_ID('sp_dupliquer_ressource', 'P') IS NOT NULL
    DROP PROCEDURE sp_dupliquer_ressource;
GO

CREATE PROCEDURE sp_dupliquer_ressource
    @ressource_id BIGINT,
    @nouveau_code VARCHAR(150),
    @nouveau_nom VARCHAR(255) = NULL,
    @nouvelle_ressource_id BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @type VARCHAR(100);
    SELECT @type = type FROM ressource WHERE id = @ressource_id;

    IF @type IS NULL
    BEGIN
        RAISERROR('Ressource source introuvable.', 16, 1);
        SET @nouvelle_ressource_id = -1;
        RETURN -1;
    END

    -- Vérifier si le nouveau code existe déjà
    IF EXISTS (SELECT 1 FROM ressource WHERE type = @type AND code = @nouveau_code)
    BEGIN
        RAISERROR('Une ressource avec ce type et code existe déjà.', 16, 1);
        SET @nouvelle_ressource_id = -1;
        RETURN -1;
    END

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Créer la nouvelle ressource
        INSERT INTO ressource (type, code, nom, description, cree_le)
        SELECT type, @nouveau_code, COALESCE(@nouveau_nom, nom), description, SYSDATETIMEOFFSET()
        FROM ressource WHERE id = @ressource_id;

        SET @nouvelle_ressource_id = SCOPE_IDENTITY();

        -- Dupliquer les permissions avec un nouveau code
        INSERT INTO permission (code, action, ressource_id, description, cree_le)
        SELECT
            CONCAT(code, '_', @nouveau_code),
            action,
            @nouvelle_ressource_id,
            description,
            SYSDATETIMEOFFSET()
        FROM permission WHERE ressource_id = @ressource_id;

        COMMIT;
        RETURN 0;
    END TRY
    BEGIN CATCH
        ROLLBACK;
        SET @nouvelle_ressource_id = -1;
        THROW;
    END CATCH
END;
GO

-- Procédure: sp_obtenir_ressource_complete
-- Description: Retourne une ressource avec toutes ses statistiques
IF OBJECT_ID('sp_obtenir_ressource_complete', 'P') IS NOT NULL
    DROP PROCEDURE sp_obtenir_ressource_complete;
GO

CREATE PROCEDURE sp_obtenir_ressource_complete
    @ressource_id BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.id AS ressource_id,
        r.type,
        r.code,
        r.nom,
        r.description,
        r.cree_le,
        COUNT(DISTINCT p.id) AS nb_permissions,
        COUNT(DISTINCT rp.role_id) AS nb_roles_utilisant,
        COUNT(DISTINCT ur.utilisateur_id) AS nb_utilisateurs_ayant_acces,
        dbo.fn_ressource_est_utilisee(r.id) AS est_utilisee
    FROM ressource r
    LEFT JOIN permission p ON p.ressource_id = r.id
    LEFT JOIN role_permission rp ON rp.permission_id = p.id
    LEFT JOIN utilisateur_role ur ON ur.role_id = rp.role_id
    WHERE r.id = @ressource_id
    GROUP BY r.id, r.type, r.code, r.nom, r.description, r.cree_le;
END;
GO

-- Procédure: sp_rechercher_ressources
-- Description: Recherche de ressources avec filtres optionnels
IF OBJECT_ID('sp_rechercher_ressources', 'P') IS NOT NULL
    DROP PROCEDURE sp_rechercher_ressources;
GO

CREATE PROCEDURE sp_rechercher_ressources
    @type VARCHAR(100) = NULL,
    @code_contient VARCHAR(150) = NULL,
    @nom_contient VARCHAR(255) = NULL,
    @uniquement_utilisees BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.id AS ressource_id,
        r.type,
        r.code,
        r.nom,
        r.description,
        r.cree_le,
        COUNT(DISTINCT p.id) AS nb_permissions,
        COUNT(DISTINCT rp.role_id) AS nb_roles_utilisant,
        dbo.fn_ressource_est_utilisee(r.id) AS est_utilisee
    FROM ressource r
    LEFT JOIN permission p ON p.ressource_id = r.id
    LEFT JOIN role_permission rp ON rp.permission_id = p.id
    WHERE (@type IS NULL OR r.type = @type)
      AND (@code_contient IS NULL OR r.code LIKE '%' + @code_contient + '%')
      AND (@nom_contient IS NULL OR r.nom LIKE '%' + @nom_contient + '%')
      AND (@uniquement_utilisees = 0 OR dbo.fn_ressource_est_utilisee(r.id) = 1)
    GROUP BY r.id, r.type, r.code, r.nom, r.description, r.cree_le
    ORDER BY r.type, r.code;
END;
GO

PRINT 'Tous les objets SQL pour la gestion des ressources ont été créés avec succès.';
GO
