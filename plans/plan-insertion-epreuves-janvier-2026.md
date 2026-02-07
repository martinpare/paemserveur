# Plan d'insertion des épreuves ministérielles - Session janvier 2026

## Références de données existantes

### Session ministérielle
- `examPeriodId = 1` (Session de janvier 2026)

### Langues
| id | Code | Nom |
|----|------|-----|
| 348 | fr | Français |
| 349 | en | Anglais |

### Compétences évaluées
| id | Code | Nom |
|----|------|-----|
| 376 | READING | Lecture |
| 377 | READING_LITERARY | Lecture - Texte littéraire |
| 378 | READING_INFO | Lecture - Texte courant |
| 379 | WRITING | Écriture |
| 380 | WRITTEN_PROD | Production écrite |
| 381 | READING_COMP | Compréhension écrite |
| 382 | ORAL_INTERACTION | Interaction orale |
| 383 | LISTENING | Compréhension orale |
| 384 | REASONING | Raisonnement |
| 385 | THEORY | Théorie |

### Structures pédagogiques (niveaux)
| id | Nom |
|----|-----|
| 10 | Fin du 3e cycle (6e année) - Primaire |
| 16 | 4e secondaire |
| 17 | 5e secondaire |

---

## Structure des données à insérer

### Principe de modélisation

1. **Épreuve principale** (`parentExamId = NULL`)
   - Représente l'épreuve complète (ex: "Français, langue d'enseignement, 6e année - Lecture")

2. **Parties d'épreuve** (`parentExamId = id_parent`)
   - Sous-épreuves avec dates différentes (ex: "Texte littéraire" le 13, "Texte courant" le 14)

3. **Codes externes multiples**
   - Quand un code couvre plusieurs variantes (136-540 et 136-550), créer une épreuve par code

---

## SECTION 1 : Enseignement primaire (Langue FR)

### 1.1 Français 6e année - Lecture (014-630)
**Épreuve principale:**
```
nameFr: "Français, langue d'enseignement, 6e année du primaire - Lecture"
nameEn: "French, Language of Instruction, Grade 6 - Reading"
externalCode: "014-630"
examPeriodId: 1
pedagogicalStructureId: 10 (6e année)
languageId: 348 (fr)
competencyId: 376 (Lecture)
parentExamId: NULL
```

**Partie 1 - Texte littéraire (13 janvier):**
```
nameFr: "Français, langue d'enseignement, 6e année - Lecture"
externalCode: "014-630"
parentExamId: [id épreuve principale]
partNumber: 1
partNameFr: "Texte littéraire"
partNameEn: "Literary Text"
competencyId: 377 (Lecture - Texte littéraire)
```

**Partie 2 - Texte courant (14 janvier):**
```
nameFr: "Français, langue d'enseignement, 6e année - Lecture"
externalCode: "014-630"
parentExamId: [id épreuve principale]
partNumber: 2
partNameFr: "Texte courant"
partNameEn: "Informational Text"
competencyId: 378 (Lecture - Texte courant)
```

### 1.2 Français 6e année - Écriture (014-620)
**Épreuve unique (15-16 janvier):**
```
nameFr: "Français, langue d'enseignement, 6e année du primaire - Écriture"
nameEn: "French, Language of Instruction, Grade 6 - Writing"
externalCode: "014-620"
examPeriodId: 1
pedagogicalStructureId: 10
languageId: 348 (fr)
competencyId: 379 (Écriture)
parentExamId: NULL
```

### 1.3 Mathématique 6e année (022-610)
**Épreuve unique (20-21-22 janvier):**
```
nameFr: "Mathématique, 6e année du primaire"
nameEn: "Mathematics, Grade 6"
externalCode: "022-610"
examPeriodId: 1
pedagogicalStructureId: 10
languageId: 348 (fr)
competencyId: 384 (Raisonnement)
parentExamId: NULL
```

---

## SECTION 2 : Enseignement secondaire - Français langue d'enseignement

### 2.1 Français 5e sec - Écriture (132-520)
**Épreuve principale:**
```
nameFr: "Français, langue d'enseignement, 5e secondaire - Écriture"
nameEn: "French, Language of Instruction, Secondary 5 - Writing"
externalCode: "132-520"
examPeriodId: 1
pedagogicalStructureId: 17 (5e sec)
languageId: 348 (fr)
competencyId: 379 (Écriture)
parentExamId: NULL
```

**Partie 1 - Remise dossier préparatoire (27 novembre):**
```
partNumber: 1
partNameFr: "Remise du dossier préparatoire"
partNameEn: "Preparatory File Submission"
```

**Partie 2 - Écriture (4 décembre):**
```
partNumber: 2
partNameFr: "Épreuve d'écriture"
partNameEn: "Writing Test"
```

---

## SECTION 3 : Anglais langue seconde - Programme de base

