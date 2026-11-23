using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.Projects;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.HasKey(project => project.Id);
        builder.Property(project => project.Name);
        builder.Property(project => project.CreatedAt);
        builder.HasIndex(project => project.Name).IsUnique();
    }
}
