using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaillotStore.Migrations
{
    /// <inheritdoc />
    public partial class AddLeagueIdToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_LeagueId",
                table: "Products",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Leagues_LeagueId",
                table: "Products",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Leagues_LeagueId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_LeagueId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Products");
        }
    }
}
