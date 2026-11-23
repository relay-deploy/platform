using Microsoft.EntityFrameworkCore;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Application.EnvVars;

public sealed class ListEnvVars(IPlatformDbContext db)
{
    public Task<Page<EnvVar>> Handle(ListEnvVarsQuery query, CancellationToken cancellationToken) =>
        db.EnvVars.AsNoTracking().OfScope(query.Scope).OrderedByName().ToPage(query.Page, cancellationToken);
}
