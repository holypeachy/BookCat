using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookCat.Site.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_AddedById",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AddedById", "AddedOn", "Author", "CoverUrl", "Description", "GoogleId", "PublishedDate", "Publisher", "Subtitle", "Title" },
                values: new object[] { new Guid("90adc711-8337-468a-a195-8501bac62015"), "f01558ea-6592-4bd4-b938-eabe29da6a89", new DateOnly(2025, 9, 25), "Timothy J. Louwers, Allen D. Blay, Jerry R. Strawser, Jay C. Thibodeau", null, "As auditors, we are trained to investigate beyond appearances to determine the underlying facts-in other words, to look beneath the surface. From the Enron and WorldCom scandals of the early 2000s to the financial crisis of 2007-2008 to present-day issues and challenges related to significant estimation uncertainty, understanding the auditor's responsibility related to fraud, maintaining a clear perspective, probing for details, and understanding the big picture are indispensable to effective auditing. With the availability of greater levels of qualitative and quantitative information (\"Big Data\"), the need for technical skills and challenges facing today's auditor is greater than ever. The Louwers, Bagley, Blay, Strawser, and Thibodeau team has dedicated years of experience in the auditing field to this new edition of Auditing & Assurance Services, supplying the necessary investigative tools for future auditors\"", "3qRuzwEACAAJ", "2023", null, null, "Auditing & Assurance Services" });

            migrationBuilder.InsertData(
                table: "BookIdentifiers",
                columns: new[] { "Id", "BookId", "Type", "Value" },
                values: new object[,]
                {
                    { new Guid("146c6c02-ce45-46b0-b811-39ed5cdea789"), new Guid("90adc711-8337-468a-a195-8501bac62015"), "ISBN_10", "1266796851" },
                    { new Guid("accfc097-bb4b-4577-98dd-07b6880ebb0f"), new Guid("90adc711-8337-468a-a195-8501bac62015"), "ISBN_13", "9781266796852" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AdminDeleted", "BookId", "Comment", "PostedAt", "Rating", "Title", "UserId" },
                values: new object[] { new Guid("b4556383-7b6c-4e39-afd7-df86664b83c7"), false, new Guid("90adc711-8337-468a-a195-8501bac62015"), "This book fucking sucks", new DateTime(2025, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "This is my title", "f01558ea-6592-4bd4-b938-eabe29da6a89" });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

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
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "BookIdentifiers",
                keyColumn: "Id",
                keyValue: new Guid("146c6c02-ce45-46b0-b811-39ed5cdea789"));

            migrationBuilder.DeleteData(
                table: "BookIdentifiers",
                keyColumn: "Id",
                keyValue: new Guid("accfc097-bb4b-4577-98dd-07b6880ebb0f"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: new Guid("b4556383-7b6c-4e39-afd7-df86664b83c7"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("90adc711-8337-468a-a195-8501bac62015"));

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_AddedById",
                table: "Books",
                column: "AddedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
