using Microsoft.EntityFrameworkCore;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public static class AppQueries
{
    public static Task<App?> ById(this IQueryable<App> apps, AppId id, CancellationToken cancellationToken) =>
        apps.FirstOrDefaultAsync(app => app.Id == id, cancellationToken);

    public static IQueryable<App> Live(this IQueryable<App> apps) =>
        apps.Where(app => app.RemovedAt == null);

    public static IQueryable<App> InProject(this IQueryable<App> apps, ProjectId? project) =>
        project is { } id ? apps.Where(app => app.ProjectId == id) : apps;

    public static Task<bool> NameTaken(this IQueryable<App> apps, ProjectId project, ResourceName name, CancellationToken cancellationToken) =>
        apps.Live().AnyAsync(app => app.ProjectId == project && app.Name == name, cancellationToken);

    public static Task<bool> SubdomainTaken(this IQueryable<App> apps, Exposure? exposure, CancellationToken cancellationToken) =>
        exposure is null
            ? Task.FromResult(false)
            : apps.Live().AnyAsync(app => app.Spec.Exposure != null && app.Spec.Exposure.Subdomain == exposure.Subdomain, cancellationToken);

    public static IQueryable<App> NewestFirst(this IQueryable<App> apps) =>
        apps.OrderByDescending(app => app.Id);
}
