using Microsoft.EntityFrameworkCore.Migrations;

namespace HairDresser1.Migrations
{
    public partial class ownername : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaloonOwnerName",
                table: "Saloon",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaloonOwnerName",
                table: "Saloon");
        }
    }
}
