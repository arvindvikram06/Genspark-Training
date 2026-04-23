using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeatHoldModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "SeatNumbers",
                table: "SeatHolds",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(int[]),
                oldType: "integer[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int[]>(
                name: "SeatNumbers",
                table: "SeatHolds",
                type: "integer[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]");
        }
    }
}
