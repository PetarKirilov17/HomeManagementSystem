using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeManagementSystem.Data.Migrations
{
    public partial class AddingIdes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Locations_LocationId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Locations_LocationId",
                table: "Assignments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Locations_LocationId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Locations_LocationId",
                table: "Assignments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
