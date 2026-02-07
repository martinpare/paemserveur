-- Script pour créer la table textAnnotations
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'textAnnotations')
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

    CREATE INDEX [IX_textAnnotations_learnerId] ON [textAnnotations] ([learnerId]);
    CREATE INDEX [IX_textAnnotations_contextId_learnerId] ON [textAnnotations] ([contextId], [learnerId]);

    PRINT 'Table textAnnotations créée avec succès.';
END
ELSE
BEGIN
    PRINT 'La table textAnnotations existe déjà.';
END
