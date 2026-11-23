using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "platform");

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<string>(type: "text", nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    idempotency_key = table.Column<string>(type: "text", nullable: false),
                    run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    max_attempts = table.Column<int>(type: "integer", nullable: false),
                    locked_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "text", nullable: true),
                    failed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "apps",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    removed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    spec_source = table.Column<string>(type: "jsonb", nullable: false),
                    spec_exposure_port = table.Column<int>(type: "integer", nullable: true),
                    spec_exposure_subdomain = table.Column<string>(type: "text", nullable: true),
                    spec_resources_cpu_limit_millicores = table.Column<int>(type: "integer", nullable: false),
                    spec_resources_memory_limit_bytes = table.Column<long>(type: "bigint", nullable: false),
                    status_current_release = table.Column<Guid>(type: "uuid", nullable: true),
                    status_observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status_phase = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apps", x => x.id);
                    table.ForeignKey(
                        name: "fk_apps_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "platform",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "releases",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    app_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image = table.Column<string>(type: "text", nullable: false),
                    image_digest = table.Column<string>(type: "text", nullable: true),
                    trigger = table.Column<string>(type: "text", nullable: false),
                    phase = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure_code = table.Column<string>(type: "text", nullable: true),
                    failure_message = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_releases", x => x.id);
                    table.ForeignKey(
                        name: "fk_releases_apps_app_id",
                        column: x => x.app_id,
                        principalSchema: "platform",
                        principalTable: "apps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_apps_project_id_name",
                schema: "platform",
                table: "apps",
                columns: new[] { "project_id", "name" });

            migrationBuilder.CreateIndex(
                name: "ix_jobs_failed_at_run_at",
                schema: "platform",
                table: "jobs",
                columns: new[] { "failed_at", "run_at" });

            migrationBuilder.CreateIndex(
                name: "ix_jobs_idempotency_key",
                schema: "platform",
                table: "jobs",
                column: "idempotency_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_projects_name",
                schema: "platform",
                table: "projects",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_releases_app_id",
                schema: "platform",
                table: "releases",
                column: "app_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "releases",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "apps",
                schema: "platform");

            migrationBuilder.DropTable(
                name: "projects",
                schema: "platform");
        }
    }
}
