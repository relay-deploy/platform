using Relay.Chassis.Postgres.Sessions;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Releases;

public sealed class PostgresReleaseRepository(PostgresSession session) : IReleaseRepository
{
    private const string Columns =
        "id, app_id, image_reference, image_digest, trigger, phase, failure_code, failure_message, created_at, finished_at";

    private const string Insert = """
        INSERT INTO release (id, app_id, image_reference, image_digest, trigger, phase, failure_code, failure_message, created_at, finished_at)
        VALUES (@id, @appId, @imageReference, @imageDigest, @trigger, @phase, @failureCode, @failureMessage, @createdAt, @finishedAt)
        """;

    private const string UpdateRelease = """
        UPDATE release
        SET image_digest = @imageDigest, phase = @phase, failure_code = @failureCode,
            failure_message = @failureMessage, finished_at = @finishedAt
        WHERE id = @id
        """;

    public async Task<Release?> Find(ReleaseId id, CancellationToken cancellationToken)
    {
        var row = await session.QuerySingleOrDefault<ReleaseRow>($"SELECT {Columns} FROM release WHERE id = @id", new { id = id.Value }, cancellationToken);
        return row?.ToDomain();
    }

    public Task Add(Release release, CancellationToken cancellationToken) =>
        session.Execute(Insert, Parameters(release), cancellationToken);

    public Task Update(Release release, CancellationToken cancellationToken) =>
        session.Execute(UpdateRelease, Parameters(release), cancellationToken);

    public async Task<Page<Release>> ListForApp(AppId app, PageRequest page, CancellationToken cancellationToken)
    {
        var rows = await session.Query<ReleaseRow>(
            $"SELECT {Columns} FROM release WHERE app_id = @app AND (@after::uuid IS NULL OR id < @after) ORDER BY id DESC LIMIT @limit",
            new { app = app.Value, after = page.After, limit = page.Size + 1 },
            cancellationToken);
        return Pages.From(rows.Select(row => row.ToDomain()).ToList(), page, release => release.Id.Value);
    }

    private static object Parameters(Release release) => new
    {
        id = release.Id.Value,
        appId = release.AppId.Value,
        imageReference = release.Image.Value,
        imageDigest = release.ImageDigest,
        trigger = ReleaseColumns.From(release.Trigger),
        phase = ReleaseColumns.From(release.Phase),
        failureCode = release.Failure?.Code,
        failureMessage = release.Failure?.Message,
        createdAt = release.CreatedAt,
        finishedAt = release.FinishedAt,
    };
}
