using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToTour.Migrations
{
    public partial class ApplicationUserMigration002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "308660dc-ae51-480f-824d-7dca6714c3e2", "90184155-dee0-40c9-bb1e-b5ed07afc04e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "308660dc-ae51-480f-824d-7dca6714c3e2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "90184155-dee0-40c9-bb1e-b5ed07afc04e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "408660dc-ae51-480f-824d-7dca6714c3e2", "522a1c76-d286-41e9-a982-715ee697f598", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "10184155-dee0-40c9-bb1e-b5ed07afc04e", 0, null, "d57536d5-98e8-43ab-86f7-135c10c2e79a", "admin@totour.com", true, false, null, "ADMIN@TOTOUR.COM", "ADMIN@TOTOUR.COM", "AQAAAAEAACcQAAAAEGnlEyd9XcMnTO3dQ4rLnzekQLRvp2nzHlt86GhOt5/7lmT4isozqyjOgKywaWFpFQ==", "123456789", false, "51fc1a0a-2197-421a-9c77-3e0e9285dea4", false, "admin@totour.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId", "ApplicationUserId" },
                values: new object[] { "408660dc-ae51-480f-824d-7dca6714c3e2", "10184155-dee0-40c9-bb1e-b5ed07afc04e", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "408660dc-ae51-480f-824d-7dca6714c3e2", "10184155-dee0-40c9-bb1e-b5ed07afc04e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "408660dc-ae51-480f-824d-7dca6714c3e2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "10184155-dee0-40c9-bb1e-b5ed07afc04e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "308660dc-ae51-480f-824d-7dca6714c3e2", "85823b3b-273e-41b3-a214-30925659a362", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "90184155-dee0-40c9-bb1e-b5ed07afc04e", 0, null, "d9c4b127-e102-4f37-bc93-c7cd153ccc86", "admin@fakexiecheng.com", true, false, null, "ADMIN@TOTOUR.COM", "ADMIN@TOTOUR.COM", "AQAAAAEAACcQAAAAEJIhM8d/UjZi6yLwn9MBjR4aRwS+56oh5oX0i0xJBk1Zq3EVQdjsbrlXw2g5tdK4+A==", "123456789", false, "3b320a75-d490-472a-a6bb-7ed6e547de53", false, "admin@fakexiecheng.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId", "ApplicationUserId" },
                values: new object[] { "308660dc-ae51-480f-824d-7dca6714c3e2", "90184155-dee0-40c9-bb1e-b5ed07afc04e", null });
        }
    }
}
