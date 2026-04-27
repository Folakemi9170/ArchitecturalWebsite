using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchitecturalWebsite.Migrations
{
    /// <inheritdoc />
    public partial class RestoredBookingClientFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Bookings");
        }
    }
}
