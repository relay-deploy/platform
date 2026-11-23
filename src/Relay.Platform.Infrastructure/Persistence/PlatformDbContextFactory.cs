using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Relay.Chassis.Postgres;
using Relay.Platform.Infrastructure.Secrets;

namespace Relay.Platform.Infrastructure.Persistence;

public sealed class PlatformDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    private const string DesignTimeKeyVersion = "design";

    public PlatformDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PlatformDbContext>();
        builder.UseRelayPostgres("Host=localhost", PlatformDbContext.Schema);
        var designTimeKey = new SecretsOptions
        {
            ActiveKeyVersion = DesignTimeKeyVersion,
            Keys = new Dictionary<string, string> { [DesignTimeKeyVersion] = Convert.ToBase64String(SecretKeys.Generate()) },
        };
        return new PlatformDbContext(builder.Options, new AesGcmSecretCipher(Options.Create(designTimeKey)));
    }
}
