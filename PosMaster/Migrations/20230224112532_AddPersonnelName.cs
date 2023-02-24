using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddPersonnelName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonnelName",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrintCount",
                table: "Receipts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonnelName",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PrintCount",
                table: "Receipts");
        }
    }
}
