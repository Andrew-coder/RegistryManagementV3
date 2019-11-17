using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RegistryManagementV3.Migrations
{
    public partial class AddResourceAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "AuthorId", table: "Resources", nullable: true);
            migrationBuilder.AddForeignKey(name: "FK_Resources_AuthorId", table: "Resources", column: "AuthorId",
                "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Resources_AuthorId", table: "Resources");
            migrationBuilder.DropColumn(name: "AuthorId", "Resources");
        }
    }
}
