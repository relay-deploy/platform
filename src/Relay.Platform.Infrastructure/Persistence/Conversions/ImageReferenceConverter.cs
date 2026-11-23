using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class ImageReferenceConverter() : ValueConverter<ImageReference, string>(image => image.Value, value => ImageReference.Create(value).Value);
