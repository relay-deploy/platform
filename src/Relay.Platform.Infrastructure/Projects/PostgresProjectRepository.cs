using Relay.Chassis.Postgres.Sessions;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Projects;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Infrastructure.Projects;

public sealed class PostgresProjectRepository(PostgresSession session) : IProjectRepository
{
    private const string Columns = "id, name, created_at";

    public async Task<Project?> Find(ProjectId id, CancellationToken cancellationToken)
    {
        var row = await session.QuerySingleOrDefault<ProjectRow>($"SELECT {Columns} FROM project WHERE id = @id", new { id = id.Value }, cancellationToken);
        return row?.ToDomain();
    }

    public Task<bool> NameTaken(ResourceName name, CancellationToken cancellationToken) =>
        session.QuerySingleOrDefault<bool>("SELECT EXISTS (SELECT 1 FROM project WHERE name = @name)", new { name = name.Value }, cancellationToken);

    public Task Add(Project project, CancellationToken cancellationToken) =>
        session.Execute(
            "INSERT INTO project (id, name, created_at) VALUES (@id, @name, @createdAt)",
            new { id = project.Id.Value, name = project.Name.Value, createdAt = project.CreatedAt },
            cancellationToken);

    public async Task<Page<Project>> List(PageRequest page, CancellationToken cancellationToken)
    {
        var rows = await session.Query<ProjectRow>(
            $"SELECT {Columns} FROM project WHERE (@after::uuid IS NULL OR id < @after) ORDER BY id DESC LIMIT @limit",
            new { after = page.After, limit = page.Size + 1 },
            cancellationToken);
        return Pages.From(rows.Select(row => row.ToDomain()).ToList(), page, project => project.Id.Value);
    }
}
