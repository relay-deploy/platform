using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;
using Relay.Platform.Infrastructure.Persistence;

namespace Relay.Platform.Infrastructure.Releases;

internal sealed class ReleaseRow
{
    public Guid Id { get; init; }

    public Guid AppId { get; init; }

    public string ImageReference { get; init; } = "";

    public string? ImageDigest { get; init; }

    public string Trigger { get; init; } = "";

    public string Phase { get; init; } = "";

    public string? FailureCode { get; init; }

    public string? FailureMessage { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? FinishedAt { get; init; }

    public Release ToDomain() =>
        Release.Restore(
            new ReleaseId(Id),
            new AppId(AppId),
            Domain.Apps.ImageReference.Create(ImageReference).Value,
            ImageDigest,
            ReleaseColumns.ToTrigger(Trigger),
            ReleaseColumns.ToPhase(Phase),
            FailureCode is { } code ? new ReleaseFailure(code, FailureMessage ?? "") : null,
            Timestamps.FromColumn(CreatedAt),
            Timestamps.FromColumn(FinishedAt));
}
