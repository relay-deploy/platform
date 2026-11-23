namespace Relay.Platform.Infrastructure.Secrets;

public interface ISecretCipher
{
    string Encrypt(string plaintext);

    string Decrypt(string stored);
}
