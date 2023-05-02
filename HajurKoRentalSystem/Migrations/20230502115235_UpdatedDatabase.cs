using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoRentalSystem.Migrations
{
    public partial class UpdatedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "DamageRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsDamaged",
                table: "Rentals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessDate",
                table: "Rentals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Rentals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDamaged",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "ProcessDate",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Rentals");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageURL",
                table: "DamageRequests",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
