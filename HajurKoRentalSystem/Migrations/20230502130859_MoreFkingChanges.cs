using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoRentalSystem.Migrations
{
    public partial class MoreFkingChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDate",
                table: "DamageRequests",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDate",
                table: "DamageRequests");
        }
    }
}
