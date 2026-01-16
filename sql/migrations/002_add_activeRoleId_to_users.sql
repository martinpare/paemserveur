-- Migration: Ajouter la colonne activeRoleId à la table users
-- Date: 2026-01-16

-- Vérifier si la colonne existe déjà avant de l'ajouter
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'users'
    AND COLUMN_NAME = 'activeRoleId'
)
BEGIN
    ALTER TABLE users
    ADD activeRoleId INT NULL;

    -- Ajouter la contrainte de clé étrangère
    ALTER TABLE users
    ADD CONSTRAINT FK_users_activeRoleId
    FOREIGN KEY (activeRoleId) REFERENCES roles(id);

    PRINT 'Colonne activeRoleId ajoutée à la table users';
END
ELSE
BEGIN
    PRINT 'La colonne activeRoleId existe déjà dans la table users';
END
GO