### 3.1 Anglais 5e sec - Interaction orale (134-510)
```
nameFr: "Anglais, langue seconde, 5e secondaire - Programme de base"
nameEn: "English as a Second Language, Secondary 5 - Core Program"
externalCode: "134-510"
pedagogicalStructureId: 17
languageId: 348 (fr) -- passation en français
competencyId: 382 (Interaction orale)
```

### 3.2 Anglais 5e sec - Production écrite (134-530)
```
nameFr: "Anglais, langue seconde, 5e secondaire - Production écrite"
nameEn: "English as a Second Language, Secondary 5 - Written Production"
externalCode: "134-530"
competencyId: 380 (Production écrite)
```

---

## SECTION 4 : Anglais langue seconde - Programme enrichi

### 4.1 Programme enrichi - 136-540
```
nameFr: "Anglais, langue seconde, 5e secondaire - Programme enrichi"
nameEn: "English as a Second Language, Secondary 5 - Enriched Program"
externalCode: "136-540"
```

### 4.2 Programme enrichi - 136-550
```
externalCode: "136-550"
```

**Parties communes aux deux codes:**
1. Remise du cahier de préparation (6-8 janvier)
2. Écoute et discussion (9-15 janvier)
3. Production écrite (16 janvier)

---

## SECTION 5 : Autres matières secondaire (FR)

### 5.1 Histoire du Québec et du Canada - 4e sec (085-404)
```
nameFr: "Histoire du Québec et du Canada, 4e secondaire"
nameEn: "History of Quebec and Canada, Secondary 4"
externalCode: "085-404"
pedagogicalStructureId: 16 (4e sec)
languageId: 348 (fr)
competencyId: NULL (pas de compétence spécifique)
```

### 5.2 Science et technologie - 4e sec (055-410)
```
nameFr: "Science et technologie, 4e secondaire - Volet théorie"
nameEn: "Science and Technology, Secondary 4 - Theory Component"
externalCode: "055-410"
competencyId: 385 (Théorie)
```

### 5.3 Applications technologiques et scientifiques (057-410)
```
nameFr: "Applications technologiques et scientifiques, 4e secondaire - Théorie"
nameEn: "Applied Science and Technology, Secondary 4 - Theory"
externalCode: "057-410"
competencyId: 385 (Théorie)
```

### 5.4 Mathématique 4e sec - CST (063-420)
```
nameFr: "Mathématique, 4e secondaire - Culture, société et technique"
nameEn: "Mathematics, Secondary 4 - Cultural, Social and Technical"
externalCode: "063-420"
competencyId: 384 (Raisonnement)
```

### 5.5 Mathématique 4e sec - TS (064-420)
```
nameFr: "Mathématique, 4e secondaire - Technico-sciences"
nameEn: "Mathematics, Secondary 4 - Technical and Scientific"
externalCode: "064-420"
```

### 5.6 Mathématique 4e sec - SN (065-420)
```
nameFr: "Mathématique, 4e secondaire - Sciences naturelles"
nameEn: "Mathematics, Secondary 4 - Science"
externalCode: "065-420"
```

---

## SECTION 6 : Épreuves en langue anglaise

### 6.1 Français langue seconde - Programme de base
**634-510, 634-520, 634-530** (3 codes distincts)
```
languageId: 349 (en) -- épreuves passées en anglais
```

### 6.2 Français langue seconde - Programme enrichi
**635-520, 635-530** (2 codes distincts)

### 6.3 English Language Arts - 5e sec
**612-520** (Reading)
```
nameFr: "English Language Arts, 5e secondaire - Lecture"
nameEn: "English Language Arts, Secondary 5 - Reading"
externalCode: "612-520"
languageId: 349 (en)
competencyId: 376 (Lecture)
```

**612-530** (Production)
- Partie 1: Production Preparation
- Partie 2: Production Writing

### 6.4 History of Quebec and Canada - 4e sec (585-404)
```
nameFr: "Histoire du Québec et du Canada, 4e secondaire (anglais)"
nameEn: "History of Quebec and Canada, Secondary 4"
externalCode: "585-404"
languageId: 349 (en)
```

### 6.5 Science - 4e sec (anglais)
**555-410, 557-410**

### 6.6 Mathematics - 4e sec (anglais)
**563-420, 564-420, 565-420**

---

## Résumé du nombre d'épreuves à créer

| Section | Épreuves principales | Parties |
|---------|---------------------|---------|
| Primaire FR | 3 | 2 |
| Secondaire FR - Français | 1 | 2 |
| Secondaire FR - Anglais base | 2 | - |
| Secondaire FR - Anglais enrichi | 2 | 3 chaque |
| Secondaire FR - Autres | 6 | - |
| Secondaire EN - FSL base | 3 | 2 |
| Secondaire EN - FSL enrichi | 2 | 2 |
| Secondaire EN - ELA | 1 | 2 |
| Secondaire EN - Autres | 6 | - |
| **TOTAL** | **~26** | **~13** |

---

## Script SQL d'insertion

Le script sera généré en plusieurs blocs pour faciliter le suivi et la validation.
