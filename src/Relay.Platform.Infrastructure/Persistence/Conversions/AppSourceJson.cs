using System.Text.Json;
using Relay.Platform.Domain.Apps;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

internal static class AppSourceJson
{
    private const string ImageKind = "image";

    private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

    public static string Write(AppSource source) => source switch
    {
        ImageSource image => JsonSerializer.Serialize(new AppSourceDocument(ImageKind, image.Reference.Value), options),
        _ => throw new ArgumentOutOfRangeException(nameof(source), source, null),
    };

    public static AppSource Read(string json)
    {
        var document = JsonSerializer.Deserialize<AppSourceDocument>(json, options) ?? throw new JsonException("The app source is empty.");
        return document.Kind switch
        {
            ImageKind => new ImageSource(ImageReference.Create(document.Reference).Value),
            _ => throw new JsonException($"Unknown app source kind {document.Kind}."),
        };
    }
}
