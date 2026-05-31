using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GonulluOlTarsus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventCancellation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IptalEdildi",
                table: "Etkinlikler",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IptalTarihi",
                table: "Etkinlikler",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IptalEdildi",
                table: "Etkinlikler");

            migrationBuilder.DropColumn(
                name: "IptalTarihi",
                table: "Etkinlikler");
        }
    }
}
