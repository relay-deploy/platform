using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Apps;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Projects;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.EnvVars;

public sealed class SetEnvVar(IPlatformDbContext db, TimeProvider time)
{
    public async Task<Result<EnvVar>> Handle(SetEnvVarCommand command, CancellationToken cancellationToken)
    {
        var name = EnvVarName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        var owner = await EnsureOwnerAvailable(command.Scope, cancellationToken);
        if (owner.IsFailure)
        {
            return owner.Error;
        }

        var now = time.GetUtcNow();
        var envVar = await db.EnvVars.OfScope(command.Scope).Named(name.Value, cancellationToken);
        if (envVar is null)
        {
            envVar = EnvVar.Create(EnvVarId.New(), command.Scope, name.Value, command.Value, command.Secret, now);
            db.EnvVars.Add(envVar);
        }
        else
        {
            envVar.Change(command.Value, command.Secret, now);
        }

        await db.SaveChangesAsync(cancellationToken);
        return envVar;
    }

    private async Task<Result> EnsureOwnerAvailable(EnvVarScope scope, CancellationToken cancellationToken)
    {
        if (scope.App is { } appId)
        {
            var app = await db.Apps.AsNoTracking().ById(appId, cancellationToken);
            return app is null ? AppErrors.NotFound(appId) : app.EnsureActive();
        }

        if (scope.Project is { } projectId)
        {
            return await db.Projects.Exists(projectId, cancellationToken) ? Result.Success() : ProjectErrors.NotFound(projectId);
        }

        return EnvVarErrors.ScopeRequired();
    }
}
