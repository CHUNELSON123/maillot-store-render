using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MaillotStore.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaguesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add the column (defaults to 0, which is temporarily invalid for FK)
            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // 2. Create the Leagues table
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.Id);
                });

            // ==============================================================================
            // FIX START: Insert a default league and assign it to existing teams
            // ==============================================================================

            // Create a default league named "Other" so we have an ID to link to
            migrationBuilder.Sql("INSERT INTO \"Leagues\" (\"Name\") VALUES ('Other');");

            // Update all existing teams (which have LeagueId=0) to link to the new "Other" league
            migrationBuilder.Sql("UPDATE \"Teams\" SET \"LeagueId\" = (SELECT \"Id\" FROM \"Leagues\" WHERE \"Name\" = 'Other' LIMIT 1);");

            // ==============================================================================
            // FIX END
            // ==============================================================================

            // 3. Create Index
            migrationBuilder.CreateIndex(
                name: "IX_Teams_LeagueId",
                table: "Teams",
                column: "LeagueId");

            // 4. Add the Foreign Key (Now safe because all teams have a valid LeagueId)
            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams",
                column: "LeagueId",
                principalTable: "Leagues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Leagues_LeagueId",
                table: "Teams");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropIndex(
                name: "IX_Teams_LeagueId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                table: "Teams");
        }
    }
}