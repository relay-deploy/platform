using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Domain.EnvVars;

public sealed class EnvVar
{
    private EnvVar(EnvVarId id, AppId? appId, ProjectId? projectId, EnvVarName name, string value, bool isSecret, DateTimeOffset updatedAt)
    {
        Id = id;
        AppId = appId;
        ProjectId = projectId;
        Name = name;
        Value = value;
        IsSecret = isSecret;
        UpdatedAt = updatedAt;
    }

    public EnvVarId Id { get; }

    public AppId? AppId { get; }

    public ProjectId? ProjectId { get; }

    public EnvVarScope Scope => AppId is { } app ? EnvVarScope.ForApp(app) : EnvVarScope.ForProject(ProjectId!.Value);

    public EnvVarName Name { get; }

    public string Value { get; private set; }

    public bool IsSecret { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static EnvVar Create(EnvVarId id, EnvVarScope scope, EnvVarName name, string value, bool isSecret, DateTimeOffset now) =>
        new(id, scope.App, scope.Project, name, value, isSecret, now);

    public void Change(string value, bool isSecret, DateTimeOffset now)
    {
        Value = value;
        IsSecret = isSecret;
        UpdatedAt = now;
    }
}
