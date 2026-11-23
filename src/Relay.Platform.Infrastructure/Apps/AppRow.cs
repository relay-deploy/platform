using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;
using Relay.Platform.Infrastructure.Persistence;

namespace Relay.Platform.Infrastructure.Apps;

internal sealed class AppRow
{
    public Guid Id { get; init; }

    public Guid ProjectId { get; init; }

    public string Name { get; init; } = "";

    public string ImageReference { get; init; } = "";

    public int? ExposurePort { get; init; }

    public string? ExposureSubdomain { get; init; }

    public long MemoryLimitBytes { get; init; }

    public int CpuLimitMillicores { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? RemovedAt { get; init; }

    public string Phase { get; init; } = "";

    public Guid? CurrentReleaseId { get; init; }

    public DateTime ObservedAt { get; init; }

    public App ToDomain()
    {
        var source = new ImageSource(Domain.Apps.ImageReference.Create(ImageReference).Value);
        var exposure = ExposurePort is { } port && ExposureSubdomain is { } subdomain
            ? Exposure.Create(port, ResourceName.Create(subdomain).Value).Value
            : null;
        var spec = new AppSpec(source, exposure, Resources.Create(MemoryLimitBytes, CpuLimitMillicores).Value);
        var status = new AppStatus(
            AppPhaseColumn.To(Phase),
            CurrentReleaseId is { } release ? new ReleaseId(release) : null,
            Timestamps.FromColumn(ObservedAt));
        return App.Restore(
            new AppId(Id),
            new ProjectId(ProjectId),
            ResourceName.Create(Name).Value,
            spec,
            status,
            Timestamps.FromColumn(CreatedAt),
            Timestamps.FromColumn(RemovedAt));
    }
}
