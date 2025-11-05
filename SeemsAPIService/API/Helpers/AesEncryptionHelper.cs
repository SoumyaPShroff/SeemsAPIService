
using System.Security.Cryptography;
using System.Text;

public static class AesEncryptionHelper
{
    // Keep this secret and store securely (env var, Key Vault). This is a "password" used to derive keys.
    private static readonly string MasterSecret = Environment.GetEnvironmentVariable("SEEMSV2@2025") ?? "ChangeThisInProd_UseKeyVault";

    // Recommended PBKDF2 parameters (tune iterations based on your server's performance)
    private const int SaltSize = 16;           // 128-bit salt
    private const int KeySize = 32;            // 256-bit AES key
    private const int IvSize = 16;             // 128-bit IV
    private const int Iterations = 100_000;    // modern minimum (increase as appropriate)
    private static readonly HashAlgorithmName HashAlg = HashAlgorithmName.SHA256;

    // Encrypt -> returns Base64 string that contains: [salt (16 bytes)] + [ciphertext bytes]
    public static string EncryptToBase64(string plainText)
    {
        if (plainText is null) throw new ArgumentNullException(nameof(plainText));

        // generate random salt
        byte[] salt = new byte[SaltSize];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        // derive bytes for key+iv in one-shot PBKDF2 call
        // we need KeySize + IvSize bytes
        int totalBytes = KeySize + IvSize;
        byte[] derived = Rfc2898DeriveBytes.Pbkdf2(MasterSecret, salt, Iterations, HashAlg, totalBytes);

        byte[] key = new byte[KeySize];
        byte[] iv = new byte[IvSize];
        Array.Copy(derived, 0, key, 0, KeySize);
        Array.Copy(derived, KeySize, iv, 0, IvSize);

        byte[] cipherBytes;
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC; // default
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.Close();
                cipherBytes = ms.ToArray();
            }
        }

        // Prepend salt to ciphertext so we can derive the same key+iv on decrypt
        byte[] result = new byte[salt.Length + cipherBytes.Length];
        Array.Copy(salt, 0, result, 0, salt.Length);
        Array.Copy(cipherBytes, 0, result, salt.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    // Decrypt a Base64 string produced by EncryptToBase64(...)
    public static string DecryptFromBase64(string base64Input)
    {
        if (base64Input is null) throw new ArgumentNullException(nameof(base64Input));

        byte[] allBytes = Convert.FromBase64String(base64Input);

        if (allBytes.Length < SaltSize)
            throw new ArgumentException("Invalid input (too short)");

        byte[] salt = new byte[SaltSize];
        Array.Copy(allBytes, 0, salt, 0, SaltSize);

        byte[] cipherBytes = new byte[allBytes.Length - SaltSize];
        Array.Copy(allBytes, SaltSize, cipherBytes, 0, cipherBytes.Length);

        int totalBytes = KeySize + IvSize;
        byte[] derived = Rfc2898DeriveBytes.Pbkdf2(MasterSecret, salt, Iterations, HashAlg, totalBytes);

        byte[] key = new byte[KeySize];
        byte[] iv = new byte[IvSize];
        Array.Copy(derived, 0, key, 0, KeySize);
        Array.Copy(derived, KeySize, iv, 0, IvSize);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
                byte[] plainBytes = ms.ToArray();
                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}
