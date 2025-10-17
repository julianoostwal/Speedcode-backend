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
                name: "profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    avatar_url = table.Column<string>(type: "text", nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    nick_name = table.Column<string>(type: "varchar(50)", nullable: true),
                    role = table.Column<string>(type: "text", nullable: false, defaultValue: "user")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profiles", x => x.id);
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
                name: "room_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    roomId = table.Column<Guid>(type: "uuid", nullable: false),
                    profileId = table.Column<Guid>(type: "uuid", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_users_profiles_profileId",
                        column: x => x.profileId,
                        principalTable: "profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_profile_achivements_achievementId",
                table: "profile_achivements",
                column: "achievementId");

            migrationBuilder.CreateIndex(
                name: "IX_room_users_profileId",
                table: "room_users",
                column: "profileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "profile_achivements");

            migrationBuilder.DropTable(
                name: "room_users");

            migrationBuilder.DropTable(
                name: "achievements");

            migrationBuilder.DropTable(
                name: "profiles");
        }
    }
}
