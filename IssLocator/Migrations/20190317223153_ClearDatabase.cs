using Microsoft.EntityFrameworkCore.Migrations;

namespace IssLocator.Migrations
{
    public partial class ClearDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [IssTrackPoints]", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
