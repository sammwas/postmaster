using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class ReuseReceiptTaxForInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Invoices");

            migrationBuilder.CreateIndex(
                name: "IX_PoGrnProducts_GoodReceivedNoteId",
                table: "PoGrnProducts",
                column: "GoodReceivedNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts",
                column: "GoodReceivedNoteId",
                principalTable: "GoodReceivedNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts");

            migrationBuilder.DropIndex(
                name: "IX_PoGrnProducts_GoodReceivedNoteId",
                table: "PoGrnProducts");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Receipts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Invoices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
