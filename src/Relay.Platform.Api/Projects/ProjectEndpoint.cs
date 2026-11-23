using Grpc.Core;
using Relay.Chassis.Grpc;
using Relay.Platform.Api.Requests;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Projects;
using Relay.Platform.Domain.Projects;
using Relay.Platform.V1;

namespace Relay.Platform.Api.Projects;

public sealed class ProjectEndpoint(CreateProject createProject, GetProject getProject, ListProjects listProjects)
    : ProjectService.ProjectServiceBase
{
    public override async Task<CreateProjectResponse> CreateProject(CreateProjectRequest request, ServerCallContext context)
    {
        var id = new ProjectId(RequestIds.Parse(request.ProjectId, "project_id").OrThrow());
        var project = (await createProject.Handle(new CreateProjectCommand(id, request.Name), context.CancellationToken)).OrThrow();
        return new CreateProjectResponse { Project = project.ToMessage() };
    }

    public override async Task<GetProjectResponse> GetProject(GetProjectRequest request, ServerCallContext context)
    {
        var id = new ProjectId(RequestIds.Parse(request.ProjectId, "project_id").OrThrow());
        var project = (await getProject.Handle(id, context.CancellationToken)).OrThrow();
        return new GetProjectResponse { Project = project.ToMessage() };
    }

    public override async Task<ListProjectsResponse> ListProjects(ListProjectsRequest request, ServerCallContext context)
    {
        var page = PageRequest.Create(request.PageSize, request.PageToken).OrThrow();
        var projects = await listProjects.Handle(page, context.CancellationToken);
        var response = new ListProjectsResponse { NextPageToken = projects.NextToken };
        response.Projects.AddRange(projects.Items.Select(project => project.ToMessage()));
        return response;
    }
}
