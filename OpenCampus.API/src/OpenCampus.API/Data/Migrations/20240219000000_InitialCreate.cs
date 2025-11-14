using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpenCampus.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Acronym = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    StudentEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Cpf = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InstitutionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InstitutionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professors_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetType = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    SuggestedData = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false, defaultValue: "PENDING"),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    ResolvedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InstitutionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProfessorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Users_ResolvedById",
                        column: x => x.ResolvedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetType = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    ScoreClarity = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoreRelevance = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoreSupport = table.Column<int>(type: "INTEGER", nullable: false),
                    ScoreInfrastructure = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    InstitutionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CourseId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ProfessorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SubjectId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Reviews_ScoreClarity", "\"ScoreClarity\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Reviews_ScoreInfrastructure", "\"ScoreInfrastructure\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Reviews_ScoreRelevance", "\"ScoreRelevance\" BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Reviews_ScoreSupport", "\"ScoreSupport\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReviewId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    IsOfficial = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_CourseId",
                table: "ChangeRequests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_CreatedById",
                table: "ChangeRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_InstitutionId",
                table: "ChangeRequests",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ProfessorId",
                table: "ChangeRequests",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_ResolvedById",
                table: "ChangeRequests",
                column: "ResolvedById");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_TargetType_Status",
                table: "ChangeRequests",
                columns: new[] { "TargetType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeRequests_SubjectId",
                table: "ChangeRequests",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ReviewId",
                table: "Comments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId_Name",
                table: "Courses",
                columns: new[] { "InstitutionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Name",
                table: "Institutions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professors_InstitutionId",
                table: "Professors",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Professors_InstitutionId_FullName",
                table: "Professors",
                columns: new[] { "InstitutionId", "FullName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorId",
                table: "Reviews",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Author_Course",
                table: "Reviews",
                columns: new[] { "AuthorId", "CourseId" },
                unique: true,
                filter: "\"TargetType\" = 'COURSE' AND \"CourseId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Author_Institution",
                table: "Reviews",
                columns: new[] { "AuthorId", "InstitutionId" },
                unique: true,
                filter: "\"TargetType\" = 'INSTITUTION' AND \"InstitutionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Author_Professor",
                table: "Reviews",
                columns: new[] { "AuthorId", "ProfessorId" },
                unique: true,
                filter: "\"TargetType\" = 'PROFESSOR' AND \"ProfessorId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Author_Subject",
                table: "Reviews",
                columns: new[] { "AuthorId", "SubjectId" },
                unique: true,
                filter: "\"TargetType\" = 'SUBJECT' AND \"SubjectId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CourseId",
                table: "Reviews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_InstitutionId",
                table: "Reviews",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProfessorId",
                table: "Reviews",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SubjectId",
                table: "Reviews",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TargetType_Approved",
                table: "Reviews",
                columns: new[] { "TargetType", "Approved" });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CourseId",
                table: "Subjects",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CourseId_Name",
                table: "Subjects",
                columns: new[] { "CourseId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeRequests");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Professors");

            migrationBuilder.DropTable(
                name: "Institutions");
        }
    }
}
