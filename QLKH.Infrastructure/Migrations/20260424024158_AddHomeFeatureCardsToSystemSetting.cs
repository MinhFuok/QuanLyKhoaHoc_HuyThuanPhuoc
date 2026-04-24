using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLKH.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeFeatureCardsToSystemSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1Description",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

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
                name: "FeatureCard1Number",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard1Title",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2Description",
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
                name: "FeatureCard2Number",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard2Title",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3Description",
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
                name: "FeatureCard3Number",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard3Title",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4Description",
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
                name: "FeatureCard4Number",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4StartColor",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureCard4Title",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeatureCard1Description",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1Number",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard1Title",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2Description",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2Number",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard2Title",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3Description",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3Number",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard3Title",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4Description",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4EndColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4MidColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4Number",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4StartColor",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "FeatureCard4Title",
                table: "SystemSettings");
        }
    }
}
