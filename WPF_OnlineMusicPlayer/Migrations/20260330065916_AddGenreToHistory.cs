using Microsoft.EntityFrameworkCore.Migrations;

namespace WPF_OnlineMusicPlayer.Migrations
{
    public partial class AddGenreToHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "ListeningHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "ListeningHistories");
        }
    }
}
