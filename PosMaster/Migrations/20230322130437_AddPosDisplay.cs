using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddPosDisplay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "GeneralLedgerEntries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "ShowCardPosDisplay",
                table: "ClientInstances",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "GeneralLedgerEntries");

            migrationBuilder.DropColumn(
                name: "ShowCardPosDisplay",
                table: "ClientInstances");
        }
    }
}
