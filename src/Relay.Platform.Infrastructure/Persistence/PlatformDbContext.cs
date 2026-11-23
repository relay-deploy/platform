using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Postgres;
using Relay.Chassis.Postgres.Jobs;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;
using Relay.Platform.Infrastructure.Persistence.Conversions;
using Relay.Platform.Infrastructure.Secrets;

namespace Relay.Platform.Infrastructure.Persistence;

public sealed class PlatformDbContext(DbContextOptions<PlatformDbContext> options, ISecretCipher cipher) : DbContext(options), IPlatformDbContext, IJobContext
{
    public const string Schema = "platform";

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<App> Apps => Set<App>();

    public DbSet<Release> Releases => Set<Release>();

    public DbSet<EnvVar> EnvVars => Set<EnvVar>();

    public DbSet<JobRecord> Jobs => Set<JobRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformDbContext).Assembly);
        modelBuilder.Entity<EnvVar>().Property(envVar => envVar.Value).HasConversion(new EncryptedStringConverter(cipher));
        modelBuilder.AddRelayJobs();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<ProjectId>().HaveConversion<ProjectIdConverter>();
        configurationBuilder.Properties<AppId>().HaveConversion<AppIdConverter>();
        configurationBuilder.Properties<ReleaseId>().HaveConversion<ReleaseIdConverter>();
        configurationBuilder.Properties<EnvVarId>().HaveConversion<EnvVarIdConverter>();
        configurationBuilder.Properties<EnvVarName>().HaveConversion<EnvVarNameConverter>();
        configurationBuilder.Properties<ResourceName>().HaveConversion<ResourceNameConverter>();
        configurationBuilder.Properties<ImageReference>().HaveConversion<ImageReferenceConverter>();
        configurationBuilder.Properties<AppSource>().HaveConversion<AppSourceConverter>().HaveColumnType("jsonb");
        configurationBuilder.Properties<AppPhase>().HaveConversion<string>();
        configurationBuilder.Properties<ReleasePhase>().HaveConversion<string>();
        configurationBuilder.Properties<ReleaseTrigger>().HaveConversion<string>();
    }
}
