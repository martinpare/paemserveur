using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace serveur.Migrations
{
    public partial class AddDashboardLayouts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "revisions");

            migrationBuilder.AddColumn<string>(
                name: "defaultData",
                table: "technologicalTools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "interface",
                table: "technologicalTools",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "data",
                table: "learnerTechnologicalTools",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "avatar",
                table: "learners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "dashboardLayouts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    roleId = table.Column<int>(type: "int", nullable: true),
                    pageKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    isDefault = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboardLayouts", x => x.id);
                    table.ForeignKey(
                        name: "FK_dashboardLayouts_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dictionary_metadata",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    currentVersion = table.Column<int>(type: "int", nullable: false),
                    totalWords = table.Column<int>(type: "int", nullable: false),
                    checksum = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dictionary_metadata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dictionary_versions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    version = table.Column<int>(type: "int", nullable: false),
                    changeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    wordId = table.Column<int>(type: "int", nullable: true),
                    changeData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dictionary_versions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "discussionThreads",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    entityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entityId = table.Column<int>(type: "int", nullable: false),
                    contextType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    contextId = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    createdByUserId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussionThreads", x => x.id);
                    table.ForeignKey(
                        name: "FK_discussionThreads_users_createdByUserId",
                        column: x => x.createdByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documentItemBanks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    documentId = table.Column<int>(type: "int", nullable: false),
                    itemBankId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentItemBanks", x => x.id);
                    table.ForeignKey(
                        name: "FK_documentItemBanks_documents_documentId",
                        column: x => x.documentId,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_documentItemBanks_itemBanks_itemBankId",
                        column: x => x.itemBankId,
                        principalTable: "itemBanks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationProcesses",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reviewableTypeId = table.Column<int>(type: "int", nullable: false),
                    organisationId = table.Column<int>(type: "int", nullable: true),
                    pedagogicalStructureId = table.Column<int>(type: "int", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationProcesses", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationProcesses_organisations_organisationId",
                        column: x => x.organisationId,
                        principalTable: "organisations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validationProcesses_pedagogicalStructures_pedagogicalStructureId",
                        column: x => x.pedagogicalStructureId,
                        principalTable: "pedagogicalStructures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validationProcesses_valueDomainItems_reviewableTypeId",
                        column: x => x.reviewableTypeId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    orthographe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    categorie = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    frequenceCp = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    frequenceCe1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    frequenceCe2Cm2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    frequenceGlobale = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    phon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phonSimplifiee = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dashboardLayoutSlots",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    layoutId = table.Column<int>(type: "int", nullable: false),
                    componentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    sortOrder = table.Column<int>(type: "int", nullable: false),
                    colXs = table.Column<int>(type: "int", nullable: false),
                    colSm = table.Column<int>(type: "int", nullable: true),
                    colMd = table.Column<int>(type: "int", nullable: false),
                    colLg = table.Column<int>(type: "int", nullable: true),
                    colXl = table.Column<int>(type: "int", nullable: true),
                    offsetXs = table.Column<int>(type: "int", nullable: false),
                    offsetMd = table.Column<int>(type: "int", nullable: true),
                    componentConfig = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dashboardLayoutSlots", x => x.id);
                    table.ForeignKey(
                        name: "FK_dashboardLayoutSlots_dashboardLayouts_layoutId",
                        column: x => x.layoutId,
                        principalTable: "dashboardLayouts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discussionMessages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    threadId = table.Column<int>(type: "int", nullable: false),
                    parentMessageId = table.Column<int>(type: "int", nullable: true),
                    authorUserId = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isEdited = table.Column<bool>(type: "bit", nullable: false),
                    editedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    deletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussionMessages", x => x.id);
                    table.ForeignKey(
                        name: "FK_discussionMessages_discussionMessages_parentMessageId",
                        column: x => x.parentMessageId,
                        principalTable: "discussionMessages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_discussionMessages_discussionThreads_threadId",
                        column: x => x.threadId,
                        principalTable: "discussionThreads",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussionMessages_users_authorUserId",
                        column: x => x.authorUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationProcessSteps",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationProcessId = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    stepOrder = table.Column<int>(type: "int", nullable: false),
                    nameFr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    nameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    descriptionFr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    descriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isMandatory = table.Column<bool>(type: "bit", nullable: false),
                    requiredValidations = table.Column<int>(type: "int", nullable: false),
                    timeoutDays = table.Column<int>(type: "int", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationProcessSteps", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationProcessSteps_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationProcessSteps_validationProcesses_validationProcessId",
                        column: x => x.validationProcessId,
                        principalTable: "validationProcesses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "discussionMentions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    messageId = table.Column<int>(type: "int", nullable: false),
                    mentionedUserId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isRead = table.Column<bool>(type: "bit", nullable: false),
                    readAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discussionMentions", x => x.id);
                    table.ForeignKey(
                        name: "FK_discussionMentions_discussionMessages_messageId",
                        column: x => x.messageId,
                        principalTable: "discussionMessages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_discussionMentions_users_mentionedUserId",
                        column: x => x.mentionedUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationInstanceRoleUsers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationInstanceRoleId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationInstanceRoleUsers", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationInstanceRoleUsers_users_userId",
                        column: x => x.userId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationInstanceRoles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationInstanceId = table.Column<int>(type: "int", nullable: false),
                    roleId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationInstanceRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationInstanceRoles_roles_roleId",
                        column: x => x.roleId,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationInstanceSteps",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationInstanceId = table.Column<int>(type: "int", nullable: false),
                    validationProcessStepId = table.Column<int>(type: "int", nullable: false),
                    statusId = table.Column<int>(type: "int", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationInstanceSteps", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationInstanceSteps_validationProcessSteps_validationProcessStepId",
                        column: x => x.validationProcessStepId,
                        principalTable: "validationProcessSteps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationInstanceSteps_valueDomainItems_statusId",
                        column: x => x.statusId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationInstances",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationProcessId = table.Column<int>(type: "int", nullable: false),
                    reviewableTypeId = table.Column<int>(type: "int", nullable: false),
                    targetEntityId = table.Column<int>(type: "int", nullable: false),
                    currentStepId = table.Column<int>(type: "int", nullable: true),
                    statusId = table.Column<int>(type: "int", nullable: false),
                    initiatedByUserId = table.Column<int>(type: "int", nullable: false),
                    initiatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    completedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationInstances", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationInstances_users_initiatedByUserId",
                        column: x => x.initiatedByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationInstances_validationInstanceSteps_currentStepId",
                        column: x => x.currentStepId,
                        principalTable: "validationInstanceSteps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validationInstances_validationProcesses_validationProcessId",
                        column: x => x.validationProcessId,
                        principalTable: "validationProcesses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationInstances_valueDomainItems_reviewableTypeId",
                        column: x => x.reviewableTypeId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationInstances_valueDomainItems_statusId",
                        column: x => x.statusId,
                        principalTable: "valueDomainItems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationInstanceStepValidations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    validationInstanceStepId = table.Column<int>(type: "int", nullable: false),
                    validatedByUserId = table.Column<int>(type: "int", nullable: false),
                    decision = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    validatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationInstanceStepValidations", x => x.id);
                    table.ForeignKey(
                        name: "FK_validationInstanceStepValidations_users_validatedByUserId",
                        column: x => x.validatedByUserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationInstanceStepValidations_validationInstanceSteps_validationInstanceStepId",
                        column: x => x.validationInstanceStepId,
                        principalTable: "validationInstanceSteps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dashboardLayouts_roleId",
                table: "dashboardLayouts",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_dashboardLayoutSlots_layoutId",
                table: "dashboardLayoutSlots",
                column: "layoutId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMentions_mentionedUserId",
                table: "discussionMentions",
                column: "mentionedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMentions_messageId_mentionedUserId",
                table: "discussionMentions",
                columns: new[] { "messageId", "mentionedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessages_authorUserId",
                table: "discussionMessages",
                column: "authorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessages_parentMessageId",
                table: "discussionMessages",
                column: "parentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionMessages_threadId",
                table: "discussionMessages",
                column: "threadId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionThreads_createdByUserId",
                table: "discussionThreads",
                column: "createdByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_discussionThreads_entityType_entityId",
                table: "discussionThreads",
                columns: new[] { "entityType", "entityId" });

            migrationBuilder.CreateIndex(
                name: "IX_documentItemBanks_documentId",
                table: "documentItemBanks",
                column: "documentId");

            migrationBuilder.CreateIndex(
                name: "IX_documentItemBanks_itemBankId",
                table: "documentItemBanks",
                column: "itemBankId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceRoles_roleId",
                table: "validationInstanceRoles",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceRoles_validationInstanceId_roleId",
                table: "validationInstanceRoles",
                columns: new[] { "validationInstanceId", "roleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceRoleUsers_userId",
                table: "validationInstanceRoleUsers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceRoleUsers_validationInstanceRoleId_userId",
                table: "validationInstanceRoleUsers",
                columns: new[] { "validationInstanceRoleId", "userId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_validationInstances_currentStepId",
                table: "validationInstances",
                column: "currentStepId",
                unique: true,
                filter: "[currentStepId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstances_initiatedByUserId",
                table: "validationInstances",
                column: "initiatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstances_reviewableTypeId",
                table: "validationInstances",
                column: "reviewableTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstances_statusId",
                table: "validationInstances",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstances_validationProcessId",
                table: "validationInstances",
                column: "validationProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceSteps_statusId",
                table: "validationInstanceSteps",
                column: "statusId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceSteps_validationInstanceId_validationProcessStepId",
                table: "validationInstanceSteps",
                columns: new[] { "validationInstanceId", "validationProcessStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceSteps_validationProcessStepId",
                table: "validationInstanceSteps",
                column: "validationProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceStepValidations_validatedByUserId",
                table: "validationInstanceStepValidations",
                column: "validatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_validationInstanceStepValidations_validationInstanceStepId_validatedByUserId",
                table: "validationInstanceStepValidations",
                columns: new[] { "validationInstanceStepId", "validatedByUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_validationProcesses_code_organisationId",
                table: "validationProcesses",
                columns: new[] { "code", "organisationId" },
                unique: true,
                filter: "[organisationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_validationProcesses_organisationId",
                table: "validationProcesses",
                column: "organisationId");

            migrationBuilder.CreateIndex(
                name: "IX_validationProcesses_pedagogicalStructureId",
                table: "validationProcesses",
                column: "pedagogicalStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_validationProcesses_reviewableTypeId",
                table: "validationProcesses",
                column: "reviewableTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_validationProcessSteps_roleId",
                table: "validationProcessSteps",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_validationProcessSteps_validationProcessId_stepOrder",
                table: "validationProcessSteps",
                columns: new[] { "validationProcessId", "stepOrder" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_validationInstanceRoleUsers_validationInstanceRoles_validationInstanceRoleId",
                table: "validationInstanceRoleUsers",
                column: "validationInstanceRoleId",
                principalTable: "validationInstanceRoles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_validationInstanceRoles_validationInstances_validationInstanceId",
                table: "validationInstanceRoles",
                column: "validationInstanceId",
                principalTable: "validationInstances",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_validationInstanceSteps_validationInstances_validationInstanceId",
                table: "validationInstanceSteps",
                column: "validationInstanceId",
                principalTable: "validationInstances",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_validationInstanceSteps_validationInstances_validationInstanceId",
                table: "validationInstanceSteps");

            migrationBuilder.DropTable(
                name: "dashboardLayoutSlots");

            migrationBuilder.DropTable(
                name: "dictionary_metadata");

            migrationBuilder.DropTable(
                name: "dictionary_versions");

            migrationBuilder.DropTable(
                name: "discussionMentions");

            migrationBuilder.DropTable(
                name: "documentItemBanks");

            migrationBuilder.DropTable(
                name: "validationInstanceRoleUsers");

            migrationBuilder.DropTable(
                name: "validationInstanceStepValidations");

            migrationBuilder.DropTable(
                name: "words");

            migrationBuilder.DropTable(
                name: "dashboardLayouts");

            migrationBuilder.DropTable(
                name: "discussionMessages");

            migrationBuilder.DropTable(
                name: "validationInstanceRoles");

            migrationBuilder.DropTable(
                name: "discussionThreads");

            migrationBuilder.DropTable(
                name: "validationInstances");

            migrationBuilder.DropTable(
                name: "validationInstanceSteps");

            migrationBuilder.DropTable(
                name: "validationProcessSteps");

            migrationBuilder.DropTable(
                name: "validationProcesses");

            migrationBuilder.DropColumn(
                name: "defaultData",
                table: "technologicalTools");

            migrationBuilder.DropColumn(
                name: "interface",
                table: "technologicalTools");

            migrationBuilder.DropColumn(
                name: "data",
                table: "learnerTechnologicalTools");

            migrationBuilder.DropColumn(
                name: "avatar",
                table: "learners");

            migrationBuilder.CreateTable(
                name: "revisions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    itemId = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateIndex(
                name: "IX_revisions_itemId",
                table: "revisions",
                column: "itemId");
        }
    }
}
