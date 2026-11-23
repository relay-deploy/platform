namespace Relay.Platform.Application.Paging;

public sealed record Page<T>(IReadOnlyList<T> Items, string NextToken);
