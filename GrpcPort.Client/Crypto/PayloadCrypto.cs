using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GrpcPort.Client.Crypto;

/// <summary>
/// Гибридное шифрование payload публичным ключом сессионного сертификата.
/// Формат совместим с серверным PayloadCrypto.
/// </summary>
public static class PayloadCrypto
{
    private const int AesKeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    public static byte[] Encrypt(byte[] plain, X509Certificate2 publicCert)
    {
        using var rsa = publicCert.GetRSAPublicKey()
            ?? throw new InvalidOperationException("В сертификате нет RSA-ключа.");

        var key = RandomNumberGenerator.GetBytes(AesKeySize);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipher = new byte[plain.Length];
        var tag = new byte[TagSize];

        using (var aes = new AesGcm(key, TagSize))
        {
            aes.Encrypt(nonce, plain, cipher, tag);
        }

        var keyMaterial = new byte[AesKeySize + NonceSize];
        Buffer.BlockCopy(key, 0, keyMaterial, 0, AesKeySize);
        Buffer.BlockCopy(nonce, 0, keyMaterial, AesKeySize, NonceSize);

        var wrappedKey = rsa.Encrypt(keyMaterial, RSAEncryptionPadding.OaepSHA256);

        var result = new byte[2 + wrappedKey.Length + cipher.Length + TagSize];
        result[0] = (byte)(wrappedKey.Length >> 8);
        result[1] = (byte)wrappedKey.Length;
        Buffer.BlockCopy(wrappedKey, 0, result, 2, wrappedKey.Length);
        Buffer.BlockCopy(cipher, 0, result, 2 + wrappedKey.Length, cipher.Length);
        Buffer.BlockCopy(tag, 0, result, 2 + wrappedKey.Length + cipher.Length, TagSize);
        return result;
    }
}
