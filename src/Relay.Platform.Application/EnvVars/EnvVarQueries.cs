using Microsoft.EntityFrameworkCore;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.EnvVars;

public static class EnvVarQueries
{
    public static IQueryable<EnvVar> OfScope(this IQueryable<EnvVar> envVars, EnvVarScope scope) =>
        envVars.Where(envVar => envVar.AppId == scope.App && envVar.ProjectId == scope.Project);

    public static Task<EnvVar?> Named(this IQueryable<EnvVar> envVars, EnvVarName name, CancellationToken cancellationToken) =>
        envVars.FirstOrDefaultAsync(envVar => envVar.Name == name, cancellationToken);

    public static IQueryable<EnvVar> OrderedByName(this IQueryable<EnvVar> envVars) =>
        envVars.OrderBy(envVar => envVar.Name);

    public static async Task<IReadOnlyList<EnvVar>> ResolveFor(
        this IQueryable<EnvVar> envVars,
        ProjectId project,
        AppId app,
        CancellationToken cancellationToken)
    {
        var candidates = await envVars
            .Where(envVar => envVar.ProjectId == project || envVar.AppId == app)
            .ToListAsync(cancellationToken);
        return candidates
            .OrderByDescending(envVar => envVar.AppId is not null)
            .DistinctBy(envVar => envVar.Name)
            .OrderBy(envVar => envVar.Name.Value, StringComparer.Ordinal)
            .ToList();
    }
}
