using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Relay.Platform.Infrastructure.Secrets;

namespace Relay.Platform.Infrastructure.Persistence.Conversions;

public sealed class EncryptedStringConverter(ISecretCipher cipher)
    : ValueConverter<string, string>(plain => cipher.Encrypt(plain), stored => cipher.Decrypt(stored));
