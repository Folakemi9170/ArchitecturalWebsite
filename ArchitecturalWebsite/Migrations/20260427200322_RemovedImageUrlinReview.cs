using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchitecturalWebsite.Migrations
{
    /// <inheritdoc />
    public partial class RemovedImageUrlinReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Reviews");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
