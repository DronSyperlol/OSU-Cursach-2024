using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class PrevTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PrevTargetId",
                table: "Targets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Targets_PrevTargetId",
                table: "Targets",
                column: "PrevTargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Targets_Targets_PrevTargetId",
                table: "Targets",
                column: "PrevTargetId",
                principalTable: "Targets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Targets_Targets_PrevTargetId",
                table: "Targets");

            migrationBuilder.DropIndex(
                name: "IX_Targets_PrevTargetId",
                table: "Targets");

            migrationBuilder.DropColumn(
                name: "PrevTargetId",
                table: "Targets");
        }
    }
}
