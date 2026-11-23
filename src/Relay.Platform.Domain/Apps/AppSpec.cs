namespace Relay.Platform.Domain.Apps;

public sealed record AppSpec(AppSource Source, Exposure? Exposure, Resources Resources);
