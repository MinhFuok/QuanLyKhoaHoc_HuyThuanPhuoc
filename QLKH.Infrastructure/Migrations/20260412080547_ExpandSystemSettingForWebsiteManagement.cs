using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSystemSettingForWebsiteManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactAddress",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FooterText",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HomeBannerSubtitle",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWebsiteEnabled",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceMessage",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAddress",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FooterText",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "HomeBannerSubtitle",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "IsWebsiteEnabled",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "MaintenanceMessage",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "SystemSettings");
        }
    }
}
