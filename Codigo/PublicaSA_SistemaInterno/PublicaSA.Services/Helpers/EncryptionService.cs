using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public EncryptionService(IConfiguration configuration)
        {
            string keyBase64 = configuration["Encryption:Key"]
                ?? throw new InvalidOperationException("La clave de cifrado no está configurada (Encryption:Key).");

            _key = Convert.FromBase64String(keyBase64);

            if (_key.Length != 32)
                throw new ArgumentException("La clave de cifrado debe ser de 32 bytes (256 bits).");
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("El texto a cifrar no puede estar vacío.");

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            using (var aes = new AesGcm(_key, TagSize))
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            byte[] result = new byte[NonceSize + cipherBytes.Length + TagSize];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, result, NonceSize + cipherBytes.Length, TagSize);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[fullCipher.Length - NonceSize - TagSize];

            Buffer.BlockCopy(fullCipher, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(fullCipher, NonceSize, cipherBytes, 0, cipherBytes.Length);
            Buffer.BlockCopy(fullCipher, NonceSize + cipherBytes.Length, tag, 0, TagSize);

            byte[] plainBytes = new byte[cipherBytes.Length];

            using (var aes = new AesGcm(_key, TagSize))
            {
                aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}