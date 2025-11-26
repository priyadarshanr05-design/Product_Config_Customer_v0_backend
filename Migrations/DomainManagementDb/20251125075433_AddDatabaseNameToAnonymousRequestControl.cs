using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Product_Config_Customer_v0.Migrations.DomainManagementDb
{
    /// <inheritdoc />
    public partial class AddDatabaseNameToAnonymousRequestControl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "AnonymousRequestControls",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AnonymousRequestControls",
                keyColumn: "Id",
                keyValue: 1,
                column: "DatabaseName",
                value: "CustDb_Jvl");

            migrationBuilder.UpdateData(
                table: "AnonymousRequestControls",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DatabaseName", "DomainName" },
                values: new object[] { "CustDb_Motor", "Motor" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "AnonymousRequestControls");

            migrationBuilder.UpdateData(
                table: "AnonymousRequestControls",
                keyColumn: "Id",
                keyValue: 2,
                column: "DomainName",
                value: "motor");
        }
    }
}
