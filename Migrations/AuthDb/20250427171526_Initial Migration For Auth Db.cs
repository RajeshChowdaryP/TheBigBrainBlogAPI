using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TheBigBrainBlog.API.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class InitialMigrationForAuthDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "020ea2c2-074c-453e-b318-e8eb9c7a815a", "d0ab1159-7ca9-4dbe-823f-21ad47b444a5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ca7c2635-b497-4423-9370-81e8b1d7a7ad", "d0ab1159-7ca9-4dbe-823f-21ad47b444a5" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "020ea2c2-074c-453e-b318-e8eb9c7a815a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca7c2635-b497-4423-9370-81e8b1d7a7ad");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d0ab1159-7ca9-4dbe-823f-21ad47b444a5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2", "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2", "Reader", "READER" },
                    { "7c94ed92-705d-42cf-9b48-a62277dab155", "7c94ed92-705d-42cf-9b48-a62277dab155", "Writer", "WRITER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "456ea606-94e0-42e8-b901-9ae8187f56f9", 0, "4d0280d0-db83-4417-9027-10f270e7cb18", "admin@thebigbrainblog.com", false, false, null, "ADMIN@THEBIGBRAINBLOG.COM", "ADMIN@THEBIGBRAINBLOG.COM", "AQAAAAIAAYagAAAAEAyFe5BwHSZ9j/vD1kzMNuH7s2d1RZij3sdaGPN48OvCLY4rknv2D4ekgTMzoxnNFQ==", null, false, "b3ed9fbc-0a66-4b69-ad31-e9de3950c510", false, "admin@thebigbrainblog.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2", "456ea606-94e0-42e8-b901-9ae8187f56f9" },
                    { "7c94ed92-705d-42cf-9b48-a62277dab155", "456ea606-94e0-42e8-b901-9ae8187f56f9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2", "456ea606-94e0-42e8-b901-9ae8187f56f9" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "7c94ed92-705d-42cf-9b48-a62277dab155", "456ea606-94e0-42e8-b901-9ae8187f56f9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7c94ed92-705d-42cf-9b48-a62277dab155");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "456ea606-94e0-42e8-b901-9ae8187f56f9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "020ea2c2-074c-453e-b318-e8eb9c7a815a", "020ea2c2-074c-453e-b318-e8eb9c7a815a", "Reader", "READER" },
                    { "ca7c2635-b497-4423-9370-81e8b1d7a7ad", "ca7c2635-b497-4423-9370-81e8b1d7a7ad", "Writer", "WRITER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d0ab1159-7ca9-4dbe-823f-21ad47b444a5", 0, "39bbc480-e591-43ef-a4b7-e344625f876e", "admin@thebigbrainblog.com", false, false, null, "ADMIN@THEBIGBRAINBLOG.COM", "ADMIN@THEBIGBRAINBLOG.COM", "AQAAAAIAAYagAAAAECDMzTsvxbeWcof4VxMQqJyMlnfhVSWtieQcXDrnZFtOujFn6Tw1w+SVr5x9+rjCyA==", null, false, "02f14126-b892-49d6-a4c1-9531752e5e17", false, "admin@thebigbrainblog.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "020ea2c2-074c-453e-b318-e8eb9c7a815a", "d0ab1159-7ca9-4dbe-823f-21ad47b444a5" },
                    { "ca7c2635-b497-4423-9370-81e8b1d7a7ad", "d0ab1159-7ca9-4dbe-823f-21ad47b444a5" }
                });
        }
    }
}
