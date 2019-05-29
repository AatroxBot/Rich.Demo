using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace article.API.Migrations
{
    public partial class AddArticleReviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "Article",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreateUser",
                table: "Article",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Article",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Article",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "Article",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UpdateUser",
                table: "Article",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "Article");

            migrationBuilder.DropColumn(
                name: "UpdateUser",
                table: "Article");
        }
    }
}
