using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Relay.Platform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EventCursors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_cursors",
                schema: "platform",
                columns: table => new
                {
                    source = table.Column<string>(type: "text", nullable: false),
                    transaction_id = table.Column<long>(type: "bigint", nullable: false),
                    sequence = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_cursors", x => x.source);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_cursors",
                schema: "platform");
        }
    }
}
