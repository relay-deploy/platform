using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Relay.Chassis.Postgres;

namespace Relay.Platform.Infrastructure.Persistence;

public sealed class PlatformDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PlatformDbContext>();
        builder.UseRelayPostgres("Host=localhost", PlatformDbContext.Schema);
        return new PlatformDbContext(builder.Options);
    }
}
