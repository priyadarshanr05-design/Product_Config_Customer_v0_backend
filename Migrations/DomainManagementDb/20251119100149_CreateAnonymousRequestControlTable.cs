using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Product_Config_Customer_v0.Migrations.DomainManagementDb
{
    /// <inheritdoc />
    public partial class CreateAnonymousRequestControlTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Anonymous_Request_Control",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DomainName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AllowAnonymousRequest = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anonymous_Request_Control", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Anonymous_Request_Control",
                columns: new[] { "Id", "AllowAnonymousRequest", "DateCreated", "DateModified", "DomainName" },
                values: new object[,]
                {
                    { 1, false, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jvl" },
                    { 2, true, new DateTime(2025, 11, 4, 7, 58, 12, 312, DateTimeKind.Unspecified).AddTicks(2640), new DateTime(2025, 11, 17, 11, 26, 16, 175, DateTimeKind.Unspecified).AddTicks(1190), "motor" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anonymous_Request_Control");
        }
    }
}
