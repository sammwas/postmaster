using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddInvoicePaidAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceTerms",
                table: "Clients");

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Invoices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceDurationDays",
                table: "ClientInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTerms",
                table: "ClientInstances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PinNo",
                table: "ClientInstances",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceDurationDays",
                table: "ClientInstances");

            migrationBuilder.DropColumn(
                name: "InvoiceTerms",
                table: "ClientInstances");

            migrationBuilder.DropColumn(
                name: "PinNo",
                table: "ClientInstances");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceTerms",
                table: "Clients",
                type: "text",
                nullable: true);
        }
    }
}
