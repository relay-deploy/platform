using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Apps;

public sealed class GetApp(IAppRepository apps)
{
    public async Task<Result<App>> Handle(AppId id, CancellationToken cancellationToken)
    {
        var app = await apps.Find(id, cancellationToken);
        return app is null ? AppErrors.NotFound(id) : app;
    }
}
