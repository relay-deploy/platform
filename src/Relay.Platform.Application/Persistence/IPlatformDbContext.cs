using Microsoft.EntityFrameworkCore;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Persistence;

public interface IPlatformDbContext
{
    DbSet<Project> Projects { get; }

    DbSet<App> Apps { get; }

    DbSet<Release> Releases { get; }

    DbSet<EnvVar> EnvVars { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
