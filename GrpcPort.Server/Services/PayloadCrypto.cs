using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GrpcPort.Server.Services;

/// <summary>
/// Гибридное шифрование: AES-256-GCM для данных + RSA-OAEP для ключа AES.
/// Формат: [2 байта длины RSA-блока][RSA(key||nonce)][ciphertext||tag].
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

    public static byte[] Decrypt(byte[] package, byte[] serverPfx)
    {
        using var cert = X509CertificateLoader.LoadPkcs12(
            serverPfx,
            password: null,
            keyStorageFlags: X509KeyStorageFlags.EphemeralKeySet);
        using var rsa = cert.GetRSAPrivateKey()
            ?? throw new InvalidOperationException("В PFX нет приватного RSA-ключа.");

        if (package.Length < 2)
            throw new CryptographicException("Повреждённый пакет.");

        var wrappedLen = (package[0] << 8) | package[1];
        if (wrappedLen <= 0 || 2 + wrappedLen + TagSize > package.Length)
            throw new CryptographicException("Некорректная длина RSA-блока.");

        var wrappedKey = package.AsSpan(2, wrappedLen).ToArray();
        var cipherLen = package.Length - 2 - wrappedLen - TagSize;
        var cipher = package.AsSpan(2 + wrappedLen, cipherLen);
        var tag = package.AsSpan(2 + wrappedLen + cipherLen, TagSize);

        var keyMaterial = rsa.Decrypt(wrappedKey, RSAEncryptionPadding.OaepSHA256);
        if (keyMaterial.Length != AesKeySize + NonceSize)
            throw new CryptographicException("Некорректный материал ключа.");

        var key = keyMaterial.AsSpan(0, AesKeySize);
        var nonce = keyMaterial.AsSpan(AesKeySize, NonceSize);
        var plain = new byte[cipherLen];

        using (var aes = new AesGcm(key, TagSize))
        {
            aes.Decrypt(nonce, cipher, tag, plain);
        }

        return plain;
    }
}
