using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiplomProject.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScienceEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEvent = table.Column<string>(type: "text", nullable: false),
                    DateEvent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlaceEvent = table.Column<string>(type: "text", nullable: false),
                    RequirementsEvent = table.Column<string>(type: "text", nullable: false),
                    InformationEvent = table.Column<string>(type: "text", nullable: false),
                    DateEventCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AddByAdminChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScienceEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    TgChatId = table.Column<long>(type: "bigint", nullable: false),
                    IsSubscribed = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    HashedPassword = table.Column<string>(type: "text", nullable: true),
                    LastMessageTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCreatedEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NameEvent = table.Column<string>(type: "text", nullable: false),
                    PlaceEvent = table.Column<string>(type: "text", nullable: false),
                    DateEvent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TgUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsWinner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCreatedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCreatedEvents_TelegramUsers_TgUserId",
                        column: x => x.TgUserId,
                        principalTable: "TelegramUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCreatedEvents_TgUserId",
                table: "UserCreatedEvents",
                column: "TgUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScienceEvents");

            migrationBuilder.DropTable(
                name: "UserCreatedEvents");

            migrationBuilder.DropTable(
                name: "TelegramUsers");
        }
    }
}
