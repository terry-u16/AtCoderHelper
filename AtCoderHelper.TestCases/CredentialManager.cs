using MessagePack;
using System.Security.Cryptography;

namespace TerryU16.AtCoderHelper.TestCases;

internal class CredentialManager
{
    const string aesKey = "+GhvQZ#Fu4CAd!P,";
    const int KeyLength = 16;

    public async Task SaveCredentialAsync(Stream stream, LoginCredential credential, CancellationToken ct = default)
    {
        using var aes = Aes.Create();
        var deriveBytes = new Rfc2898DeriveBytes(aesKey, KeyLength);
        var bufferKey = deriveBytes.GetBytes(KeyLength);
        aes.Key = bufferKey;
        aes.GenerateIV();
        var encryptor = aes.CreateEncryptor();

        await stream.WriteAsync(deriveBytes.Salt, ct);
        await stream.WriteAsync(aes.IV, ct);
        var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write);
        await MessagePackSerializer.SerializeAsync(cryptoStream, credential, cancellationToken: ct);
        await cryptoStream.FlushFinalBlockAsync(ct);
    }

    public async Task<LoginCredential> LoadCredendialAsync(Stream stream, CancellationToken ct = default)
    {
        var salt = new byte[KeyLength];
        await stream.ReadAsync(salt, ct);
        var iv = new byte[KeyLength];
        await stream.ReadAsync(iv, ct);

        using var aes = Aes.Create();
        aes.IV = iv;
        var deriveBytes = new Rfc2898DeriveBytes(aesKey, salt);
        var bufferKey = deriveBytes.GetBytes(KeyLength);
        aes.Key = bufferKey;

        var dectyptor = aes.CreateDecryptor();
        var cryptoStream = new CryptoStream(stream, dectyptor, CryptoStreamMode.Read);
        var credential = await MessagePackSerializer.DeserializeAsync<LoginCredential>(cryptoStream, cancellationToken: ct);
        return credential;
    }
}
