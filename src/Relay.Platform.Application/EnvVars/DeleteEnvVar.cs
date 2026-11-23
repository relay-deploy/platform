using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Application.EnvVars;

public sealed class DeleteEnvVar(IPlatformDbContext db)
{
    public async Task<Result> Handle(DeleteEnvVarCommand command, CancellationToken cancellationToken)
    {
        var name = EnvVarName.Create(command.Name);
        if (name.IsFailure)
        {
            return name.Error;
        }

        var envVar = await db.EnvVars.OfScope(command.Scope).Named(name.Value, cancellationToken);
        if (envVar is null)
        {
            return EnvVarErrors.NotFound(name.Value);
        }

        db.EnvVars.Remove(envVar);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
