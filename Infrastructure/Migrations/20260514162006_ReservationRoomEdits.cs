using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReservationRoomEdits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationID",
                table: "ReservationRooms");

            migrationBuilder.RenameColumn(
                name: "ReservationID",
                table: "ReservationRooms",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationRooms_ReservationID",
                table: "ReservationRooms",
                newName: "IX_ReservationRooms_ReservationId");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "ReservationRooms",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationId",
                table: "ReservationRooms");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "ReservationRooms",
                newName: "ReservationID");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationRooms_ReservationId",
                table: "ReservationRooms",
                newName: "IX_ReservationRooms_ReservationID");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationID",
                table: "ReservationRooms",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRooms_Reservations_ReservationID",
                table: "ReservationRooms",
                column: "ReservationID",
                principalTable: "Reservations",
                principalColumn: "ID");
        }
    }
}
