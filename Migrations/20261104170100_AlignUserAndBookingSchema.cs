using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieTicketBooking.Migrations
{
    /// <inheritdoc />
    public partial class AlignUserAndBookingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats");

            migrationBuilder.AlterColumn<string>(
                name: "SeatType",
                table: "Seats",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Show_TicketPrice",
                table: "Shows",
                sql: "[TicketPrice] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Movie_DurationMinutes",
                table: "Movies",
                sql: "[DurationMinutes] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BookingSeat_Price",
                table: "BookingSeats",
                sql: "[Price] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Booking_TotalAmount",
                table: "Bookings",
                sql: "[TotalAmount] >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "ScreenId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Show_TicketPrice",
                table: "Shows");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Movie_DurationMinutes",
                table: "Movies");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BookingSeat_Price",
                table: "BookingSeats");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Booking_TotalAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bookings");

            migrationBuilder.AlterColumn<string>(
                name: "SeatType",
                table: "Seats",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Movies",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Screens_ScreenId",
                table: "Seats",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "ScreenId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
