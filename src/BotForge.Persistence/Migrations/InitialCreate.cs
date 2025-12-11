using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotForge.Persistence.Migrations;

/// <summary>
/// Initial migration for BotForge default entities.
/// This migration is applied automatically if no user-defined migrations exist.
/// </summary>
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BotRoles",
            columns: table => new
            {
                Id = table.Column<long>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                Description = table.Column<string>(type: "TEXT", nullable: true),
                WelcomeMessageKey = table.Column<string>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BotRoles", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BotUsers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                RoleId = table.Column<long>(type: "INTEGER", nullable: true),
                PlatformUserId = table.Column<long>(type: "INTEGER", nullable: true),
                Username = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                Discriminator = table.Column<int>(type: "INTEGER", nullable: false),
                DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                PreferredLocale = table.Column<string>(type: "TEXT", nullable: true),
                OriginalLocale = table.Column<string>(type: "TEXT", nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                LastSeen = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BotUsers", x => x.Id);
                table.ForeignKey(
                    name: "FK_BotUsers_BotRoles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "BotRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "UserStates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                StateId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                StateData = table.Column<string>(type: "TEXT", nullable: true),
                UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserStates", x => x.Id);
                table.ForeignKey(
                    name: "FK_UserStates_BotUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "BotUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BotRoles_Name",
            table: "BotRoles",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BotUsers_PlatformUserId",
            table: "BotUsers",
            column: "PlatformUserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BotUsers_Username_Discriminator",
            table: "BotUsers",
            columns: new[] { "Username", "Discriminator" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserStates_UserId",
            table: "UserStates",
            column: "UserId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserStates");

        migrationBuilder.DropTable(
            name: "BotUsers");

        migrationBuilder.DropTable(
            name: "BotRoles");
    }
}

