using Microsoft.EntityFrameworkCore.Migrations;

namespace server.Migrations
{
    public partial class add_relation_user_act : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Avtivity_Users_UserId",
                table: "Avtivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Avtivity",
                table: "Avtivity");

            migrationBuilder.RenameTable(
                name: "Avtivity",
                newName: "Activity");

            migrationBuilder.RenameIndex(
                name: "IX_Avtivity_UserId",
                table: "Activity",
                newName: "IX_Activity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activity",
                table: "Activity",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activity_Users_UserId",
                table: "Activity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activity_Users_UserId",
                table: "Activity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activity",
                table: "Activity");

            migrationBuilder.RenameTable(
                name: "Activity",
                newName: "Avtivity");

            migrationBuilder.RenameIndex(
                name: "IX_Activity_UserId",
                table: "Avtivity",
                newName: "IX_Avtivity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Avtivity",
                table: "Avtivity",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Avtivity_Users_UserId",
                table: "Avtivity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
