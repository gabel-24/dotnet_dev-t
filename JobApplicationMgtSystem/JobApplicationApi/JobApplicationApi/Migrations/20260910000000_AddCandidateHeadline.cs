using Microsoft.EntityFrameworkCore.Migrations;

namespace JobApplicationApi.Migrations
{
    public partial class AddCandidateHeadline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Headline", table: "Candidates", type: "longtext",
                nullable: false, defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Headline", table: "Candidates");
        }
    }
}
