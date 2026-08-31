using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobApplicationApi.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewDetailsToInterviewStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "InterviewStages");

            migrationBuilder.AddColumn<string>(
                name: "Interviewer",
                table: "InterviewStages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "InterviewStages",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "InterviewStages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "InterviewStages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interviewer",
                table: "InterviewStages");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "InterviewStages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InterviewStages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "InterviewStages");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "InterviewStages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
