using Microsoft.EntityFrameworkCore;

namespace Relay.Platform.Application.Paging;

public static class Paging
{
    public static async Task<Page<T>> ToPage<T>(this IQueryable<T> query, PageRequest page, CancellationToken cancellationToken)
    {
        var fetched = await query.Skip(page.Offset).Take(page.Size + 1).ToListAsync(cancellationToken);
        return fetched.Count <= page.Size
            ? new Page<T>(fetched, "")
            : new Page<T>(fetched.Take(page.Size).ToList(), page.NextToken());
    }
}
