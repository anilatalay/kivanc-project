using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebCheckerAPI.Migrations
{
    /// <inheritdoc />
    public partial class Added_LastRequestDate_In_Ednpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Time",
                table: "Endpoints",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRequestDate",
                table: "Endpoints",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRequestDate",
                table: "Endpoints");

            migrationBuilder.AlterColumn<int>(
                name: "Time",
                table: "Endpoints",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");
        }
    }
}
