using Microsoft.EntityFrameworkCore;
using Relay.Chassis.Primitives;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class GetApp(IPlatformDbContext db)
{
    public async Task<Result<App>> Handle(AppId id, CancellationToken cancellationToken)
    {
        var app = await db.Apps.AsNoTracking().ById(id, cancellationToken);
        return app is null ? AppErrors.NotFound(id) : app;
    }
}
