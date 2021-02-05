using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class ModifyToReusePoGrnProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "PoGrnProducts",
                newName: "PoUnitPrice");

            migrationBuilder.RenameColumn(
                name: "PoId",
                table: "PoGrnProducts",
                newName: "PurchaseOrderId");

            migrationBuilder.RenameColumn(
                name: "OrderedQuantity",
                table: "PoGrnProducts",
                newName: "PoQuantity");

            migrationBuilder.RenameColumn(
                name: "GrnId",
                table: "PoGrnProducts",
                newName: "GoodReceivedNoteId");

            migrationBuilder.RenameColumn(
                name: "DeliveredQuantity",
                table: "PoGrnProducts",
                newName: "GrnUnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "GrnNotes",
                table: "PoGrnProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrnQuantity",
                table: "PoGrnProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PoNotes",
                table: "PoGrnProducts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrnNotes",
                table: "PoGrnProducts");

            migrationBuilder.DropColumn(
                name: "GrnQuantity",
                table: "PoGrnProducts");

            migrationBuilder.DropColumn(
                name: "PoNotes",
                table: "PoGrnProducts");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderId",
                table: "PoGrnProducts",
                newName: "PoId");

            migrationBuilder.RenameColumn(
                name: "PoUnitPrice",
                table: "PoGrnProducts",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "PoQuantity",
                table: "PoGrnProducts",
                newName: "OrderedQuantity");

            migrationBuilder.RenameColumn(
                name: "GrnUnitPrice",
                table: "PoGrnProducts",
                newName: "DeliveredQuantity");

            migrationBuilder.RenameColumn(
                name: "GoodReceivedNoteId",
                table: "PoGrnProducts",
                newName: "GrnId");
        }
    }
}
