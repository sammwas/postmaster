using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PosMaster.Migrations
{
    public partial class HrTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bank",
                table: "EmployeeSalaries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "EmployeeSalaries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "EmployeeMonthPayments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AltPhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalAddress",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Town",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    ContactPerson = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Counties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Counties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeKins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    PostalAddress = table.Column<string>(type: "text", nullable: true),
                    Town = table.Column<string>(type: "text", nullable: true),
                    EmailAddress = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    AltPhoneNumber = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeKins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeKins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaveCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    MaxDays = table.Column<int>(type: "integer", nullable: false),
                    AllowedGender = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeLeaveCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SalaryFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    SalaryTo = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeSalaryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaveApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationStatus = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    DateFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmployeeLeaveCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Days = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeLeaveApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveApplications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveApplications_EmployeeLeaveCategories_EmployeeL~",
                        column: x => x.EmployeeLeaveCategoryId,
                        principalTable: "EmployeeLeaveCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeLeaveEntitlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    EmployeeLeaveCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RemainingDays = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeLeaveEntitlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveEntitlements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeLeaveEntitlements_EmployeeLeaveCategories_EmployeeL~",
                        column: x => x.EmployeeLeaveCategoryId,
                        principalTable: "EmployeeLeaveCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_UserId",
                table: "EmployeeSalaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeMonthPayments_UserId",
                table: "EmployeeMonthPayments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeKins_UserId",
                table: "EmployeeKins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveApplications_EmployeeLeaveCategoryId",
                table: "EmployeeLeaveApplications",
                column: "EmployeeLeaveCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveApplications_UserId",
                table: "EmployeeLeaveApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveEntitlements_EmployeeLeaveCategoryId",
                table: "EmployeeLeaveEntitlements",
                column: "EmployeeLeaveCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLeaveEntitlements_UserId",
                table: "EmployeeLeaveEntitlements",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryLogs_UserId",
                table: "EmployeeSalaryLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeMonthPayments_AspNetUsers_UserId",
                table: "EmployeeMonthPayments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeSalaries_AspNetUsers_UserId",
                table: "EmployeeSalaries",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeMonthPayments_AspNetUsers_UserId",
                table: "EmployeeMonthPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeSalaries_AspNetUsers_UserId",
                table: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "Counties");

            migrationBuilder.DropTable(
                name: "EmployeeKins");

            migrationBuilder.DropTable(
                name: "EmployeeLeaveApplications");

            migrationBuilder.DropTable(
                name: "EmployeeLeaveEntitlements");

            migrationBuilder.DropTable(
                name: "EmployeeSalaryLogs");

            migrationBuilder.DropTable(
                name: "EmployeeLeaveCategories");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSalaries_UserId",
                table: "EmployeeSalaries");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeMonthPayments_UserId",
                table: "EmployeeMonthPayments");

            migrationBuilder.DropColumn(
                name: "Bank",
                table: "EmployeeSalaries");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "EmployeeSalaries");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmployeeMonthPayments");

            migrationBuilder.DropColumn(
                name: "AltPhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "County",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PostalAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Town",
                table: "AspNetUsers");
        }
    }
}
