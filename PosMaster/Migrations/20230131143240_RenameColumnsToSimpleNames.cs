using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class RenameColumnsToSimpleNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "ExternalRef",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "ReceiptLineItems");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "KRAPin",
                table: "Receipts",
                newName: "PinNo");

            migrationBuilder.RenameColumn(
                name: "KRAPin",
                table: "Customers",
                newName: "PinNo");

            migrationBuilder.CreateTable(
                name: "ProductPoQuantityLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DeliveredQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateLastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Personnel = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPoQuantityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPoQuantityLogs_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductPoQuantityLogs_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPoQuantityLogs_ProductId",
                table: "ProductPoQuantityLogs",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPoQuantityLogs_PurchaseOrderId",
                table: "ProductPoQuantityLogs",
                column: "PurchaseOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductPoQuantityLogs");

            migrationBuilder.RenameColumn(
                name: "PinNo",
                table: "Receipts",
                newName: "KRAPin");

            migrationBuilder.RenameColumn(
                name: "PinNo",
                table: "Customers",
                newName: "KRAPin");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Receipts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ExternalRef",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "ReceiptLineItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
