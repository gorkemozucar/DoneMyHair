using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HairDresser1.Migrations
{
    public partial class hairdresser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HairDresserModel",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SaloonID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HairdresserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HairdresserSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HairdresserPhoneNumber = table.Column<int>(type: "int", nullable: false),
                    HairdresserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HairdresserDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HairDresserModel", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HairDresserModel");
        }
    }
}
