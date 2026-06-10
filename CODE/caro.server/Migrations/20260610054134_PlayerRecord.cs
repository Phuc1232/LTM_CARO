using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caro.server.Migrations
{
    /// <inheritdoc />
    public partial class PlayerRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_records",
                columns: table => new
                {
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    losses = table.Column<int>(type: "integer", nullable: false),
                    draws = table.Column<int>(type: "integer", nullable: false),
                    win_streak = table.Column<int>(type: "integer", nullable: false),
                    max_win_streak = table.Column<int>(type: "integer", nullable: false),
                    shortest_win_moves = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_records", x => x.username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_records");
        }
    }
}
