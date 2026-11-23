using Google.Protobuf.WellKnownTypes;
using Relay.Platform.Domain.Projects;
using ProjectMessage = Relay.Platform.V1.Project;

namespace Relay.Platform.Api.Projects;

internal static class ProjectMapping
{
    public static ProjectMessage ToMessage(this Project project) => new()
    {
        Id = project.Id.ToString(),
        Name = project.Name.Value,
        CreatedAt = Timestamp.FromDateTimeOffset(project.CreatedAt),
    };
}
