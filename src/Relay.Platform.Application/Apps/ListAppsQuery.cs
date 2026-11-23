using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public sealed record ListAppsQuery(ProjectId? Project, PageRequest Page);
