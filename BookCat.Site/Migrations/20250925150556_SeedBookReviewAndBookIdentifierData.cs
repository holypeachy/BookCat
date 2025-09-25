using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCat.Site.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookReviewAndBookIdentifierData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AdminDeleted", "BookId", "Comment", "PostedAt", "Rating", "UserId" },
                values: new object[] { new Guid("b4556383-7b6c-4e39-afd7-df86664b83c7"), false, new Guid("90adc711-8337-468a-a195-8501bac62015"), "This book fucking sucks", new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "b258429f-5fbc-46c0-955e-6a38a64bde61" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: new Guid("b4556383-7b6c-4e39-afd7-df86664b83c7"));
        }
    }
}
