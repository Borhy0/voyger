using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voyagr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteCurrencyPairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteCurrencyPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromCurrency = table.Column<string>(type: "text", nullable: false),
                    ToCurrency = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteCurrencyPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteCurrencyPairs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteCurrencyPairs_UserId_FromCurrency_ToCurrency",
                table: "FavoriteCurrencyPairs",
                columns: new[] { "UserId", "FromCurrency", "ToCurrency" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteCurrencyPairs");
        }
    }
}
