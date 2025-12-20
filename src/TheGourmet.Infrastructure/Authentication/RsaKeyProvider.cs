using System.Security.Cryptography;
using TheGourmet.Application.Interfaces;

namespace TheGourmet.Infrastructure.Authentication;

public class RsaKeyProvider : IJwtKeyProvider
{
    // Đường dẫn lưu file key 
    private readonly string _privateKeyPath = "keys/private_key.pem";
    private readonly string _publicKeyPath = "keys/public_key.pem";

    // Biến lưu cache key trong RAM để đỡ phải đọc file nhiều lần
    private RSA? _rsa;

    public RsaKeyProvider()
    {
        // Constructor chỉ chạy 1 lần duy nhất khi khởi tạo Singleton
        // Tạo thư mục keys nếu chưa tồn tại
        if (!Directory.Exists("keys")) Directory.CreateDirectory("keys");
        if (!File.Exists(_privateKeyPath) || !File.Exists(_publicKeyPath))
        {
            
        }
    }

    public RSA GetPrivateKey()
    {
        // Nếu đã load vào RAM rồi thì trả về luôn
        if (_rsa != null) return _rsa;

        // Chưa load thì load từ file lên RAM
        LoadKeys();
        return _rsa!;
    }

    public RSA GetPublicKey()
    {
        if (_rsa == null) LoadKeys();

        // Tạo một object RSA mới chỉ chứa public key
        var publicKey = RSA.Create();
        publicKey.ImportFromPem(File.ReadAllText(_publicKeyPath));
        return publicKey;
    }

    // Hàm sinh ra cặp khóa RSA và lưu vào file
    private void GenerateAndSaveKeys()
    {
        // Tạo RSA 2048 bit 
        using var rsa = RSA.Create(2048);

        var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
        File.WriteAllText(_privateKeyPath, privateKeyPem);

        var publicKeyPem = rsa.ExportRSAPublicKeyPem();
        File.WriteAllText(_publicKeyPath, publicKeyPem);
    }

    // Hàm đọc khóa từ file lên RAM
    private void LoadKeys()
    {
        if (!File.Exists(_privateKeyPath)) GenerateAndSaveKeys();

        _rsa = RSA.Create();
        var pemContent = File.ReadAllText(_privateKeyPath);

        // Import PEM vào object RSA
        _rsa.ImportFromPem(pemContent);
    }
}