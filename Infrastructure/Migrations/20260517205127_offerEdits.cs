using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class offerEdits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RoomOffers_RoomId",
                table: "RoomOffers",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomOffers_Rooms_RoomId",
                table: "RoomOffers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomOffers_Rooms_RoomId",
                table: "RoomOffers");

            migrationBuilder.DropIndex(
                name: "IX_RoomOffers_RoomId",
                table: "RoomOffers");
        }
    }
}
