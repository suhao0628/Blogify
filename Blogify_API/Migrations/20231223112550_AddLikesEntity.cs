using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogify_API.Migrations
{
    /// <inheritdoc />
    public partial class AddLikesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityUser_Communities_CommunityId",
                table: "CommunityUser");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityUser_Users_UserId",
                table: "CommunityUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityUser",
                table: "CommunityUser");

            migrationBuilder.RenameTable(
                name: "CommunityUser",
                newName: "CommunityUsers");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityUser_UserId",
                table: "CommunityUsers",
                newName: "IX_CommunityUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityUser_CommunityId",
                table: "CommunityUsers",
                newName: "IX_CommunityUsers_CommunityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityUsers_Communities_CommunityId",
                table: "CommunityUsers",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityUsers_Users_UserId",
                table: "CommunityUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityUsers_Communities_CommunityId",
                table: "CommunityUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityUsers_Users_UserId",
                table: "CommunityUsers");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommunityUsers",
                table: "CommunityUsers");

            migrationBuilder.RenameTable(
                name: "CommunityUsers",
                newName: "CommunityUser");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityUsers_UserId",
                table: "CommunityUser",
                newName: "IX_CommunityUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CommunityUsers_CommunityId",
                table: "CommunityUser",
                newName: "IX_CommunityUser_CommunityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommunityUser",
                table: "CommunityUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityUser_Communities_CommunityId",
                table: "CommunityUser",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityUser_Users_UserId",
                table: "CommunityUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
