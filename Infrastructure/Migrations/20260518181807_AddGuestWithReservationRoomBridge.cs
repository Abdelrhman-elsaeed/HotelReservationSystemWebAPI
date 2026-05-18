using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestWithReservationRoomBridge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_ReservationRooms_ReservationRoomID",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_ReservationRoomID",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "ReservationRoomID",
                table: "Guests");

            migrationBuilder.CreateTable(
                name: "GuestReservationRoom",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestId = table.Column<int>(type: "int", nullable: false),
                    ReservationRoomId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestReservationRoom", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GuestReservationRoom_Guests_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Guests",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GuestReservationRoom_ReservationRooms_ReservationRoomId",
                        column: x => x.ReservationRoomId,
                        principalTable: "ReservationRooms",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservationRoom_GuestId",
                table: "GuestReservationRoom",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestReservationRoom_ReservationRoomId",
                table: "GuestReservationRoom",
                column: "ReservationRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestReservationRoom");

            migrationBuilder.AddColumn<int>(
                name: "ReservationRoomID",
                table: "Guests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guests_ReservationRoomID",
                table: "Guests",
                column: "ReservationRoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_ReservationRooms_ReservationRoomID",
                table: "Guests",
                column: "ReservationRoomID",
                principalTable: "ReservationRooms",
                principalColumn: "ID");
        }
    }
}
