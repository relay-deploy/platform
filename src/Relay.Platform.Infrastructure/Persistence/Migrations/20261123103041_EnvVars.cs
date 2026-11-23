using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnvVars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "env_vars",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    app_id = table.Column<Guid>(type: "uuid", nullable: true),
                    project_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    is_secret = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_env_vars", x => x.id);
                    table.ForeignKey(
                        name: "fk_env_vars_apps_app_id",
                        column: x => x.app_id,
                        principalSchema: "platform",
                        principalTable: "apps",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_env_vars_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "platform",
                        principalTable: "projects",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_env_vars_app_id_project_id_name",
                schema: "platform",
                table: "env_vars",
                columns: new[] { "app_id", "project_id", "name" },
                unique: true)
                .Annotation("Npgsql:NullsDistinct", false);

            migrationBuilder.CreateIndex(
                name: "ix_env_vars_project_id",
                schema: "platform",
                table: "env_vars",
                column: "project_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "env_vars",
                schema: "platform");
        }
    }
}
