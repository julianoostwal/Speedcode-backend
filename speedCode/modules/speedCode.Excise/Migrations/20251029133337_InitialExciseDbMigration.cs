using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace speedCode.Excise.Migrations
{
    /// <inheritdoc />
    public partial class InitialExciseDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "testCode",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    AC_regex = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_testCode", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "exercise_tests",
                columns: table => new
                {
                    testId = table.Column<Guid>(type: "uuid", nullable: false),
                    exerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercise_tests", x => new { x.exerciseId, x.testId });
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
                name: "IX_exercise_tests_testId",
                table: "exercise_tests",
                column: "testId");

            migrationBuilder.CreateIndex(
                name: "IX_testExpectIO_testCodeId",
                table: "testExpectIO",
                column: "testCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exercise_tests");

            migrationBuilder.DropTable(
                name: "testExpectIO");

            migrationBuilder.DropTable(
                name: "exercises");

            migrationBuilder.DropTable(
                name: "testCode");
        }
    }
}
