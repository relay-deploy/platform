using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Apps;

public sealed record DeployAppCommand(AppId App, ReleaseId Release);
