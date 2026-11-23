using Relay.Chassis.Postgres.Sessions;
using Relay.Platform.Application.Apps;
using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.Apps;

public sealed class PostgresAppRepository(PostgresSession session) : IAppRepository
{
    private const string Select = """
        SELECT a.id, a.project_id, a.name, a.image_reference, a.exposure_port, a.exposure_subdomain,
               a.memory_limit_bytes, a.cpu_limit_millicores, a.created_at, a.removed_at,
               s.phase, s.current_release_id, s.observed_at
        FROM app a
        JOIN app_state s ON s.app_id = a.id
        """;

    private const string InsertApp = """
        INSERT INTO app (id, project_id, name, source_kind, image_reference, exposure_port, exposure_subdomain,
                         memory_limit_bytes, cpu_limit_millicores, created_at, removed_at)
        VALUES (@id, @projectId, @name, 'image', @imageReference, @exposurePort, @exposureSubdomain,
                @memoryLimitBytes, @cpuLimitMillicores, @createdAt, @removedAt)
        """;

    private const string InsertState = """
        INSERT INTO app_state (app_id, phase, current_release_id, observed_at)
        VALUES (@id, @phase, @currentReleaseId, @observedAt)
        """;

    private const string UpdateApp = "UPDATE app SET removed_at = @removedAt WHERE id = @id";

    private const string UpdateState = """
        UPDATE app_state SET phase = @phase, current_release_id = @currentReleaseId, observed_at = @observedAt
        WHERE app_id = @id
        """;

    public async Task<App?> Find(AppId id, CancellationToken cancellationToken)
    {
        var row = await session.QuerySingleOrDefault<AppRow>($"{Select} WHERE a.id = @id", new { id = id.Value }, cancellationToken);
        return row?.ToDomain();
    }

    public Task<bool> NameTaken(ProjectId project, ResourceName name, CancellationToken cancellationToken) =>
        session.QuerySingleOrDefault<bool>(
            "SELECT EXISTS (SELECT 1 FROM app WHERE project_id = @project AND name = @name AND removed_at IS NULL)",
            new { project = project.Value, name = name.Value },
            cancellationToken);

    public Task<bool> SubdomainTaken(ResourceName subdomain, CancellationToken cancellationToken) =>
        session.QuerySingleOrDefault<bool>(
            "SELECT EXISTS (SELECT 1 FROM app WHERE exposure_subdomain = @subdomain AND removed_at IS NULL)",
            new { subdomain = subdomain.Value },
            cancellationToken);

    public async Task Add(App app, CancellationToken cancellationToken)
    {
        await session.Execute(InsertApp, AppParameters(app), cancellationToken);
        await session.Execute(InsertState, StateParameters(app), cancellationToken);
    }

    public async Task Update(App app, CancellationToken cancellationToken)
    {
        await session.Execute(UpdateApp, new { id = app.Id.Value, removedAt = app.RemovedAt }, cancellationToken);
        await session.Execute(UpdateState, StateParameters(app), cancellationToken);
    }

    public async Task<Page<App>> List(ProjectId? project, PageRequest page, CancellationToken cancellationToken)
    {
        var rows = await session.Query<AppRow>(
            $"""
            {Select}
            WHERE a.removed_at IS NULL
              AND (@project::uuid IS NULL OR a.project_id = @project)
              AND (@after::uuid IS NULL OR a.id < @after)
            ORDER BY a.id DESC
            LIMIT @limit
            """,
            new { project = project?.Value, after = page.After, limit = page.Size + 1 },
            cancellationToken);
        return Pages.From(rows.Select(row => row.ToDomain()).ToList(), page, app => app.Id.Value);
    }

    private static object AppParameters(App app) => new
    {
        id = app.Id.Value,
        projectId = app.ProjectId.Value,
        name = app.Name.Value,
        imageReference = ((ImageSource)app.Spec.Source).Reference.Value,
        exposurePort = app.Spec.Exposure?.Port,
        exposureSubdomain = app.Spec.Exposure?.Subdomain.Value,
        memoryLimitBytes = app.Spec.Resources.MemoryLimitBytes,
        cpuLimitMillicores = app.Spec.Resources.CpuLimitMillicores,
        createdAt = app.CreatedAt,
        removedAt = app.RemovedAt,
    };

    private static object StateParameters(App app) => new
    {
        id = app.Id.Value,
        phase = AppPhaseColumn.From(app.Status.Phase),
        currentReleaseId = app.Status.CurrentRelease?.Value,
        observedAt = app.Status.ObservedAt,
    };
}
