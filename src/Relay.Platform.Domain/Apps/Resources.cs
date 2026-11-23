using Relay.Chassis.Primitives;

namespace Relay.Platform.Domain.Apps;

public sealed record Resources
{
    private Resources(long memoryLimitBytes, int cpuLimitMillicores)
    {
        MemoryLimitBytes = memoryLimitBytes;
        CpuLimitMillicores = cpuLimitMillicores;
    }

    public long MemoryLimitBytes { get; }

    public int CpuLimitMillicores { get; }

    public static Result<Resources> Create(long memoryLimitBytes, int cpuLimitMillicores) =>
        memoryLimitBytes >= 0 && cpuLimitMillicores >= 0
            ? Result.Success(new Resources(memoryLimitBytes, cpuLimitMillicores))
            : Error.Validation("resources.invalid", "Resource limits cannot be negative.");
}
