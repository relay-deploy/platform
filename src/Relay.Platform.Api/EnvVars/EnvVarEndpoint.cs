using Grpc.Core;
using Relay.Chassis.Grpc;
using Relay.Platform.Application.EnvVars;
using Relay.Platform.Application.Paging;
using Relay.Platform.V1;

namespace Relay.Platform.Api.EnvVars;

public sealed class EnvVarEndpoint(SetEnvVar setEnvVar, ListEnvVars listEnvVars, DeleteEnvVar deleteEnvVar)
    : EnvVarService.EnvVarServiceBase
{
    public override async Task<SetEnvVarResponse> SetEnvVar(SetEnvVarRequest request, ServerCallContext context)
    {
        var scope = request.Scope.ToDomain().OrThrow();
        var command = new SetEnvVarCommand(scope, request.Name, request.Value, request.Secret);
        var envVar = (await setEnvVar.Handle(command, context.CancellationToken)).OrThrow();
        return new SetEnvVarResponse { EnvVar = envVar.ToMessage() };
    }

    public override async Task<ListEnvVarsResponse> ListEnvVars(ListEnvVarsRequest request, ServerCallContext context)
    {
        var scope = request.Scope.ToDomain().OrThrow();
        var page = PageRequest.Create(request.PageSize, request.PageToken).OrThrow();
        var envVars = await listEnvVars.Handle(new ListEnvVarsQuery(scope, page), context.CancellationToken);
        var response = new ListEnvVarsResponse { NextPageToken = envVars.NextToken };
        response.EnvVars.AddRange(envVars.Items.Select(envVar => envVar.ToMessage()));
        return response;
    }

    public override async Task<DeleteEnvVarResponse> DeleteEnvVar(DeleteEnvVarRequest request, ServerCallContext context)
    {
        var scope = request.Scope.ToDomain().OrThrow();
        (await deleteEnvVar.Handle(new DeleteEnvVarCommand(scope, request.Name), context.CancellationToken)).OrThrow();
        return new DeleteEnvVarResponse();
    }
}
