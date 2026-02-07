using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace serveur.Migrations
{
    public partial class AddTextAnnotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "functions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    labelFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    labelEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    route = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_functions", x => x.id);
                    table.ForeignKey(
                        name: "FK_functions_functions_parentId",
                        column: x => x.parentId,
                        principalTable: "functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "organisations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    acronym = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    webSite = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organisations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pageContents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    pedagogicalStrudtureId = table.Column<int>(type: "int", nullable: true),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    contentFr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    contentEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isEndPage = table.Column<bool>(type: "bit", nullable: false),
                    requiresConsent = table.Column<bool>(type: "bit", nullable: false),
                    consentTextFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    consentTextEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pageContents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prompts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prompts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedagogicalStructureId = table.Column<int>(type: "int", nullable: true),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    testId = table.Column<int>(type: "int", nullable: false),
                    scheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    authTypeId = table.Column<int>(type: "int", nullable: false),
                    durationMinutes = table.Column<int>(type: "int", nullable: false),
                    allowLanguageChange = table.Column<bool>(type: "bit", nullable: true),
                    allowItemMarking = table.Column<bool>(type: "bit", nullable: true),
                    allowNoteTaking = table.Column<bool>(type: "bit", nullable: true),
                    allowItemScrambling = table.Column<bool>(type: "bit", nullable: true),
                    allowHighContrast = table.Column<bool>(type: "bit", nullable: true),
                    forceFullscreen = table.Column<bool>(type: "bit", nullable: true),
                    enableKeyboardShortcuts = table.Column<bool>(type: "bit", nullable: true),
                    enableGuidedTour = table.Column<bool>(type: "bit", nullable: true),
                    questionSummaryNotice = table.Column<int>(type: "int", nullable: true),
                    enableRemoteMode = table.Column<bool>(type: "bit", nullable: true),
                    debugMode = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "technologicalTools",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    displayOrder = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    adaptiveMeasure = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_technologicalTools", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "titles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maleLabelFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    femaleLabelFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    maleLabelEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    femaleLabelEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titles", x => x.id);
                    table.ForeignKey(
                        name: "FK_titles_titles_parentId",
                        column: x => x.parentId,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "valueDomains",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    isOrdered = table.Column<bool>(type: "bit", nullable: false),
                    isPublic = table.Column<bool>(type: "bit", nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_valueDomains", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "administrationCenters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    organisationId = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    shortName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    officialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    province = table.Column<int>(type: "int", nullable: true),
                    country = table.Column<int>(type: "int", nullable: true),
                    postalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    contactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    contactExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrationCenters", x => x.id);
                    table.ForeignKey(
                        name: "FK_administrationCenters_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "learningCenters",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    internalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    shortName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    officialName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    phoneExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    province = table.Column<int>(type: "int", nullable: true),
                    postalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    administrativeRegion = table.Column<int>(type: "int", nullable: true),
                    educationNetwork = table.Column<int>(type: "int", nullable: true),
                    educationLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    teachingLanguage = table.Column<int>(type: "int", nullable: true),
                    organisationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learningCenters", x => x.id);
                    table.ForeignKey(
                        name: "FK_learningCenters_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "pedagogicalElementTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    organisationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedagogicalElementTypes", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedagogicalElementTypes_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    organisationId = table.Column<int>(type: "int", nullable: true),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    level = table.Column<int>(type: "int", nullable: false),
                    isSystem = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    hasAllPermissions = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                    table.ForeignKey(
                        name: "FK_roles_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_roles_roles_parentId",
                        column: x => x.parentId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "promptVersions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    promptId = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    newContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promptVersions", x => x.id);
                    table.ForeignKey(
                        name: "FK_promptVersions_prompts_promptId",
                        column: x => x.promptId,
                        principalTable: "prompts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "valueDomainItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    valueDomainId = table.Column<int>(type: "int", nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    valueFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    valueEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_valueDomainItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_valueDomainItems_valueDomains_valueDomainId",
                        column: x => x.valueDomainId,
                        principalTable: "valueDomains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pedagogicalStructures",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pedagogicalElementTypeId = table.Column<int>(type: "int", nullable: false),
                    sectorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    organisationId = table.Column<int>(type: "int", nullable: true),
                    sortOrder = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedagogicalStructures", x => x.id);
                    table.ForeignKey(
                        name: "FK_pedagogicalStructures_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_pedagogicalStructures_pedagogicalElementTypes_pedagogicalElementTypeId",
                        column: x => x.pedagogicalElementTypeId,
                        principalTable: "pedagogicalElementTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pedagogicalStructures_pedagogicalStructures_parentId",
                        column: x => x.parentId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "roleFunctions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    functionCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roleFunctions", x => x.id);
                    table.ForeignKey(
                        name: "FK_roleFunctions_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "classifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedagogicalStrudtureId = table.Column<int>(type: "int", nullable: true),
                    tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    descriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    isRequired = table.Column<bool>(type: "bit", nullable: false),
                    allowMultiple = table.Column<bool>(type: "bit", nullable: false),
                    hasDescription = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_classifications_pedagogicalStructures_pedagogicalStrudtureId",
                        column: x => x.pedagogicalStrudtureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documentTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedagogicalStrudtureId = table.Column<int>(type: "int", nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    titleFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    titleEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentTypes", x => x.id);
                    table.ForeignKey(
                        name: "FK_documentTypes_pedagogicalStructures_pedagogicalStrudtureId",
                        column: x => x.pedagogicalStrudtureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "itemBanks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    pedagogicalStrudtureId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: true),
                    hasFeedback = table.Column<bool>(type: "bit", nullable: true),
                    hasFeedbackForChoices = table.Column<bool>(type: "bit", nullable: true),
                    hasDocumentation = table.Column<bool>(type: "bit", nullable: true),
                    hasDocumentationForChoices = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemBanks", x => x.id);
                    table.ForeignKey(
                        name: "FK_itemBanks_pedagogicalStructures_pedagogicalStrudtureId",
                        column: x => x.pedagogicalStrudtureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    organisationId = table.Column<int>(type: "int", nullable: false),
                    pedagogicalStrudturId = table.Column<int>(type: "int", nullable: true),
                    useCompression = table.Column<bool>(type: "bit", nullable: false),
                    compressionMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    compressionPage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    widgetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    controllerMethod = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_reports_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reports_pedagogicalStructures_pedagogicalStrudturId",
                        column: x => x.pedagogicalStrudturId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    firstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sexe = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    darkMode = table.Column<bool>(type: "bit", nullable: false),
                    avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    accentColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    organisationId = table.Column<int>(type: "int", nullable: true),
                    pedagogicalStructureId = table.Column<int>(type: "int", nullable: true),
                    learningCenterId = table.Column<int>(type: "int", nullable: true),
                    titleId = table.Column<int>(type: "int", nullable: true),
                    activeRoleId = table.Column<int>(type: "int", nullable: true),
                    resetToken = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    resetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_learningCenters_learningCenterId",
                        column: x => x.learningCenterId,
                        principalTable: "learningCenters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_pedagogicalStructures_pedagogicalStructureId",
                        column: x => x.pedagogicalStructureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_roles_activeRoleId",
                        column: x => x.activeRoleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_titles_titleId",
                        column: x => x.titleId,
                        principalTable: "titles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "classificationNodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classificationId = table.Column<int>(type: "int", nullable: false),
                    parentId = table.Column<int>(type: "int", nullable: true),
                    label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    weight = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    referencesJuridiques = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classificationNodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_classificationNodes_classificationNodes_parentId",
                        column: x => x.parentId,
                        principalTable: "classificationNodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_classificationNodes_classifications_classificationId",
                        column: x => x.classificationId,
                        principalTable: "classifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "itemBankClassifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemBankId = table.Column<int>(type: "int", nullable: false),
                    classificationId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemBankClassifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_itemBankClassifications_classifications_classificationId",
                        column: x => x.classificationId,
                        principalTable: "classifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_itemBankClassifications_itemBanks_itemBankId",
                        column: x => x.itemBankId,
                        principalTable: "itemBanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pedagogicalStrudtureId = table.Column<int>(type: "int", nullable: true),
                    documentTypeId = table.Column<int>(type: "int", nullable: true),
                    externalReferenceCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    titleFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    titleEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    welcomeMessageFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    welcomeMessageEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    copyrightFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    copyrightEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    urlFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    urlEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDownloadable = table.Column<bool>(type: "bit", nullable: false),
                    isPublic = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    isEditable = table.Column<bool>(type: "bit", nullable: false),
                    editorSettings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authorId = table.Column<int>(type: "int", nullable: true),
                    isTemplate = table.Column<bool>(type: "bit", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                    table.ForeignKey(
                        name: "FK_documents_documentTypes_documentTypeId",
                        column: x => x.documentTypeId,
                        principalTable: "documentTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_pedagogicalStructures_pedagogicalStrudtureId",
                        column: x => x.pedagogicalStrudtureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_documents_users_authorId",
                        column: x => x.authorId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    functionId = table.Column<int>(type: "int", nullable: false),
                    order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorites", x => x.id);
                    table.ForeignKey(
                        name: "FK_favorites_functions_functionId",
                        column: x => x.functionId,
                        principalTable: "functions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorites_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    learningCenterId = table.Column<int>(type: "int", nullable: false),
                    gradeId = table.Column<int>(type: "int", nullable: false),
                    teacherId = table.Column<int>(type: "int", nullable: true),
                    proctorId = table.Column<int>(type: "int", nullable: true),
                    languageId = table.Column<int>(type: "int", nullable: false),
                    academicYearId = table.Column<int>(type: "int", nullable: false),
                    allowManualEntry = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_groups_learningCenters_learningCenterId",
                        column: x => x.learningCenterId,
                        principalTable: "learningCenters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groups_users_proctorId",
                        column: x => x.proctorId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_groups_users_teacherId",
                        column: x => x.teacherId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_groups_valueDomainItems_academicYearId",
                        column: x => x.academicYearId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groups_valueDomainItems_gradeId",
                        column: x => x.gradeId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groups_valueDomainItems_languageId",
                        column: x => x.languageId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "promptVersionComments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    promptVersionId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_promptVersionComments", x => x.id);
                    table.ForeignKey(
                        name: "FK_promptVersionComments_promptVersions_promptVersionId",
                        column: x => x.promptVersionId,
                        principalTable: "promptVersions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_promptVersionComments_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_revoked = table.Column<bool>(type: "bit", nullable: false),
                    replaced_by_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    organisationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_userRoles_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userRoles_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userRoles_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "classificationNodeCriteria",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    classificationNodeId = table.Column<int>(type: "int", nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    isLegalObligation = table.Column<bool>(type: "bit", nullable: false),
                    examplesFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    examplesEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classificationNodeCriteria", x => x.id);
                    table.ForeignKey(
                        name: "FK_classificationNodeCriteria_classificationNodes_classificationNodeId",
                        column: x => x.classificationNodeId,
                        principalTable: "classificationNodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documentMedias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    documentId = table.Column<int>(type: "int", nullable: false),
                    fileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    mimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    fileSize = table.Column<int>(type: "int", nullable: true),
                    dataUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    width = table.Column<int>(type: "int", nullable: true),
                    height = table.Column<int>(type: "int", nullable: true),
                    uploadedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentMedias", x => x.id);
                    table.ForeignKey(
                        name: "FK_documentMedias_documents_documentId",
                        column: x => x.documentId,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    typeId = table.Column<int>(type: "int", nullable: false),
                    label = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    statusId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: true),
                    documentId = table.Column<int>(type: "int", nullable: true),
                    itemBankId = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    number = table.Column<int>(type: "int", nullable: true),
                    context = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    statement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    points = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    required = table.Column<bool>(type: "bit", nullable: true),
                    hint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    media = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_items_documents_documentId",
                        column: x => x.documentId,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_itemBanks_itemBankId",
                        column: x => x.itemBankId,
                        principalTable: "itemBanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_items_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "templatePages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    templateDocumentId = table.Column<int>(type: "int", nullable: false),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    pageNameFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    pageNameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templatePages", x => x.id);
                    table.ForeignKey(
                        name: "FK_templatePages_documents_templateDocumentId",
                        column: x => x.templateDocumentId,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "groupTechnologicalTools",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    groupId = table.Column<int>(type: "int", nullable: false),
                    technologicalToolId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groupTechnologicalTools", x => x.id);
                    table.ForeignKey(
                        name: "FK_groupTechnologicalTools_groups_groupId",
                        column: x => x.groupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_groupTechnologicalTools_technologicalTools_technologicalToolId",
                        column: x => x.technologicalToolId,
                        principalTable: "technologicalTools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learners",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    permanentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    firstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    dateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    nativeUserCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    nativePassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    learningCenterId = table.Column<int>(type: "int", nullable: false),
                    groupId = table.Column<int>(type: "int", nullable: true),
                    languageId = table.Column<int>(type: "int", nullable: false),
                    hasAccommodations = table.Column<bool>(type: "bit", nullable: false),
                    isManualEntry = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    modifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learners", x => x.id);
                    table.ForeignKey(
                        name: "FK_learners_groups_groupId",
                        column: x => x.groupId,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_learners_learningCenters_learningCenterId",
                        column: x => x.learningCenterId,
                        principalTable: "learningCenters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_learners_valueDomainItems_languageId",
                        column: x => x.languageId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analyses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analyses", x => x.id);
                    table.ForeignKey(
                        name: "FK_analyses_items_itemId",
                        column: x => x.itemId,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "itemClassificationNodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    classificationNodeId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemClassificationNodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_itemClassificationNodes_classificationNodes_classificationNodeId",
                        column: x => x.classificationNodeId,
                        principalTable: "classificationNodes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_itemClassificationNodes_items_itemId",
                        column: x => x.itemId,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "itemVersions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itemVersions", x => x.id);
                    table.ForeignKey(
                        name: "FK_itemVersions_items_itemId",
                        column: x => x.itemId,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "modifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_modifications_items_itemId",
                        column: x => x.itemId,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "revisions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    itemId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revisions", x => x.id);
                    table.ForeignKey(
                        name: "FK_revisions_items_itemId",
                        column: x => x.itemId,
                        principalTable: "items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "documentElements",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    documentId = table.Column<int>(type: "int", nullable: false),
                    sortOrder = table.Column<int>(type: "int", nullable: true),
                    elementType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    templatePageId = table.Column<int>(type: "int", nullable: true),
                    styleProps = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentElements", x => x.id);
                    table.ForeignKey(
                        name: "FK_documentElements_documents_documentId",
                        column: x => x.documentId,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documentElements_templatePages_templatePageId",
                        column: x => x.templatePageId,
                        principalTable: "templatePages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "learnerTechnologicalTools",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    learnerId = table.Column<int>(type: "int", nullable: false),
                    technologicalToolId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learnerTechnologicalTools", x => x.id);
                    table.ForeignKey(
                        name: "FK_learnerTechnologicalTools_learners_learnerId",
                        column: x => x.learnerId,
                        principalTable: "learners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_learnerTechnologicalTools_technologicalTools_technologicalToolId",
                        column: x => x.technologicalToolId,
                        principalTable: "technologicalTools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "textAnnotations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    learnerId = table.Column<int>(type: "int", nullable: false),
                    contextId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    annotationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    textContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    xPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    startOffset = table.Column<int>(type: "int", nullable: false),
                    endOffset = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_textAnnotations", x => x.id);
                    table.ForeignKey(
                        name: "FK_textAnnotations_learners_learnerId",
                        column: x => x.learnerId,
                        principalTable: "learners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_administrationCenters_organisationId",
                table: "administrationCenters",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_analyses_itemId",
                table: "analyses",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_classificationNodeCriteria_classificationNodeId",
                table: "classificationNodeCriteria",
                column: "classificationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_classificationNodes_classificationId",
                table: "classificationNodes",
                column: "classificationId");

            migrationBuilder.CreateIndex(
                name: "IX_classificationNodes_parentId",
                table: "classificationNodes",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_classifications_pedagogicalStrudtureId",
                table: "classifications",
                column: "pedagogicalStrudtureId");

            migrationBuilder.CreateIndex(
                name: "IX_documentElements_documentId",
                table: "documentElements",
                column: "documentId");

            migrationBuilder.CreateIndex(
                name: "IX_documentElements_templatePageId",
                table: "documentElements",
                column: "templatePageId");

            migrationBuilder.CreateIndex(
                name: "IX_documentMedias_documentId",
                table: "documentMedias",
                column: "documentId");

            migrationBuilder.CreateIndex(
                name: "IX_documents_authorId",
                table: "documents",
                column: "authorId");

            migrationBuilder.CreateIndex(
                name: "IX_documents_documentTypeId",
                table: "documents",
                column: "documentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_documents_pedagogicalStrudtureId",
                table: "documents",
                column: "pedagogicalStrudtureId");

            migrationBuilder.CreateIndex(
                name: "IX_documentTypes_pedagogicalStrudtureId",
                table: "documentTypes",
                column: "pedagogicalStrudtureId");

            migrationBuilder.CreateIndex(
                name: "IX_favorites_functionId",
                table: "favorites",
                column: "functionId");

            migrationBuilder.CreateIndex(
                name: "IX_favorites_userId",
                table: "favorites",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_functions_code",
                table: "functions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_functions_parentId",
                table: "functions",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_academicYearId",
                table: "groups",
                column: "academicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_gradeId",
                table: "groups",
                column: "gradeId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_languageId",
                table: "groups",
                column: "languageId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_learningCenterId",
                table: "groups",
                column: "learningCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_proctorId",
                table: "groups",
                column: "proctorId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_teacherId",
                table: "groups",
                column: "teacherId");

            migrationBuilder.CreateIndex(
                name: "IX_groupTechnologicalTools_groupId",
                table: "groupTechnologicalTools",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_groupTechnologicalTools_technologicalToolId",
                table: "groupTechnologicalTools",
                column: "technologicalToolId");

            migrationBuilder.CreateIndex(
                name: "IX_itemBankClassifications_classificationId",
                table: "itemBankClassifications",
                column: "classificationId");

            migrationBuilder.CreateIndex(
                name: "IX_itemBankClassifications_itemBankId",
                table: "itemBankClassifications",
                column: "itemBankId");

            migrationBuilder.CreateIndex(
                name: "IX_itemBanks_pedagogicalStrudtureId",
                table: "itemBanks",
                column: "pedagogicalStrudtureId");

            migrationBuilder.CreateIndex(
                name: "IX_itemClassificationNodes_classificationNodeId",
                table: "itemClassificationNodes",
                column: "classificationNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_itemClassificationNodes_itemId",
                table: "itemClassificationNodes",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_items_documentId",
                table: "items",
                column: "documentId");

            migrationBuilder.CreateIndex(
                name: "IX_items_itemBankId",
                table: "items",
                column: "itemBankId");

            migrationBuilder.CreateIndex(
                name: "IX_items_userId",
                table: "items",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_itemVersions_itemId",
                table: "itemVersions",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_learners_email",
                table: "learners",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_learners_groupId",
                table: "learners",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_learners_languageId",
                table: "learners",
                column: "languageId");

            migrationBuilder.CreateIndex(
                name: "IX_learners_learningCenterId",
                table: "learners",
                column: "learningCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_learners_permanentCode",
                table: "learners",
                column: "permanentCode",
                unique: true,
                filter: "[permanentCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_learnerTechnologicalTools_learnerId",
                table: "learnerTechnologicalTools",
                column: "learnerId");

            migrationBuilder.CreateIndex(
                name: "IX_learnerTechnologicalTools_technologicalToolId",
                table: "learnerTechnologicalTools",
                column: "technologicalToolId");

            migrationBuilder.CreateIndex(
                name: "IX_learningCenters_organisationId",
                table: "learningCenters",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_modifications_itemId",
                table: "modifications",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_pedagogicalElementTypes_organisationId",
                table: "pedagogicalElementTypes",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_pedagogicalStructures_organisationId",
                table: "pedagogicalStructures",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_pedagogicalStructures_parentId",
                table: "pedagogicalStructures",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_pedagogicalStructures_pedagogicalElementTypeId",
                table: "pedagogicalStructures",
                column: "pedagogicalElementTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_promptVersionComments_promptVersionId",
                table: "promptVersionComments",
                column: "promptVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_promptVersionComments_userId",
                table: "promptVersionComments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_promptVersions_promptId",
                table: "promptVersions",
                column: "promptId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_organisationId",
                table: "reports",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_pedagogicalStrudturId",
                table: "reports",
                column: "pedagogicalStrudturId");

            migrationBuilder.CreateIndex(
                name: "IX_revisions_itemId",
                table: "revisions",
                column: "itemId");

            migrationBuilder.CreateIndex(
                name: "IX_roleFunctions_roleId",
                table: "roleFunctions",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_code",
                table: "roles",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_organisationId",
                table: "roles",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_roles_parentId",
                table: "roles",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_templatePages_templateDocumentId",
                table: "templatePages",
                column: "templateDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_textAnnotations_learnerId",
                table: "textAnnotations",
                column: "learnerId");

            migrationBuilder.CreateIndex(
                name: "IX_titles_parentId",
                table: "titles",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_organisationId",
                table: "userRoles",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_roleId",
                table: "userRoles",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_userId",
                table: "userRoles",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_users_activeRoleId",
                table: "users",
                column: "activeRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_learningCenterId",
                table: "users",
                column: "learningCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_users_mail",
                table: "users",
                column: "mail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_organisationId",
                table: "users",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_users_pedagogicalStructureId",
                table: "users",
                column: "pedagogicalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_users_titleId",
                table: "users",
                column: "titleId");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_valueDomainItems_valueDomainId",
                table: "valueDomainItems",
                column: "valueDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_valueDomains_tag",
                table: "valueDomains",
                column: "tag",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrationCenters");

            migrationBuilder.DropTable(
                name: "analyses");

            migrationBuilder.DropTable(
                name: "classificationNodeCriteria");

            migrationBuilder.DropTable(
                name: "documentElements");

            migrationBuilder.DropTable(
                name: "documentMedias");

            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "groupTechnologicalTools");

            migrationBuilder.DropTable(
                name: "itemBankClassifications");

            migrationBuilder.DropTable(
                name: "itemClassificationNodes");

            migrationBuilder.DropTable(
                name: "itemVersions");

            migrationBuilder.DropTable(
                name: "learnerTechnologicalTools");

            migrationBuilder.DropTable(
                name: "modifications");

            migrationBuilder.DropTable(
                name: "pageContents");

            migrationBuilder.DropTable(
                name: "promptVersionComments");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "revisions");

            migrationBuilder.DropTable(
                name: "roleFunctions");

            migrationBuilder.DropTable(
                name: "sessions");

            migrationBuilder.DropTable(
                name: "textAnnotations");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "templatePages");

            migrationBuilder.DropTable(
                name: "functions");

            migrationBuilder.DropTable(
                name: "classificationNodes");

            migrationBuilder.DropTable(
                name: "technologicalTools");

            migrationBuilder.DropTable(
                name: "promptVersions");

            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "learners");

            migrationBuilder.DropTable(
                name: "classifications");

            migrationBuilder.DropTable(
                name: "prompts");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "itemBanks");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "documentTypes");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "valueDomainItems");

            migrationBuilder.DropTable(
                name: "learningCenters");

            migrationBuilder.DropTable(
                name: "pedagogicalStructures");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "titles");

            migrationBuilder.DropTable(
                name: "valueDomains");

            migrationBuilder.DropTable(
                name: "pedagogicalElementTypes");

            migrationBuilder.DropTable(
                name: "organisations");
        }
    }
}
