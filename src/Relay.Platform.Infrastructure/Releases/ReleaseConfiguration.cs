using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Releases;

public sealed class ReleaseConfiguration : IEntityTypeConfiguration<Release>
{
    public void Configure(EntityTypeBuilder<Release> builder)
    {
        builder.ToTable("releases");
        builder.HasKey(release => release.Id);
        builder.Property(release => release.AppId);
        builder.Property(release => release.Image);
        builder.Property(release => release.ImageDigest);
        builder.Property(release => release.Trigger);
        builder.Property(release => release.Phase);
        builder.Property(release => release.CreatedAt);
        builder.Property(release => release.FinishedAt);
        builder.Ignore(release => release.IsFinished);
        builder.ComplexProperty(release => release.Failure, failure =>
        {
            failure.Property(value => value.Code);
            failure.Property(value => value.Message);
        });
        builder.HasOne<App>().WithMany().HasForeignKey(release => release.AppId);
        builder.HasIndex(release => release.AppId);
    }
}
