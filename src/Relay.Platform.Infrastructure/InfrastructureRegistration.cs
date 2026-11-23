using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Relay.Chassis.Grpc;
using Relay.Chassis.Postgres;
using Relay.Platform.Application.Persistence;
using Relay.Platform.Application.Releases;
using Relay.Platform.Application.Runtime;
using Relay.Platform.Infrastructure.Persistence;
using Relay.Platform.Infrastructure.Releases;
using Relay.Platform.Infrastructure.Runtime;
using Relay.Platform.Infrastructure.Secrets;
using Relay.Runtime.V1;

namespace Relay.Platform.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddPlatformInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<SecretsOptions>().BindConfiguration(SecretsOptions.Section).ValidateDataAnnotations().ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<SecretsOptions>, SecretsOptionsValidator>());
        services.AddSingleton<ISecretCipher, AesGcmSecretCipher>();
        services.AddRelayPostgres<PlatformDbContext>(PlatformDbContext.Schema).AddRelayJobs<PlatformDbContext>().AddRelayOutbox<PlatformDbContext>();
        services.AddScoped<IPlatformDbContext>(provider => provider.GetRequiredService<PlatformDbContext>());
        services.AddOptions<RuntimeOptions>().BindConfiguration(RuntimeOptions.Section).ValidateDataAnnotations().ValidateOnStart();

        services.AddScoped<IReleaseEvents, OutboxReleaseEvents>();
        services.AddScoped<IReleaseScheduler, JobReleaseScheduler>();
        services.AddScoped<IContainerRuntime, GrpcContainerRuntime>();
        services.AddRelayGrpcClient<RuntimeService.RuntimeServiceClient>(provider =>
            provider.GetRequiredService<IOptions<RuntimeOptions>>().Value.Address!);
        services.AddRelayJob<ApplyReleaseJob, ApplyReleaseJobHandler>();

        services.AddRelayEventSubscription<PlatformDbContext>(RuntimeEvents.Source);
        services.AddRelayEventConsumer<WorkloadCrashed, WorkloadEventConsumer>(RuntimeEvents.Source);
        services.AddRelayEventConsumer<WorkloadRecovered, WorkloadEventConsumer>(RuntimeEvents.Source);
        services.AddRelayEventConsumer<WorkloadStopped, WorkloadEventConsumer>(RuntimeEvents.Source);
        return services;
    }
}
