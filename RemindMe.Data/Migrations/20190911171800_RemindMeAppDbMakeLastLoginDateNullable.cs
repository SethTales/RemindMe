using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RemindMe.Data.Migrations
{
    public partial class RemindMeAppDbMakeLastLoginDateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MostRecentLogin",
                table: "RemindMeUsers",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MostRecentLogin",
                table: "RemindMeUsers",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
