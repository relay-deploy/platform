using Relay.Platform.Domain.Apps;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Application.Releases;

public interface IReleaseEvents
{
    void Succeeded(App app, Release release);

    void Failed(App app, Release release);
}
