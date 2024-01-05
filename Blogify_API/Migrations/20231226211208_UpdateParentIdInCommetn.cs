using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogify_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParentIdInCommetn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCommentId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
