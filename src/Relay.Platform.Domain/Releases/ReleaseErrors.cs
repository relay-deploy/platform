using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.Releases;

public static class ReleaseErrors
{
    public static Error NotFound(ReleaseId id) => Error.NotFound("release.not_found", $"Release {id} does not exist.");

    public static Error BelongsToAnotherApp(ReleaseId id) => Error.Conflict("release.belongs_to_another_app", $"Release {id} already exists for a different app.");
}
