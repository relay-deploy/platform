using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.Apps;

public sealed class AppConfiguration : IEntityTypeConfiguration<App>
{
    public void Configure(EntityTypeBuilder<App> builder)
    {
        builder.ToTable("apps");
        builder.HasKey(app => app.Id);
        builder.Property(app => app.ProjectId);
        builder.Property(app => app.Name);
        builder.Property(app => app.CreatedAt);
        builder.Property(app => app.RemovedAt);
        builder.Ignore(app => app.IsRemoved);
        builder.ComplexProperty(app => app.Spec, spec =>
        {
            spec.Property(value => value.Source);
            spec.ComplexProperty(value => value.Exposure, exposure =>
            {
                exposure.Property(value => value.Port);
                exposure.Property(value => value.Subdomain);
            });
            spec.ComplexProperty(value => value.Resources, resources =>
            {
                resources.Property(value => value.MemoryLimitBytes);
                resources.Property(value => value.CpuLimitMillicores);
            });
        });
        builder.ComplexProperty(app => app.Status, status =>
        {
            status.Property(value => value.Phase);
            status.Property(value => value.CurrentRelease);
            status.Property(value => value.ObservedAt);
        });
        builder.HasOne<Project>().WithMany().HasForeignKey(app => app.ProjectId);
        builder.HasIndex(app => new { app.ProjectId, app.Name });
    }
}
