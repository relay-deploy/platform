using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Naming;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class ResourceNameConverter() : ValueConverter<ResourceName, string>(name => name.Value, value => ResourceName.Create(value).Value);
