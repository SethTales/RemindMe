using Microsoft.EntityFrameworkCore.Migrations;

namespace RemindMe.Data.Migrations
{
    public partial class UpdateReminderSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Reminders");

            migrationBuilder.RenameColumn(
                name: "RecipientEmailAddress",
                table: "Reminders",
                newName: "Message");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Message",
                table: "Reminders",
                newName: "RecipientEmailAddress");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reminders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Reminders",
                nullable: true);
        }
    }
}
