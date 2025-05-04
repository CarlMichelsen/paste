using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.Migrations
{
    /// <inheritdoc />
    public partial class PasteInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "paste");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "paste",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    authentication_method = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    authentication_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    first_login_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "file",
                schema: "paste",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<long>(type: "bigint", nullable: false),
                    mime_type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_file", x => x.id);
                    table.ForeignKey(
                        name: "fk_file_user_owner_id",
                        column: x => x.owner_id,
                        principalSchema: "paste",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "action",
                schema: "paste",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checksum_algorithm = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    checksum = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    action = table.Column<string>(type: "text", nullable: false),
                    performed_by_id = table.Column<long>(type: "bigint", nullable: false),
                    performed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_action", x => x.id);
                    table.ForeignKey(
                        name: "fk_action_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "paste",
                        principalTable: "file",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_action_user_performed_by_id",
                        column: x => x.performed_by_id,
                        principalSchema: "paste",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "content",
                schema: "paste",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_file_file_id",
                        column: x => x.file_id,
                        principalSchema: "paste",
                        principalTable: "file",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_action_file_id",
                schema: "paste",
                table: "action",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "ix_action_performed_by_id",
                schema: "paste",
                table: "action",
                column: "performed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_file_id",
                schema: "paste",
                table: "content",
                column: "file_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_name_owner_id",
                schema: "paste",
                table: "file",
                columns: new[] { "name", "owner_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_file_owner_id",
                schema: "paste",
                table: "file",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action",
                schema: "paste");

            migrationBuilder.DropTable(
                name: "content",
                schema: "paste");

            migrationBuilder.DropTable(
                name: "file",
                schema: "paste");

            migrationBuilder.DropTable(
                name: "user",
                schema: "paste");
        }
    }
}
