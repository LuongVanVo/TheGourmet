using System.Security.Cryptography;

namespace TheGourmet.Application.Interfaces;

public interface IJwtKeyProvider
{
    RSA GetPrivateKey();
    RSA GetPublicKey();
}