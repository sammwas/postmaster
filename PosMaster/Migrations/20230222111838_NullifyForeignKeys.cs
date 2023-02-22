using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class NullifyForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PoGrnProducts_PurchaseOrders_PurchaseOrderId",
                table: "PoGrnProducts");

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseOrderId",
                table: "PoGrnProducts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "GoodReceivedNoteId",
                table: "PoGrnProducts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "BusinessShortCode",
                table: "ClientInstances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessShortCodeType",
                table: "ClientInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptFooterNotes",
                table: "ClientInstances",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts",
                column: "GoodReceivedNoteId",
                principalTable: "GoodReceivedNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PoGrnProducts_PurchaseOrders_PurchaseOrderId",
                table: "PoGrnProducts",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_PoGrnProducts_PurchaseOrders_PurchaseOrderId",
                table: "PoGrnProducts");

            migrationBuilder.DropColumn(
                name: "BusinessShortCode",
                table: "ClientInstances");

            migrationBuilder.DropColumn(
                name: "BusinessShortCodeType",
                table: "ClientInstances");

            migrationBuilder.DropColumn(
                name: "ReceiptFooterNotes",
                table: "ClientInstances");

            migrationBuilder.AlterColumn<Guid>(
                name: "PurchaseOrderId",
                table: "PoGrnProducts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GoodReceivedNoteId",
                table: "PoGrnProducts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PoGrnProducts_GoodReceivedNotes_GoodReceivedNoteId",
                table: "PoGrnProducts",
                column: "GoodReceivedNoteId",
                principalTable: "GoodReceivedNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PoGrnProducts_PurchaseOrders_PurchaseOrderId",
                table: "PoGrnProducts",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
