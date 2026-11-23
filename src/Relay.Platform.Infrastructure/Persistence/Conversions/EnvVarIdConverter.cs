using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class EnvVarIdConverter() : ValueConverter<EnvVarId, Guid>(id => id.Value, value => new EnvVarId(value));
