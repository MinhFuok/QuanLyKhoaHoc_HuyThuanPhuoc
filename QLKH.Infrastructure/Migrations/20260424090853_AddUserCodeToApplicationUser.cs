using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCodeToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureCard1EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4StartColor",
                table: "SystemSettings");

            migrationBuilder.AddColumn<string>(
                name: "UserCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCode",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1EndColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1MidColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2EndColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2MidColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3EndColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3MidColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4EndColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4MidColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
