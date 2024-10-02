using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class EnhanceExpenseAsPv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountReceived",
                table: "GoodReceivedNotes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ModeNumber",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentModeId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "Expenses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowClerkDashboard",
                table: "Clients",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentModeId",
                table: "Expenses",
                column: "PaymentModeId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SupplierId",
                table: "Expenses",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_PaymentModes_PaymentModeId",
                table: "Expenses",
                column: "PaymentModeId",
                principalTable: "PaymentModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Suppliers_SupplierId",
                table: "Expenses",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_PaymentModes_PaymentModeId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Suppliers_SupplierId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaymentModeId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SupplierId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AmountReceived",
                table: "GoodReceivedNotes");

            migrationBuilder.DropColumn(
                name: "ModeNumber",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaymentModeId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ShowClerkDashboard",
                table: "Clients");
        }
    }
}
