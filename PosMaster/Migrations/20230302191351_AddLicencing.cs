using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddLicencing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPoQuantityLogs_PurchaseOrders_PurchaseOrderId",
                table: "ProductPoQuantityLogs");

            migrationBuilder.DropColumn(
                name: "EnforcePassword",
                table: "Clients");

            migrationBuilder.AddColumn<string>(
                name: "Stamp",
                table: "Receipts",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseOrderId",
                table: "ProductPoQuantityLogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Licence",
                table: "Clients",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxInstance",
                table: "Clients",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_Stamp",
                table: "Receipts",
                column: "Stamp",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdNumber",
                table: "AspNetUsers",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPoQuantityLogs_PurchaseOrders_PurchaseOrderId",
                table: "ProductPoQuantityLogs",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPoQuantityLogs_PurchaseOrders_PurchaseOrderId",
                table: "ProductPoQuantityLogs");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_Stamp",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdNumber",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Stamp",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "Licence",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "MaxInstance",
                table: "Clients");

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseOrderId",
                table: "ProductPoQuantityLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnforcePassword",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPoQuantityLogs_PurchaseOrders_PurchaseOrderId",
                table: "ProductPoQuantityLogs",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
