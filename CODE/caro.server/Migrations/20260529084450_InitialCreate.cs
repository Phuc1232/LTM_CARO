using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace caro.server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    player2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    winner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    match_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    played_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    moves_data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_histories", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_histories");
        }
    }
}
