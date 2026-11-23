using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.EnvVars;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class EnvVarNameConverter() : ValueConverter<EnvVarName, string>(name => name.Value, value => EnvVarName.Create(value).Value);
