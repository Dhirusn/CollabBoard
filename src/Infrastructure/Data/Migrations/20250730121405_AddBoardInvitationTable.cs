using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollabBoard.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardInvitationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardsMember_AspNetUsers_UserId",
                table: "BoardsMember");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardsMember_Boards_BoardId",
                table: "BoardsMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardsMember",
                table: "BoardsMember");

            migrationBuilder.RenameTable(
                name: "BoardsMember",
                newName: "BoardsMembers");

            migrationBuilder.RenameIndex(
                name: "IX_BoardsMember_UserId",
                table: "BoardsMembers",
                newName: "IX_BoardsMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BoardsMember_BoardId_UserId",
                table: "BoardsMembers",
                newName: "IX_BoardsMembers_BoardId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardsMembers",
                table: "BoardsMembers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BoardInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    InvitedByUserId = table.Column<string>(type: "text", nullable: false),
                    TargetUserId = table.Column<string>(type: "text", nullable: false),
                    RequestedRole = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RespondedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardInvitations_AspNetUsers_InvitedByUserId",
                        column: x => x.InvitedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardInvitations_AspNetUsers_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardInvitations_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardInvitation_Board_Target_Status",
                table: "BoardInvitations",
                columns: new[] { "BoardId", "TargetUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BoardInvitations_InvitedByUserId",
                table: "BoardInvitations",
                column: "InvitedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardInvitations_TargetUserId",
                table: "BoardInvitations",
                column: "TargetUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardsMembers_AspNetUsers_UserId",
                table: "BoardsMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BoardsMembers_Boards_BoardId",
                table: "BoardsMembers",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardsMembers_AspNetUsers_UserId",
                table: "BoardsMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_BoardsMembers_Boards_BoardId",
                table: "BoardsMembers");

            migrationBuilder.DropTable(
                name: "BoardInvitations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardsMembers",
                table: "BoardsMembers");

            migrationBuilder.RenameTable(
                name: "BoardsMembers",
                newName: "BoardsMember");

            migrationBuilder.RenameIndex(
                name: "IX_BoardsMembers_UserId",
                table: "BoardsMember",
                newName: "IX_BoardsMember_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BoardsMembers_BoardId_UserId",
                table: "BoardsMember",
                newName: "IX_BoardsMember_BoardId_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardsMember",
                table: "BoardsMember",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardsMember_AspNetUsers_UserId",
                table: "BoardsMember",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BoardsMember_Boards_BoardId",
                table: "BoardsMember",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
