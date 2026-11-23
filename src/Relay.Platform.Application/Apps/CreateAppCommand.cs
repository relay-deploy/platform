using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Application.Apps;

public sealed record CreateAppCommand(AppId Id, ProjectId Project, string Name, AppSpec Spec);
