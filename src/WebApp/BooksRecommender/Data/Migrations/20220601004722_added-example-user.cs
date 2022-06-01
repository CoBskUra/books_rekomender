using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksRecommender.Data.Migrations
{
    public partial class addedexampleuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b8157f05-3a48-427c-93e8-b67285e4ee59", 0, "53f9b632-8ba0-4d2d-ab17-0eabddce3091", "user@book.com", true, false, null, null, null, null, null, false, "d96388f7-cb9b-44cf-807d-eb0ec38aee1f", false, "user@book.com" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b8157f05-3a48-427c-93e8-b67285e4ee59");
        }
    }
}
