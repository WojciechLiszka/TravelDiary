using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelDiary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changed_Photo_Url_Property_To_FileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Photos",
                newName: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Photos",
                newName: "Url");
        }
    }
}
