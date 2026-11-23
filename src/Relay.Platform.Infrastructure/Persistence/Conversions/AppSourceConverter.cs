using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class AppSourceConverter() : ValueConverter<AppSource, string>(source => AppSourceJson.Write(source), json => AppSourceJson.Read(json));
