using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class AddNewPrimaryKeyToPackageItemsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageItems",
                table: "PackageItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageItems",
                table: "PackageItems",
                columns: new[] { "PackageId", "ItemId", "IsWith" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageItems",
                table: "PackageItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageItems",
                table: "PackageItems",
                columns: new[] { "PackageId", "ItemId" });
        }
    }
}
