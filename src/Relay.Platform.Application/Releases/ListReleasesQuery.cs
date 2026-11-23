using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Application.Releases;

public sealed record ListReleasesQuery(AppId App, PageRequest Page);
