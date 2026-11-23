using Relay.Chassis.Primitives;

namespace Relay.Platform.Application.Paging;

public sealed record PageRequest
{
    private const int DefaultSize = 50;
    private const int MaxSize = 200;

    private PageRequest(int size, Guid? after)
    {
        Size = size;
        After = after;
    }

    public int Size { get; }

    public Guid? After { get; }

    public static Result<PageRequest> Create(int size, string token)
    {
        if (size < 0)
        {
            return Error.Validation("page.invalid_size", "The page size cannot be negative.");
        }

        var normalized = size == 0 ? DefaultSize : Math.Min(size, MaxSize);
        if (token.Length == 0)
        {
            return Result.Success(new PageRequest(normalized, null));
        }

        return Guid.TryParse(token, out var after)
            ? Result.Success(new PageRequest(normalized, after))
            : Error.Validation("page.invalid_token", "The page token is not valid.");
    }
}
