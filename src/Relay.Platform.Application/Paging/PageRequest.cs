using System.Globalization;
using Relay.Chassis.Primitives;

namespace Relay.Platform.Application.Paging;

public sealed record PageRequest
{
    private const int DefaultSize = 50;
    private const int MaxSize = 200;

    private PageRequest(int size, int offset)
    {
        Size = size;
        Offset = offset;
    }

    public int Size { get; }

    public int Offset { get; }

    public static Result<PageRequest> Create(int size, string token)
    {
        if (size < 0)
        {
            return Error.Validation("page.invalid_size", "The page size cannot be negative.");
        }

        var normalized = size == 0 ? DefaultSize : Math.Min(size, MaxSize);
        if (token.Length == 0)
        {
            return Result.Success(new PageRequest(normalized, 0));
        }

        return int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out var offset)
            ? Result.Success(new PageRequest(normalized, offset))
            : Error.Validation("page.invalid_token", "The page token is not valid.");
    }

    public string NextToken() => (Offset + Size).ToString(CultureInfo.InvariantCulture);
}
