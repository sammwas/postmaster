using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class RemovePaymentMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMode",
                table: "Receipts",
                newName: "PaymentModeNo");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentModeId",
                table: "Receipts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_PaymentModeId",
                table: "Receipts",
                column: "PaymentModeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_PaymentModes_PaymentModeId",
                table: "Receipts",
                column: "PaymentModeId",
                principalTable: "PaymentModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_PaymentModes_PaymentModeId",
                table: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Receipts_PaymentModeId",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "PaymentModeId",
                table: "Receipts");

            migrationBuilder.RenameColumn(
                name: "PaymentModeNo",
                table: "Receipts",
                newName: "PaymentMode");
        }
    }
}
