# Instructions pour Claude Code

## Base de données

Ce projet utilise une base de données SQL Server accessible via le serveur MCP `mezur3`.

### Modifications de schéma

Lorsque l'utilisateur demande de modifier une table (ajouter une colonne, modifier un type, etc.) :

1. **Modifier le modèle C#** dans `models/entities/` avec les attributs appropriés :
   - `[Column("nom_colonne")]` pour le nom de la colonne
   - `[StringLength(n)]` pour NVARCHAR(n)
   - `[Required]` si NOT NULL

2. **Exécuter le SQL directement** via l'outil `mcp__mezur3__execute_sql` :
   ```sql
   ALTER TABLE dbo.nom_table ADD nom_colonne TYPE NULL/NOT NULL;
   ```

3. **Vérifier** que la modification a été appliquée avec une requête SELECT.

### Conventions de nommage

**IMPORTANT : Toujours respecter ces conventions pour toute nouvelle table ou colonne.**

- **Tables** : camelCase pluriel (ex: `organisations`, `learningCenters`, `discussionThreads`)
  - Utiliser le pluriel pour les tables
  - Première lettre en minuscule, puis camelCase
- **Colonnes** : camelCase (ex: `nameFr`, `isActive`, `createdAt`)
  - Première lettre en minuscule, puis camelCase
- **Schéma** : `dbo`

**Exemples :**
```sql
-- ✅ Correct
CREATE TABLE dbo.discussionThreads (
    id INT,
    entityType NVARCHAR(100),
    createdAt DATETIME2
);

-- ❌ Incorrect (snake_case)
CREATE TABLE dbo.discussion_threads (
    id INT,
    entity_type NVARCHAR(100),
    created_at DATETIME2
);
```

### Utilisateur SQL

L'utilisateur `claude_user` a les droits db_owner sur la base `mezur3`.
