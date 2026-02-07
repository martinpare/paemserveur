# Plan d'évolution du Hub SignalR pour l'administration d'épreuves

**Date:** 2026-02-03
**Version:** 1.0
**Auteur:** Claude Code

---

## Sommaire

1. [État actuel du système](#état-actuel-du-système)
2. [Problème architectural majeur](#problème-architectural-majeur)
3. [Architecture cible recommandée](#architecture-cible-recommandée)
4. [Plan d'évolution en 4 phases](#plan-dévolution-en-4-phases)
5. [Ce qui manque actuellement](#ce-qui-manque-actuellement)
6. [Recommandation de priorisation](#recommandation-de-priorisation)

---

## État actuel du système

### Côté Serveur (Hubs/NotificationHub.cs)

Le hub actuel est **générique** - il gère des connexions utilisateurs et des groupes, mais n'a **aucune connaissance métier** de l'administration d'examens.

**Fonctionnalités existantes:**
- Gestion des connexions utilisateurs (Register, OnConnected, OnDisconnected)
- Groupes SignalR (JoinGroup, LeaveGroup)
- Envoi de notifications et alertes
- Chat de groupe
- Suivi des utilisateurs connectés

### Côté Client (paem/src/composables/useGroupeExamen.js)

Le composable est **très avancé** et gère toute la logique métier côté client :

**Types de messages (20+):**
- Connexion/Déconnexion (ETUDIANT_CONNECTE, ETUDIANT_DECONNECTE, SURVEILLANT_CONNECTE)
- Vérification (VERIFIER_ETUDIANT, ETUDIANT_DEJA_CONNECTE)
- Commandes (DEMARRER_EXAMEN, PAUSE_EXAMEN, REPRENDRE_EXAMEN, TERMINER_EXAMEN)
- Demandes (DEMANDER_PAUSE, SIGNALER_PROBLEME)
- Temps (AJOUTER_TEMPS_ETUDIANT, AJOUTER_TEMPS_GROUPE, TEMPS_RESTANT_ETUDIANT, TEMPS_ECOULE)
- Chat (MESSAGE_CHAT, DESACTIVER_CHAT, ACTIVER_CHAT)
- Notifications (MESSAGE_GROUPE, ALERTE)

**Statuts étudiants (9 états):**
- EN_LIGNE, EN_EXAMEN, PAUSE, PAUSE_DEMANDEE, REPRISE
- TERMINE, DECONNECTE, SORTIE_EXAMEN, SORTIE_PLEIN_ECRAN

### Composants Vue existants

1. **PanneauSurveillance.vue** (~1150 lignes)
   - Interface complète de surveillance
   - Grille des étudiants avec statuts
   - Commandes globales et individuelles
   - Gestion du temps
   - Journal des événements

2. **ChatSurveillance.vue** (~450 lignes)
   - Chat bidirectionnel surveillant/étudiants
   - Gestion des conversations multiples
   - Indicateurs de messages non lus

3. **SurveillancePage.vue**
   - Page conteneur avec fil d'Ariane

---

## Problème architectural majeur

```
┌─────────────────────────────────────────────────────────────────┐
│                    ARCHITECTURE ACTUELLE                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  CLIENT A (Surveillant)         CLIENT B (Étudiant)            │
│  ┌─────────────────────┐        ┌─────────────────────┐        │
│  │ useGroupeExamen.js  │        │ useGroupeExamen.js  │        │
│  │ (TOUTE LA LOGIQUE)  │◄──────►│ (TOUTE LA LOGIQUE)  │        │
│  └─────────────────────┘        └─────────────────────┘        │
│           │                              │                      │
│           ▼                              ▼                      │
│  ┌─────────────────────────────────────────────────────┐       │
│  │              NotificationHub.cs                      │       │
│  │         (Simple relais de messages)                  │       │
│  │         AUCUNE PERSISTANCE                           │       │
│  │         AUCUNE VALIDATION                            │       │
│  └─────────────────────────────────────────────────────┘       │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### Conséquences

| Problème | Impact |
|----------|--------|
| **Aucune persistance** | Si le surveillant se déconnecte, tout l'état est perdu |
| **Pas de validation serveur** | Un client malveillant peut envoyer n'importe quoi |
| **Pas de traçabilité** | Aucun audit des actions pour contestation ou analyse |
| **Pas de récupération** | Si un étudiant se reconnecte après une panne, il perd son contexte |
| **Pas de cohérence garantie** | L'état peut diverger entre clients |

---

## Architecture cible recommandée

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        ARCHITECTURE CIBLE                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  CLIENT (Surveillant)              CLIENT (Étudiant)                        │
│  ┌───────────────────┐             ┌───────────────────┐                    │
│  │ PanneauSurveillance│            │  InterfaceExamen  │                    │
│  └─────────┬─────────┘             └─────────┬─────────┘                    │
│            │                                  │                              │
│            ▼                                  ▼                              │
│  ┌─────────────────────────────────────────────────────────────┐           │
│  │                    ExamHub.cs (NOUVEAU)                      │           │
│  │  ┌─────────────────────────────────────────────────────┐    │           │
│  │  │  État en mémoire (ConcurrentDictionary)             │    │           │
│  │  │  - Sessions actives par code de groupe              │    │           │
│  │  │  - Participants par session                         │    │           │
│  │  │  - Statuts, temps, incidents                        │    │           │
│  │  └─────────────────────────────────────────────────────┘    │           │
│  │                           │                                  │           │
│  │                           ▼                                  │           │
│  │  ┌─────────────────────────────────────────────────────┐    │           │
│  │  │  Validation métier                                   │    │           │
│  │  │  - Droits (surveillant vs étudiant)                  │    │           │
│  │  │  - Transitions de statut valides                     │    │           │
│  │  │  - Cohérence temporelle                              │    │           │
│  │  └─────────────────────────────────────────────────────┘    │           │
│  └─────────────────────────────────────────────────────────────┘           │
│                           │                                                 │
│                           ▼                                                 │
│  ┌─────────────────────────────────────────────────────────────┐           │
│  │                    Base de données                           │           │
│  │  - examSessionLogs (audit)                                   │           │
│  │  - examParticipantStates (persistance)                       │           │
│  │  - examIncidents (traçabilité)                               │           │
│  └─────────────────────────────────────────────────────────────┘           │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## Plan d'évolution en 4 phases

### Phase 1 : Tables de persistance (Fondation)

**Durée estimée:** À déterminer par l'équipe

**Nouvelles tables requises:**

#### 1.1 examSessions
```sql
CREATE TABLE dbo.examSessions (
    id INT IDENTITY(1,1) PRIMARY KEY,
    sessionId INT NOT NULL,                    -- FK vers sessions
    groupCode NVARCHAR(20) NOT NULL UNIQUE,    -- Code de groupe (ex: ABC123)
    supervisorUserId INT NOT NULL,             -- FK vers users (surveillant)
    status NVARCHAR(50) NOT NULL,              -- pending, active, paused, ended
    startedAt DATETIME2 NULL,
    endedAt DATETIME2 NULL,
    createdAt DATETIME2 NOT NULL,
    updatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_examSessions_sessions FOREIGN KEY (sessionId) REFERENCES dbo.sessions(id),
    CONSTRAINT FK_examSessions_users FOREIGN KEY (supervisorUserId) REFERENCES dbo.users(id)
);
```

#### 1.2 examParticipants
```sql
CREATE TABLE dbo.examParticipants (
    id INT IDENTITY(1,1) PRIMARY KEY,
    examSessionId INT NOT NULL,                -- FK vers examSessions
    learnerId INT NOT NULL,                    -- FK vers learners
    convocationId INT NULL,                    -- FK vers convocations
    status NVARCHAR(50) NOT NULL,              -- Statut actuel
    connectionId NVARCHAR(100) NULL,           -- ID de connexion SignalR
    remainingTimeSeconds INT NULL,             -- Temps restant
    additionalTimeMinutes INT DEFAULT 0,       -- Temps supplémentaire accordé
    startedAt DATETIME2 NULL,
    submittedAt DATETIME2 NULL,
    createdAt DATETIME2 NOT NULL,
    updatedAt DATETIME2 NOT NULL,
    CONSTRAINT FK_examParticipants_examSessions FOREIGN KEY (examSessionId) REFERENCES dbo.examSessions(id),
    CONSTRAINT FK_examParticipants_learners FOREIGN KEY (learnerId) REFERENCES dbo.learners(id),
    CONSTRAINT FK_examParticipants_convocations FOREIGN KEY (convocationId) REFERENCES dbo.convocations(id)
);
```

#### 1.3 examLogs
```sql
CREATE TABLE dbo.examLogs (
    id INT IDENTITY(1,1) PRIMARY KEY,
    examSessionId INT NOT NULL,
    participantId INT NULL,                    -- NULL si action globale
    actorType NVARCHAR(20) NOT NULL,           -- supervisor, learner, system
    actorId INT NULL,                          -- userId ou learnerId
    action NVARCHAR(100) NOT NULL,             -- Type d'action
    details NVARCHAR(MAX) NULL,                -- JSON avec détails
    createdAt DATETIME2 NOT NULL,
    CONSTRAINT FK_examLogs_examSessions FOREIGN KEY (examSessionId) REFERENCES dbo.examSessions(id)
);
```

#### 1.4 examIncidents
```sql
CREATE TABLE dbo.examIncidents (
    id INT IDENTITY(1,1) PRIMARY KEY,
    examParticipantId INT NOT NULL,
    incidentType NVARCHAR(50) NOT NULL,        -- fullscreen_exit, tab_change, disconnect, etc.
    description NVARCHAR(500) NULL,
    previousStatus NVARCHAR(50) NULL,
    createdAt DATETIME2 NOT NULL,
    CONSTRAINT FK_examIncidents_examParticipants FOREIGN KEY (examParticipantId) REFERENCES dbo.examParticipants(id)
);
```

#### 1.5 examMessages
```sql
CREATE TABLE dbo.examMessages (
    id INT IDENTITY(1,1) PRIMARY KEY,
    examSessionId INT NOT NULL,
    senderType NVARCHAR(20) NOT NULL,          -- supervisor, learner
    senderId INT NOT NULL,
    recipientType NVARCHAR(20) NULL,           -- NULL = broadcast, supervisor, learner
    recipientId INT NULL,
    messageText NVARCHAR(MAX) NOT NULL,
    isRead BIT DEFAULT 0,
    createdAt DATETIME2 NOT NULL,
    CONSTRAINT FK_examMessages_examSessions FOREIGN KEY (examSessionId) REFERENCES dbo.examSessions(id)
);
```

---

### Phase 2 : Hub spécialisé (ExamHub.cs)

**Durée estimée:** À déterminer par l'équipe

#### 2.1 Structure du hub

```csharp
namespace serveur.Hubs
{
    public class ExamHub : Hub
    {
        // État en mémoire
        private static readonly ConcurrentDictionary<string, ExamSessionState> _activeSessions;

        // Injection de dépendances
        private readonly AppDbContext _context;
        private readonly ILogger<ExamHub> _logger;

        // Méthodes...
    }
}
```

#### 2.2 Méthodes surveillant

| Méthode | Paramètres | Description |
|---------|------------|-------------|
| `CreateExamSession` | sessionId | Crée une session et retourne le code groupe |
| `StartExam` | codeGroupe, learnerIds? | Démarre l'examen (tous ou certains) |
| `PauseExam` | codeGroupe, learnerId? | Met en pause (global ou individuel) |
| `ResumeExam` | codeGroupe, learnerId? | Reprend l'examen |
| `EndExam` | codeGroupe, learnerId? | Termine l'examen |
| `AddTime` | codeGroupe, minutes, learnerId? | Ajoute du temps |
| `SendMessage` | codeGroupe, message, learnerId? | Envoie un message |
| `ToggleChat` | codeGroupe, learnerId, enabled | Active/désactive le chat |
| `GetSessionState` | codeGroupe | Retourne l'état complet |
| `ApprovePause` | codeGroupe, learnerId | Approuve une demande de pause |
| `DenyPause` | codeGroupe, learnerId, reason | Refuse une demande de pause |

#### 2.3 Méthodes étudiant

| Méthode | Paramètres | Description |
|---------|------------|-------------|
| `JoinExam` | codeGroupe, learnerInfo | Rejoint la session d'examen |
| `RequestPause` | codeGroupe, reason | Demande une pause |
| `ReportProblem` | codeGroupe, description | Signale un problème technique |
| `UpdateProgress` | codeGroupe, questionIndex, tempsRestant | Met à jour la progression |
| `SendChatMessage` | codeGroupe, message | Envoie un message au surveillant |
| `ReportIncident` | codeGroupe, incidentType | Signale un incident (sortie plein écran, etc.) |
| `SubmitExam` | codeGroupe | Soumet l'examen |
| `Heartbeat` | codeGroupe | Signal de vie (toutes les 30s) |

#### 2.4 Gestion de l'état en mémoire

```csharp
public class ExamSessionState
{
    public int Id { get; set; }
    public string GroupCode { get; set; }
    public int SessionId { get; set; }
    public string SupervisorConnectionId { get; set; }
    public ExamSessionStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public ConcurrentDictionary<int, ParticipantState> Participants { get; set; }
}

public class ParticipantState
{
    public int LearnerId { get; set; }
    public string ConnectionId { get; set; }
    public ParticipantStatus Status { get; set; }
    public int RemainingTimeSeconds { get; set; }
    public int AdditionalTimeMinutes { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime LastHeartbeat { get; set; }
    public List<Incident> Incidents { get; set; }
}
```

---

### Phase 3 : Événements temps réel enrichis

**Durée estimée:** À déterminer par l'équipe

#### 3.1 Événements serveur → tous

| Événement | Données | Description |
|-----------|---------|-------------|
| `SessionStateChanged` | état complet ou delta | Changement d'état de la session |
| `ParticipantJoined` | participantInfo | Un participant a rejoint |
| `ParticipantLeft` | participantId | Un participant a quitté |
| `MessageReceived` | messageInfo | Message reçu |
| `AlertReceived` | alertInfo | Alerte reçue |

#### 3.2 Événements serveur → étudiant

| Événement | Données | Description |
|-----------|---------|-------------|
| `ExamStarted` | config (durée, questions, outils) | L'examen démarre |
| `ExamPaused` | - | L'examen est en pause |
| `ExamResumed` | - | L'examen reprend |
| `ExamEnded` | - | L'examen est terminé |
| `TimeAdded` | minutes | Temps supplémentaire accordé |
| `ChatToggled` | enabled | Chat activé/désactivé |
| `ForceSubmit` | - | Soumission forcée (temps écoulé) |
| `PauseApproved` | - | Demande de pause approuvée |
| `PauseDenied` | reason | Demande de pause refusée |

#### 3.3 Événements serveur → surveillant

| Événement | Données | Description |
|-----------|---------|-------------|
| `ParticipantStatusChanged` | participantId, newStatus | Changement de statut |
| `IncidentReported` | participantId, incidentInfo | Incident signalé |
| `PauseRequested` | participantId, reason | Demande de pause |
| `ProgressUpdated` | participantId, progress | Progression mise à jour |
| `TimeWarning` | participantId, remainingSeconds | Alerte temps (< 5 min) |
| `ExamSubmitted` | participantId | Examen soumis |
| `HeartbeatMissed` | participantId | Pas de signal depuis > 60s |

---

### Phase 4 : Fonctionnalités avancées

**Durée estimée:** À déterminer par l'équipe

| Fonctionnalité | Description | Priorité |
|----------------|-------------|----------|
| **Récupération de session** | Restaurer l'état complet après reconnexion surveillant ou étudiant | Haute |
| **Multi-surveillant** | Plusieurs surveillants peuvent superviser la même session | Moyenne |
| **Outils technologiques** | Intégration avec `learnerTechnologicalTools` pour autoriser/bloquer des outils | Moyenne |
| **Statistiques temps réel** | Progression moyenne, temps moyen par question, distribution des statuts | Moyenne |
| **Alertes automatiques** | Détection d'anomalies (temps anormal sur une question, inactivité prolongée) | Basse |
| **Export session** | Rapport PDF post-examen avec tous les événements | Basse |
| **Détection de triche** | Analyse des patterns (copier-coller, changements d'onglet fréquents) | Basse |

---

## Ce qui manque actuellement

### 1. Gestion des données d'examen

- [ ] Liaison avec la table `convocations` (qui passe quel examen)
- [ ] Liaison avec `examDocuments` (quel document pour quel examen)
- [ ] Liaison avec `sessions` (quelle session d'administration)
- [ ] Configuration des outils autorisés par apprenant (`learnerTechnologicalTools`)
- [ ] Gestion des accommodations (temps supplémentaire pré-configuré)

### 2. Sécurité et validation

- [ ] Authentification des connexions SignalR (JWT)
- [ ] Validation des droits (est-ce bien le surveillant de cette session?)
- [ ] Validation des transitions de statut (machine à états)
- [ ] Rate limiting des messages
- [ ] Chiffrement des données sensibles

### 3. Persistance et récupération

- [ ] Sauvegarde de l'état en base de données
- [ ] Récupération après déconnexion surveillant
- [ ] Récupération après déconnexion étudiant (avec temps ajusté)
- [ ] Backup automatique toutes les X minutes
- [ ] Gestion des conflits de version

### 4. Monitoring et audit

- [ ] Logging de toutes les actions (examLogs)
- [ ] Tableaux de bord temps réel
- [ ] Alertes administrateur (email/notification)
- [ ] Rapport post-examen détaillé
- [ ] Métriques de performance SignalR

### 5. Fonctionnalités métier manquantes

- [ ] Gestion du temps par étudiant (accommodations pré-configurées)
- [ ] Blocage d'application (plein écran forcé côté serveur)
- [ ] Détection de triche (changement d'onglet, copier-coller)
- [ ] Soumission automatique à la fin du temps
- [ ] Mode question par question vs document complet
- [ ] Impression des résultats
- [ ] Notification par email aux parents/tuteurs

---

## Recommandation de priorisation

### PRIORITÉ HAUTE (Phase 1-2)

```
┌────────────────────────────────────────────────────┐
│ 1. Tables de persistance                           │
│    - examSessions                                  │
│    - examParticipants                              │
│    - examLogs                                      │
│    - examIncidents                                 │
│    - examMessages                                  │
│                                                    │
│ 2. ExamHub.cs avec état serveur                    │
│    - Gestion des connexions                        │
│    - Méthodes surveillant/étudiant                 │
│    - État en mémoire synchronisé avec DB           │
│                                                    │
│ 3. Authentification SignalR                        │
│    - Validation JWT                                │
│    - Vérification des droits                       │
│                                                    │
│ 4. Récupération après déconnexion                  │
│    - Restauration de l'état                        │
│    - Ajustement du temps                           │
└────────────────────────────────────────────────────┘
```

### PRIORITÉ MOYENNE (Phase 3)

```
┌────────────────────────────────────────────────────┐
│ 5. Logging et audit complet                        │
│ 6. Gestion des accommodations (temps supplémentaire│
│ 7. Intégration convocations/sessions               │
│ 8. Statistiques temps réel                         │
│ 9. Heartbeat et détection de déconnexion           │
└────────────────────────────────────────────────────┘
```

### PRIORITÉ BASSE (Phase 4)

```
┌────────────────────────────────────────────────────┐
│ 10. Multi-surveillant                              │
│ 11. Détection de triche avancée                    │
│ 12. Export PDF                                     │
│ 13. Alertes automatiques                           │
│ 14. Intégration outils technologiques              │
└────────────────────────────────────────────────────┘
```

---

## Diagramme des relations de données

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│   exams     │     │  sessions   │     │  learners   │
└──────┬──────┘     └──────┬──────┘     └──────┬──────┘
       │                   │                   │
       │    ┌──────────────┴──────────────┐    │
       │    │                             │    │
       ▼    ▼                             ▼    ▼
┌─────────────────┐               ┌─────────────────┐
│  convocations   │               │  examSessions   │
│  (existante)    │               │  (nouvelle)     │
└────────┬────────┘               └────────┬────────┘
         │                                 │
         │         ┌───────────────────────┤
         │         │                       │
         ▼         ▼                       ▼
    ┌─────────────────┐           ┌─────────────────┐
    │ examParticipants│           │   examLogs      │
    │   (nouvelle)    │           │   (nouvelle)    │
    └────────┬────────┘           └─────────────────┘
             │
    ┌────────┴────────┐
    │                 │
    ▼                 ▼
┌─────────────┐  ┌─────────────┐
│examIncidents│  │examMessages │
│ (nouvelle)  │  │ (nouvelle)  │
└─────────────┘  └─────────────┘
```

---

## Notes de mise en oeuvre

### Migration du client

Le composable `useGroupeExamen.js` devra être adapté pour :
1. Appeler les nouvelles méthodes du hub
2. Gérer les nouveaux événements
3. Simplifier la logique (déléguer au serveur)

### Rétrocompatibilité

Pendant la transition :
1. Garder `NotificationHub.cs` pour les autres fonctionnalités
2. Créer `ExamHub.cs` sur une route séparée (`/hubs/exam`)
3. Migrer progressivement les clients

### Tests

- [ ] Tests unitaires pour les transitions de statut
- [ ] Tests d'intégration pour les scénarios complets
- [ ] Tests de charge pour la concurrence
- [ ] Tests de récupération après panne

---

## Changelog

| Version | Date | Changements |
|---------|------|-------------|
| 1.0 | 2026-02-03 | Version initiale du plan |
