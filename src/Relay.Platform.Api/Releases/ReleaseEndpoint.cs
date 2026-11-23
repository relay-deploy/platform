using Grpc.Core;
using Relay.Chassis.Grpc;
using Relay.Platform.Api.Requests;
using Relay.Platform.Application.Paging;
using Relay.Platform.Application.Releases;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;
using Relay.Platform.V1;

namespace Relay.Platform.Api.Releases;

public sealed class ReleaseEndpoint(GetRelease getRelease, ListReleases listReleases) : ReleaseService.ReleaseServiceBase
{
    public override async Task<GetReleaseResponse> GetRelease(GetReleaseRequest request, ServerCallContext context)
    {
        var id = new ReleaseId(RequestIds.Parse(request.ReleaseId, "release_id").OrThrow());
        var release = (await getRelease.Handle(id, context.CancellationToken)).OrThrow();
        return new GetReleaseResponse { Release = release.ToMessage() };
    }

    public override async Task<ListReleasesResponse> ListReleases(ListReleasesRequest request, ServerCallContext context)
    {
        var app = new AppId(RequestIds.Parse(request.AppId, "app_id").OrThrow());
        var page = PageRequest.Create(request.PageSize, request.PageToken).OrThrow();
        var releases = await listReleases.Handle(new ListReleasesQuery(app, page), context.CancellationToken);
        var response = new ListReleasesResponse { NextPageToken = releases.NextToken };
        response.Releases.AddRange(releases.Items.Select(release => release.ToMessage()));
        return response;
    }
}
