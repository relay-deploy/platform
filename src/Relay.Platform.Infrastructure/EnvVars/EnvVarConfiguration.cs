using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.EnvVars;

public sealed class EnvVarConfiguration : IEntityTypeConfiguration<EnvVar>
{
    public void Configure(EntityTypeBuilder<EnvVar> builder)
    {
        builder.ToTable("env_vars");
        builder.HasKey(envVar => envVar.Id);
        builder.Property(envVar => envVar.Name);
        builder.Property(envVar => envVar.Value);
        builder.Property(envVar => envVar.IsSecret);
        builder.Property(envVar => envVar.UpdatedAt);
        builder.Property(envVar => envVar.AppId);
        builder.Property(envVar => envVar.ProjectId);
        builder.Ignore(envVar => envVar.Scope);
        builder.HasOne<App>().WithMany().HasForeignKey(envVar => envVar.AppId);
        builder.HasOne<Project>().WithMany().HasForeignKey(envVar => envVar.ProjectId);
        builder.HasIndex(envVar => new { envVar.AppId, envVar.ProjectId, envVar.Name }).IsUnique().AreNullsDistinct(false);
    }
}
