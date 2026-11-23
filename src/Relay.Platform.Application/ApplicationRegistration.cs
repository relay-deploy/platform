using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Relay.Platform.Application.Apps;
using Relay.Platform.Application.EnvVars;
using Relay.Platform.Application.Projects;
using Relay.Platform.Application.Releases;

namespace Relay.Platform.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddPlatformApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<CreateProject>();
        services.AddScoped<GetProject>();
        services.AddScoped<ListProjects>();
        services.AddScoped<CreateApp>();
        services.AddScoped<GetApp>();
        services.AddScoped<ListApps>();
        services.AddScoped<DeployApp>();
        services.AddScoped<AppRuntimeOperation>();
        services.AddScoped<RestartApp>();
        services.AddScoped<StopApp>();
        services.AddScoped<RemoveApp>();
        services.AddScoped<RecordWorkloadObservation>();
        services.AddScoped<ApplyRelease>();
        services.AddScoped<GetRelease>();
        services.AddScoped<ListReleases>();
        services.AddScoped<SetEnvVar>();
        services.AddScoped<ListEnvVars>();
        services.AddScoped<DeleteEnvVar>();
        return services;
    }
}
