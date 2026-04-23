using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBusModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledFrom",
                table: "Buses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledTo",
                table: "Buses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Buses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledFrom",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "DisabledTo",
                table: "Buses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Buses");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "Phone", "Role" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@busbooking.com", true, "System Admin", "$2a$11$mC7GUEVvYnO6.iZg8L7eD.U8U8U8U8U8U8U8U8U8U8U8U8U8U8U8U", "1234567890", 2 });
        }
    }
}
