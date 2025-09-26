using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCat.Site.Migrations
{
    /// <inheritdoc />
    public partial class AddedByIdAndAddedByInBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddedById",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("90adc711-8337-468a-a195-8501bac62015"),
                column: "AddedById",
                value: "b258429f-5fbc-46c0-955e-6a38a64bde61");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AddedById",
                table: "Books",
                column: "AddedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_AddedById",
                table: "Books",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_AddedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AddedById",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AddedById",
                table: "Books");
        }
    }
}
