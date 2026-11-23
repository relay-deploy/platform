using Relay.Chassis.Hosting;
using Relay.Platform.Api.Apps;
using Relay.Platform.Api.Projects;
using Relay.Platform.Api.Releases;
using Relay.Platform.Application;
using Relay.Platform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.AddRelayServiceDefaults();
builder.Services.AddPlatformApplication().AddPlatformInfrastructure();

var app = builder.Build();
app.MapRelayServiceDefaults();
app.MapGrpcService<ProjectEndpoint>();
app.MapGrpcService<AppEndpoint>();
app.MapGrpcService<ReleaseEndpoint>();

await app.RunAsync();
