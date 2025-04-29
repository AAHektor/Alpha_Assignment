using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceClientNameWithClientId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "Clients",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_ClientName",
                table: "Clients",
                newName: "IX_Clients_ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Clients",
                newName: "ClientName");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_ClientId",
                table: "Clients",
                newName: "IX_Clients_ClientName");
        }
    }
}
