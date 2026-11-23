using Relay.Chassis.Primitives;
using Relay.Platform.Domain.Naming;

namespace Relay.Platform.Domain.Apps;

public sealed record Exposure
{
    private Exposure(int port, ResourceName subdomain)
    {
        Port = port;
        Subdomain = subdomain;
    }

    public int Port { get; }

    public ResourceName Subdomain { get; }

    public static Result<Exposure> Create(int port, ResourceName subdomain) =>
        port is >= 1 and <= 65535
            ? Result.Success(new Exposure(port, subdomain))
            : Error.Validation("exposure.invalid_port", $"{port} is not a valid port.");
}
