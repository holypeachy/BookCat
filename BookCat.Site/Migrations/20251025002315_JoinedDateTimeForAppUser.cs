using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCat.Site.Migrations
{
    /// <inheritdoc />
    public partial class JoinedDateTimeForAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedDateTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "BookIdentifiers",
                keyColumn: "Id",
                keyValue: new Guid("accfc097-bb4b-4577-98dd-07b6880ebb0f"),
                column: "Value",
                value: "978-1266796852");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinedDateTime",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "BookIdentifiers",
                keyColumn: "Id",
                keyValue: new Guid("accfc097-bb4b-4577-98dd-07b6880ebb0f"),
                column: "Value",
                value: "9781266796852");
        }
    }
}
