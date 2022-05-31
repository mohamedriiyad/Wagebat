using Microsoft.EntityFrameworkCore.Migrations;

namespace Wagebat.Data.Migrations
{
    public partial class AddUniversitiesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniversityId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "University",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_University", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_UniversityId",
                table: "Courses",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_University_UniversityId",
                table: "Courses",
                column: "UniversityId",
                principalTable: "University",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_University_UniversityId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "University");

            migrationBuilder.DropIndex(
                name: "IX_Courses_UniversityId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Courses");
        }
    }
}
