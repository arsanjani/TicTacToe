using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Player1Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Player1Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Player1Character = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player2Id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Player2Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Player2Character = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    State = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValue: "WaitingForPlayers"),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "None"),
                    WinnerId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    WinnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalMoves = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_CreatedAt",
                table: "Games",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameId",
                table: "Games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1Id",
                table: "Games",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2Id",
                table: "Games",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_State",
                table: "Games",
                column: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
