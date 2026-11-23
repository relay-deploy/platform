using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Projects;

namespace Relay.Platform.Domain.EnvVars;

public sealed record EnvVarScope
{
    private EnvVarScope(AppId? app, ProjectId? project)
    {
        App = app;
        Project = project;
    }

    public AppId? App { get; }

    public ProjectId? Project { get; }

    public static EnvVarScope ForApp(AppId app) => new(app, null);

    public static EnvVarScope ForProject(ProjectId project) => new(null, project);
}
