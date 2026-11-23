using Google.Protobuf.WellKnownTypes;
using Relay.Chassis.Postgres.Outbox;
using Relay.Common.V1;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;
using Proto = Relay.Platform.V1;

namespace Relay.Platform.Infrastructure.Releases;

public sealed class OutboxReleaseEvents(IOutbox outbox) : IReleaseEvents
{
    public void Succeeded(App app, Release release) =>
        outbox.Publish(
            new Proto.ReleaseSucceeded
            {
                AppId = app.Id.Value.ToString(),
                ProjectId = app.ProjectId.Value.ToString(),
                ReleaseId = release.Id.Value.ToString(),
                ImageReference = release.Image.Value,
                ImageDigest = release.ImageDigest ?? "",
                Duration = DurationOf(release),
            },
            app.Id.Value);

    public void Failed(App app, Release release) =>
        outbox.Publish(
            new Proto.ReleaseFailed
            {
                AppId = app.Id.Value.ToString(),
                ProjectId = app.ProjectId.Value.ToString(),
                ReleaseId = release.Id.Value.ToString(),
                ImageReference = release.Image.Value,
                Failure = new Failure { Code = release.Failure!.Code, Message = release.Failure.Message },
                Duration = DurationOf(release),
            },
            app.Id.Value);

    private static Duration DurationOf(Release release) =>
        Duration.FromTimeSpan(release.FinishedAt!.Value - release.CreatedAt);
}
