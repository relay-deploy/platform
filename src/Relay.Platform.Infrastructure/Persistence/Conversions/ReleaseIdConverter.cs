using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Releases;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class ReleaseIdConverter() : ValueConverter<ReleaseId, Guid>(id => id.Value, value => new ReleaseId(value));
