using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Naming;

namespace Relay.Platform.Domain.Apps;

public static class AppErrors
{
    public static Error NotFound(AppId id) => Error.NotFound("app.not_found", $"App {id} does not exist.");

    public static Error Removed(AppId id) => Error.Conflict("app.removed", $"App {id} has been removed.");

    public static Error DeployInProgress(AppId id) => Error.Conflict("app.deploy_in_progress", $"App {id} is already being deployed.");

    public static Error NameTaken(ResourceName name) => Error.Conflict("app.name_taken", $"The project already has an app named {name}.");

    public static Error SubdomainTaken(ResourceName subdomain) => Error.Conflict("app.subdomain_taken", $"The subdomain {subdomain} is already in use.");
}
