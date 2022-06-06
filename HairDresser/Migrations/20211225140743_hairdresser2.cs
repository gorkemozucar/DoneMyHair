using Microsoft.EntityFrameworkCore.Migrations;

namespace HairDresser1.Migrations
{
    public partial class hairdresser2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HairDresserModel",
                table: "HairDresserModel");

            migrationBuilder.RenameTable(
                name: "HairDresserModel",
                newName: "HairDresser");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HairDresser",
                table: "HairDresser",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HairDresser",
                table: "HairDresser");

            migrationBuilder.RenameTable(
                name: "HairDresser",
                newName: "HairDresserModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HairDresserModel",
                table: "HairDresserModel",
                column: "ID");
        }
    }
}
