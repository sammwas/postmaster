using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddGoodsReceived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLineItem_Products_ProductId",
                table: "ReceiptLineItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLineItem_Receipts_ReceiptId",
                table: "ReceiptLineItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptLineItem",
                table: "ReceiptLineItem");

            migrationBuilder.RenameTable(
                name: "ReceiptLineItem",
                newName: "ReceiptLineItems");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptLineItem_ReceiptId",
                table: "ReceiptLineItems",
                newName: "IX_ReceiptLineItems_ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptLineItem_ProductId",
                table: "ReceiptLineItems",
                newName: "IX_ReceiptLineItems_ProductId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PasswordChangeDate",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptLineItems",
                table: "ReceiptLineItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "GoodReturnedNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_GoodReturnedNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodReturnedNotes_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ClientId",
                table: "AspNetUsers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodReturnedNotes_ReceiptId",
                table: "GoodReturnedNotes",
                column: "ReceiptId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clients_ClientId",
                table: "AspNetUsers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLineItems_Products_ProductId",
                table: "ReceiptLineItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLineItems_Receipts_ReceiptId",
                table: "ReceiptLineItems",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clients_ClientId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLineItems_Products_ProductId",
                table: "ReceiptLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLineItems_Receipts_ReceiptId",
                table: "ReceiptLineItems");

            migrationBuilder.DropTable(
                name: "GoodReturnedNotes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ClientId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptLineItems",
                table: "ReceiptLineItems");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "ReceiptLineItems",
                newName: "ReceiptLineItem");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptLineItems_ReceiptId",
                table: "ReceiptLineItem",
                newName: "IX_ReceiptLineItem_ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptLineItems_ProductId",
                table: "ReceiptLineItem",
                newName: "IX_ReceiptLineItem_ProductId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PasswordChangeDate",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptLineItem",
                table: "ReceiptLineItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLineItem_Products_ProductId",
                table: "ReceiptLineItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLineItem_Receipts_ReceiptId",
                table: "ReceiptLineItem",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
