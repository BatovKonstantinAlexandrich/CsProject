using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GrpcPort.Server.Services;

/// <summary>
/// Выпускает сессионные X.509 сертификаты для шифрования payload клиентских сообщений.
/// </summary>
public sealed class CertificateService
{
    private readonly X509Certificate2 _ca;

    public CertificateService()
    {
        _ca = CreateSelfSignedCa("CN=GrpcPort-CA");
    }

    public IssuedCertificate IssueForClient(string clientId, TimeSpan lifetime)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            $"CN={clientId}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                critical: true));

        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.2") },
                critical: false));

        var notBefore = DateTimeOffset.UtcNow.AddMinutes(-1);
        var notAfter = notBefore.Add(lifetime);

        using var issued = request.Create(
            _ca,
            notBefore,
            notAfter,
            RandomNumberGenerator.GetBytes(16));

        // Экспортируем публичный сертификат без приватного ключа — его клиент использует для шифрования к серверу.
        var publicDer = issued.Export(X509ContentType.Cert);

        // Полный PFX с ключом храним на сервере для расшифровки (если понадобится двустороннее шифрование).
        using var withKey = issued.CopyWithPrivateKey(rsa);
        var serverPfx = withKey.Export(X509ContentType.Pfx, password: string.Empty);

        return new IssuedCertificate(
            PublicCertificateDer: publicDer,
            ServerPfx: serverPfx,
            Thumbprint: issued.Thumbprint,
            ExpiresUnix: notAfter.ToUnixTimeSeconds());
    }

    private static X509Certificate2 CreateSelfSignedCa(string subject)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            subject,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(true, false, 0, true));
        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature,
                true));

        using var cert = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1),
            DateTimeOffset.UtcNow.AddYears(5));

        return X509CertificateLoader.LoadPkcs12(
            cert.Export(X509ContentType.Pfx),
            password: null,
            keyStorageFlags: X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);
    }
}

public sealed record IssuedCertificate(
    byte[] PublicCertificateDer,
    byte[] ServerPfx,
    string Thumbprint,
    long ExpiresUnix);
