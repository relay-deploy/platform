using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class AppIdConverter() : ValueConverter<AppId, Guid>(id => id.Value, value => new AppId(value));
