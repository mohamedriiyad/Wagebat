using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class UpdateItemsAmndPackageItemsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "With",
                table: "Items");

            migrationBuilder.RenameColumn(
                name: "Without",
                table: "Items",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "IsWith",
                table: "PackageItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWith",
                table: "PackageItems");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Items",
                newName: "Without");

            migrationBuilder.AddColumn<string>(
                name: "With",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
