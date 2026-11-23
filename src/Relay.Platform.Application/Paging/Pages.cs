namespace Relay.Platform.Application.Paging;

public static class Pages
{
    public static Page<T> From<T>(IReadOnlyList<T> fetched, PageRequest request, Func<T, Guid> id)
    {
        if (fetched.Count <= request.Size)
        {
            return new Page<T>(fetched, "");
        }

        var items = fetched.Take(request.Size).ToList();
        return new Page<T>(items, id(items[^1]).ToString());
    }
}
