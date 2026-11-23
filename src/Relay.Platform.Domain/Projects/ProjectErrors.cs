using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Naming;

namespace Relay.Platform.Domain.Projects;

public static class ProjectErrors
{
    public static Error NotFound(ProjectId id) => Error.NotFound("project.not_found", $"Project {id} does not exist.");

    public static Error NameTaken(ResourceName name) => Error.Conflict("project.name_taken", $"A project named {name} already exists.");
}
