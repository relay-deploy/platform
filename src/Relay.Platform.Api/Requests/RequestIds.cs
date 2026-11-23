using Relay.Chassis.Primitives;

namespace Relay.Platform.Api.Requests;

internal static class RequestIds
{
    public static Result<Guid> Parse(string value, string field) =>
        Guid.TryParse(value, out var id)
            ? Result.Success(id)
            : Error.Validation("request.invalid_id", $"{field} must be a UUID, got '{value}'.");
}
