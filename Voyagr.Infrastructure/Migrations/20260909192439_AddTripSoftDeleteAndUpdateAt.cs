using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyagr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripSoftDeleteAndUpdateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Trips",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Trips",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Trips",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Trips");
        }
    }
}
