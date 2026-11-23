using Microsoft.EntityFrameworkCore;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Projects;

public sealed class ListProjects(IPlatformDbContext db)
{
    public Task<Page<Project>> Handle(PageRequest page, CancellationToken cancellationToken) =>
        db.Projects.AsNoTracking().NewestFirst().ToPage(page, cancellationToken);
}
