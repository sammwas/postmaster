using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class AddProductRelatedColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.AddColumn<bool>(
                name: "IsService",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceEndDate",
                table: "Products",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceStartDate",
                table: "Products",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProductInstanceStamp",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxTypeId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceEndDate",
                table: "ProductPriceLogs",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceStartDate",
                table: "ProductPriceLogs",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Products_TaxTypeId",
                table: "Products",
                column: "TaxTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TaxTypes_TaxTypeId",
                table: "Products",
                column: "TaxTypeId",
                principalTable: "TaxTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_TaxTypes_TaxTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TaxTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsService",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceEndDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceStartDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductInstanceStamp",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TaxTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceEndDate",
                table: "ProductPriceLogs");

            migrationBuilder.DropColumn(
                name: "PriceStartDate",
                table: "ProductPriceLogs");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AllowDiscount = table.Column<bool>(type: "boolean", nullable: false),
                    AvailableQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    BuyingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateLastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ImagePath = table.Column<string>(type: "text", nullable: true),
                    InstancesId = table.Column<Guid[]>(type: "uuid[]", nullable: true),
                    ItemNature = table.Column<int>(type: "integer", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    Personnel = table.Column<string>(type: "text", nullable: true),
                    ProductCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "numeric", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TaxTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ProductCategories_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProductCategoryId",
                table: "Items",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TaxTypeId",
                table: "Items",
                column: "TaxTypeId");
        }
    }
}
