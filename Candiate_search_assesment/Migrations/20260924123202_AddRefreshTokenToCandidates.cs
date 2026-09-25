using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Candiate_search_assesment.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenToCandidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expires_at",
                table: "candidates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token_hash",
                table: "candidates",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "refresh_token_expires_at",
                table: "candidates");

            migrationBuilder.DropColumn(
                name: "refresh_token_hash",
                table: "candidates");
        }
    }
}
