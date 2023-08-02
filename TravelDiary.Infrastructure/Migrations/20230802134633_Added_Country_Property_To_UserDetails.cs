using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelDiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_Country_Property_To_UserDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserDetails_Country",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDetails_Country",
                table: "Users");
        }
    }
}
