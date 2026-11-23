using Microsoft.EntityFrameworkCore;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public static class ProjectQueries
{
    public static Task<Project?> ById(this IQueryable<Project> projects, ProjectId id, CancellationToken cancellationToken) =>
        projects.FirstOrDefaultAsync(project => project.Id == id, cancellationToken);

    public static Task<bool> Exists(this IQueryable<Project> projects, ProjectId id, CancellationToken cancellationToken) =>
        projects.AnyAsync(project => project.Id == id, cancellationToken);

    public static Task<bool> NameTaken(this IQueryable<Project> projects, ResourceName name, CancellationToken cancellationToken) =>
        projects.AnyAsync(project => project.Name == name, cancellationToken);

    public static IQueryable<Project> NewestFirst(this IQueryable<Project> projects) =>
        projects.OrderByDescending(project => project.Id);
}
