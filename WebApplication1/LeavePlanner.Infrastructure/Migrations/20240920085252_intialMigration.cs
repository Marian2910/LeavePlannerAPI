using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeavePlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class intialMigration : Migration
    {
        private const string SqlServerIdentityAnnotation = "SqlServer:Identity";
        private const string NVarCharMaxColumnType = "nvarchar(max)";
        private const string DateTime2ColumnType = "datetime2";
        private const string EmployeesTableName = "Employees";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    Name = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Email = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    PhoneNumber = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Country = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    City = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    PostalCode = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Street = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Number = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    BillingType = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Tva = table.Column<int>(type: "int", nullable: false),
                    Addition = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Date = table.Column<DateTime>(type: DateTime2ColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    Name = table.Column<string>(type: NVarCharMaxColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    Title = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Role = table.Column<string>(type: NVarCharMaxColumnType, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    Name = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Type = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Date = table.Column<DateTime>(type: DateTime2ColumnType, nullable: false),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: EmployeesTableName,
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    FirstName = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    LastName = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Email = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Password = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Birthdate = table.Column<DateTime>(type: DateTime2ColumnType, nullable: false),
                    EmploymentDate = table.Column<DateTime>(type: DateTime2ColumnType, nullable: false),
                    RemainingLeaveDays = table.Column<int>(type: "int", nullable: false),
                    AnnualLeaveDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation(SqlServerIdentityAnnotation, "1, 1"),
                    Title = table.Column<string>(type: NVarCharMaxColumnType, nullable: false),
                    Description = table.Column<string>(type: NVarCharMaxColumnType, nullable: true),
                    StartDate = table.Column<DateTime>(type: DateTime2ColumnType, nullable: false),
                    EndDate = table.Column<DateTime>(type: DateTime2ColumnType, nullable: true),
                    Location = table.Column<string>(type: NVarCharMaxColumnType, nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: EmployeesTableName,
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CustomerId",
                table: "Documents",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: EmployeesTableName,
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_JobId",
                table: EmployeesTableName,
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EmployeeId",
                table: "Events",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: EmployeesTableName);

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
