using Grpc.Core;
using Relay.Chassis.Grpc;
using Relay.Platform.Api.Releases;
using Relay.Platform.Api.Requests;
using Relay.Platform.Application.Apps;
using Relay.Platform.Application.Paging;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Projects;
using Relay.Platform.Domain.Releases;
using Relay.Platform.V1;

namespace Relay.Platform.Api.Apps;

public sealed class AppEndpoint(
    CreateApp createApp,
    GetApp getApp,
    ListApps listApps,
    DeployApp deployApp,
    RestartApp restartApp,
    StopApp stopApp,
    RemoveApp removeApp) : AppService.AppServiceBase
{
    public override async Task<CreateAppResponse> CreateApp(CreateAppRequest request, ServerCallContext context)
    {
        var id = new AppId(RequestIds.Parse(request.AppId, "app_id").OrThrow());
        var project = new ProjectId(RequestIds.Parse(request.ProjectId, "project_id").OrThrow());
        var spec = request.Spec.ToDomain().OrThrow();
        var app = (await createApp.Handle(new CreateAppCommand(id, project, request.Name, spec), context.CancellationToken)).OrThrow();
        return new CreateAppResponse { App = app.ToMessage() };
    }

    public override async Task<GetAppResponse> GetApp(GetAppRequest request, ServerCallContext context)
    {
        var app = (await getApp.Handle(AppIdOf(request.AppId), context.CancellationToken)).OrThrow();
        return new GetAppResponse { App = app.ToMessage() };
    }

    public override async Task<ListAppsResponse> ListApps(ListAppsRequest request, ServerCallContext context)
    {
        ProjectId? project = request.ProjectId.Length == 0
            ? null
            : new ProjectId(RequestIds.Parse(request.ProjectId, "project_id").OrThrow());
        var page = PageRequest.Create(request.PageSize, request.PageToken).OrThrow();
        var apps = await listApps.Handle(new ListAppsQuery(project, page), context.CancellationToken);
        var response = new ListAppsResponse { NextPageToken = apps.NextToken };
        response.Apps.AddRange(apps.Items.Select(app => app.ToMessage()));
        return response;
    }

    public override async Task<DeployAppResponse> DeployApp(DeployAppRequest request, ServerCallContext context)
    {
        var release = new ReleaseId(RequestIds.Parse(request.ReleaseId, "release_id").OrThrow());
        var queued = (await deployApp.Handle(new DeployAppCommand(AppIdOf(request.AppId), release), context.CancellationToken)).OrThrow();
        return new DeployAppResponse { Release = queued.ToMessage() };
    }

    public override async Task<RestartAppResponse> RestartApp(RestartAppRequest request, ServerCallContext context)
    {
        var app = (await restartApp.Handle(AppIdOf(request.AppId), context.CancellationToken)).OrThrow();
        return new RestartAppResponse { App = app.ToMessage() };
    }

    public override async Task<StopAppResponse> StopApp(StopAppRequest request, ServerCallContext context)
    {
        var app = (await stopApp.Handle(AppIdOf(request.AppId), context.CancellationToken)).OrThrow();
        return new StopAppResponse { App = app.ToMessage() };
    }

    public override async Task<RemoveAppResponse> RemoveApp(RemoveAppRequest request, ServerCallContext context)
    {
        (await removeApp.Handle(AppIdOf(request.AppId), context.CancellationToken)).OrThrow();
        return new RemoveAppResponse();
    }

    private static AppId AppIdOf(string value) => new(RequestIds.Parse(value, "app_id").OrThrow());
}
