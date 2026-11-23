using Google.Protobuf.WellKnownTypes;
using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Naming;
using AppMessage = Relay.Platform.V1.App;
using AppPhaseMessage = Relay.Platform.V1.AppPhase;
using AppSourceMessage = Relay.Platform.V1.AppSource;
using AppSpecMessage = Relay.Platform.V1.AppSpec;
using AppStatusMessage = Relay.Platform.V1.AppStatus;
using ExposureMessage = Relay.Platform.V1.Exposure;
using ImageSourceMessage = Relay.Platform.V1.ImageSource;
using ResourcesMessage = Relay.Common.V1.Resources;

namespace Relay.Platform.Api.Apps;

internal static class AppMapping
{
    public static AppMessage ToMessage(this App app) => new()
    {
        Id = app.Id.ToString(),
        ProjectId = app.ProjectId.ToString(),
        Name = app.Name.Value,
        Spec = app.Spec.ToMessage(),
        Status = new AppStatusMessage
        {
            Phase = app.Status.Phase.ToMessage(),
            CurrentReleaseId = app.Status.CurrentRelease?.ToString() ?? "",
            ObservedAt = Timestamp.FromDateTimeOffset(app.Status.ObservedAt),
        },
        CreatedAt = Timestamp.FromDateTimeOffset(app.CreatedAt),
    };

    public static Result<AppSpec> ToDomain(this AppSpecMessage? spec)
    {
        if (spec?.Source?.KindCase != AppSourceMessage.KindOneofCase.Image)
        {
            return Error.Validation("app.source_required", "The app spec needs an image source.");
        }

        var image = ImageReference.Create(spec.Source.Image.Reference);
        if (image.IsFailure)
        {
            return image.Error;
        }

        var exposure = spec.Exposure?.ToDomain();
        if (exposure is { IsFailure: true })
        {
            return exposure.Error;
        }

        var resources = Resources.Create((long)(spec.Resources?.MemoryLimitBytes ?? 0), (int)(spec.Resources?.CpuLimitMillicores ?? 0));
        if (resources.IsFailure)
        {
            return resources.Error;
        }

        return new AppSpec { Source = new ImageSource(image.Value), Exposure = exposure?.Value, Resources = resources.Value };
    }

    private static Result<Exposure> ToDomain(this ExposureMessage exposure)
    {
        var subdomain = ResourceName.Create(exposure.Subdomain);
        return subdomain.IsFailure ? subdomain.Error : Exposure.Create((int)exposure.Port, subdomain.Value);
    }

    private static AppSpecMessage ToMessage(this AppSpec spec)
    {
        var message = new AppSpecMessage
        {
            Source = new AppSourceMessage { Image = new ImageSourceMessage { Reference = ((ImageSource)spec.Source).Reference.Value } },
            Resources = new ResourcesMessage
            {
                MemoryLimitBytes = (ulong)spec.Resources.MemoryLimitBytes,
                CpuLimitMillicores = (uint)spec.Resources.CpuLimitMillicores,
            },
        };
        if (spec.Exposure is { } exposure)
        {
            message.Exposure = new ExposureMessage { Port = (uint)exposure.Port, Subdomain = exposure.Subdomain.Value };
        }

        return message;
    }

    private static AppPhaseMessage ToMessage(this AppPhase phase) => phase switch
    {
        AppPhase.NotDeployed => AppPhaseMessage.NotDeployed,
        AppPhase.Deploying => AppPhaseMessage.Deploying,
        AppPhase.Running => AppPhaseMessage.Running,
        AppPhase.Stopped => AppPhaseMessage.Stopped,
        AppPhase.Crashed => AppPhaseMessage.Crashed,
        _ => AppPhaseMessage.Unspecified,
    };
}
