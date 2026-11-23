using Google.Protobuf.WellKnownTypes;
using Relay.Chassis.Primitives;
using Relay.Platform.Api.Requests;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.EnvVars;
using Relay.Platform.Domain.Projects;
using EnvVarMessage = Relay.Platform.V1.EnvVar;
using EnvVarScopeMessage = Relay.Platform.V1.EnvVarScope;

namespace Relay.Platform.Api.EnvVars;

internal static class EnvVarMapping
{
    public static EnvVarMessage ToMessage(this EnvVar envVar) => new()
    {
        Scope = envVar.Scope.ToMessage(),
        Name = envVar.Name.Value,
        Value = envVar.IsSecret ? "" : envVar.Value,
        Secret = envVar.IsSecret,
        UpdatedAt = Timestamp.FromDateTimeOffset(envVar.UpdatedAt),
    };

    public static Result<EnvVarScope> ToDomain(this EnvVarScopeMessage? scope) => scope?.TargetCase switch
    {
        EnvVarScopeMessage.TargetOneofCase.AppId =>
            RequestIds.Parse(scope.AppId, "scope.app_id").Map(id => EnvVarScope.ForApp(new AppId(id))),
        EnvVarScopeMessage.TargetOneofCase.ProjectId =>
            RequestIds.Parse(scope.ProjectId, "scope.project_id").Map(id => EnvVarScope.ForProject(new ProjectId(id))),
        _ => EnvVarErrors.ScopeRequired(),
    };

    private static EnvVarScopeMessage ToMessage(this EnvVarScope scope) => scope.App is { } app
        ? new EnvVarScopeMessage { AppId = app.ToString() }
        : new EnvVarScopeMessage { ProjectId = scope.Project.ToString() };
}
