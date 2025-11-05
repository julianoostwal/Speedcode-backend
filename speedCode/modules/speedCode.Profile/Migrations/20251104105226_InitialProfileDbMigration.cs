using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace speedCode.Profile.Migrations
{
    /// <inheritdoc />
    public partial class InitialProfileDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "achievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    achievementIcon = table.Column<string>(type: "text", nullable: false),
                    achievementName = table.Column<string>(type: "text", nullable: false),
                    achievementDescription = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_achievements", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isRoomExcise = table.Column<bool>(type: "boolean", nullable: false),
                    isPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercises", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    avatarUrl = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    nickName = table.Column<string>(type: "varchar(50)", nullable: true),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "user")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "completed_profile_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    profileId = table.Column<Guid>(type: "uuid", nullable: false),
                    memory = table.Column<double>(type: "double precision", nullable: false),
                    time = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_completed_profile_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_completed_profile_exercises_exercises_exerciseId",
                        column: x => x.exerciseId,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_completed_profile_exercises_profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "created_profile_exercises",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    profileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_created_profile_exercises", x => x.id);
                    table.ForeignKey(
                        name: "FK_created_profile_exercises_exercises_exerciseId",
                        column: x => x.exerciseId,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_created_profile_exercises_profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "profile_achivements",
                columns: table => new
                {
                    achievementId = table.Column<Guid>(type: "uuid", nullable: false),
                    profileId = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_achivements", x => new { x.profileId, x.achievementId });
                    table.ForeignKey(
                        name: "FK_profile_achivements_achievements_achievementId",
                        column: x => x.achievementId,
                        principalTable: "achievements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_profile_achivements_profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testCode",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    AC_regex = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Profilesid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testCode", x => x.id);
                    table.ForeignKey(
                        name: "FK_testCode_profiles_Profilesid",
                        column: x => x.Profilesid,
                        principalTable: "profiles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "exercise_tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    exerciseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_tests", x => x.id);
                    table.ForeignKey(
                        name: "FK_exercise_tests_exercises_exerciseId",
                        column: x => x.exerciseId,
                        principalTable: "exercises",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exercise_tests_testCode_testId",
                        column: x => x.testId,
                        principalTable: "testCode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "testExpectIO",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    input = table.Column<string>(type: "text", nullable: true),
                    output = table.Column<string>(type: "text", nullable: false),
                    testCodeId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testExpectIO", x => x.id);
                    table.ForeignKey(
                        name: "FK_testExpectIO_testCode_testCodeId",
                        column: x => x.testCodeId,
                        principalTable: "testCode",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_completed_profile_exercises_exerciseId",
                table: "completed_profile_exercises",
                column: "exerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_completed_profile_exercises_profileId",
                table: "completed_profile_exercises",
                column: "profileId");

            migrationBuilder.CreateIndex(
                name: "IX_created_profile_exercises_exerciseId",
                table: "created_profile_exercises",
                column: "exerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_created_profile_exercises_profileId",
                table: "created_profile_exercises",
                column: "profileId");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_tests_exerciseId",
                table: "exercise_tests",
                column: "exerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_tests_testId",
                table: "exercise_tests",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_profile_achivements_achievementId",
                table: "profile_achivements",
                column: "achievementId");

            migrationBuilder.CreateIndex(
                name: "IX_testCode_Profilesid",
                table: "testCode",
                column: "Profilesid");

            migrationBuilder.CreateIndex(
                name: "IX_testExpectIO_testCodeId",
                table: "testExpectIO",
                column: "testCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "completed_profile_exercises");

            migrationBuilder.DropTable(
                name: "created_profile_exercises");

            migrationBuilder.DropTable(
                name: "exercise_tests");

            migrationBuilder.DropTable(
                name: "profile_achivements");

            migrationBuilder.DropTable(
                name: "testExpectIO");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "testCode");

            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
