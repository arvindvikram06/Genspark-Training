using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Routes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$mC7GUEVvYnO6.iZg8L7eD.U8U8U8U8U8U8U8U8U8U8U8U8U8U8U8U");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Routes");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGl29uS/5V3G56z6Vv6W==");
        }
    }
}
