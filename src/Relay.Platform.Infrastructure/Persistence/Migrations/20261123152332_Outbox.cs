using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Relay.Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Outbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_events",
                schema: "platform",
                columns: table => new
                {
                    sequence = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    transaction_id = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "(pg_current_xact_id()::text)::bigint")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_events", x => x.sequence);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_events_id",
                schema: "platform",
                table: "outbox_events",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_events_transaction_id_sequence",
                schema: "platform",
                table: "outbox_events",
                columns: new[] { "transaction_id", "sequence" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_events",
                schema: "platform");
        }
    }
}
