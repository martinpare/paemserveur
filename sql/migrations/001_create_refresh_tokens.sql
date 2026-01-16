-- Migration: Create refresh_tokens table for JWT authentication
-- Date: 2026-01-16

-- Create refresh_tokens table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'refresh_tokens')
BEGIN
    CREATE TABLE refresh_tokens (
        id INT IDENTITY(1,1) PRIMARY KEY,
        token NVARCHAR(500) NOT NULL,
        user_id INT NOT NULL,
        expires_at DATETIME2 NOT NULL,
        created_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        revoked_at DATETIME2 NULL,
        is_revoked BIT NOT NULL DEFAULT 0,
        replaced_by_token NVARCHAR(500) NULL,

        CONSTRAINT FK_refresh_tokens_users FOREIGN KEY (user_id)
            REFERENCES users(id) ON DELETE CASCADE
    );

    -- Index for faster token lookups
    CREATE INDEX IX_refresh_tokens_token ON refresh_tokens(token);

    -- Index for user lookups (to revoke all user tokens)
    CREATE INDEX IX_refresh_tokens_user_id ON refresh_tokens(user_id);

    -- Index for cleanup of expired tokens
    CREATE INDEX IX_refresh_tokens_expires_at ON refresh_tokens(expires_at);

    PRINT 'Table refresh_tokens created successfully';
END
ELSE
BEGIN
    PRINT 'Table refresh_tokens already exists';
END
GO
