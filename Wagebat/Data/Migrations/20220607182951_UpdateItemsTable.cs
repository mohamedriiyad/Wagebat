using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class UpdateItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Items",
                newName: "Without");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Items",
                newName: "With");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Without",
                table: "Items",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "With",
                table: "Items",
                newName: "Name");
        }
    }
}
