using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLogs_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeleteMessageUpdates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeleteMessageUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeleteMessageUpdates_Updates_Id",
                        column: x => x.Id,
                        principalTable: "Updates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MessageUpdates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "varchar(4096)", maxLength: 4096, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TextEntities = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrevEditId = table.Column<long>(type: "bigint", nullable: true),
                    MsgDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageUpdates_MessageUpdates_PrevEditId",
                        column: x => x.PrevEditId,
                        principalTable: "MessageUpdates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MessageUpdates_Updates_Id",
                        column: x => x.Id,
                        principalTable: "Updates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MessageUpdates_PrevEditId",
                table: "MessageUpdates",
                column: "PrevEditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeleteMessageUpdates");

            migrationBuilder.DropTable(
                name: "MessageUpdates");
        }
    }
}
