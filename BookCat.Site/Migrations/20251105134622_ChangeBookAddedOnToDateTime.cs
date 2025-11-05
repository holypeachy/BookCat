using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCat.Site.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBookAddedOnToDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "AddedOn",
                table: "Books",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("90adc711-8337-468a-a195-8501bac62015"),
                column: "AddedOn",
                value: new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE Books SET AddedOn = DATEADD(hour, 12, CAST(AddedOn AS datetime2))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "AddedOn",
                table: "Books",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("90adc711-8337-468a-a195-8501bac62015"),
                column: "AddedOn",
                value: new DateOnly(2025, 9, 25));
        }
    }
}
