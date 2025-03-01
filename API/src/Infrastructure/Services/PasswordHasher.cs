using Application.IServices;
using System.Security.Cryptography;

namespace Infrastructure.Services;
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password , salt , Iterations , hashAlgorithm , HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool IsMatch(string newPassword, string userPassword)
    {
        var parts = userPassword.Split('-');
        var hash = Convert.FromHexString(parts[0]);
        var salt = Convert.FromHexString(parts[1]);

        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(newPassword, salt, Iterations, hashAlgorithm, HashSize);

       // return hash.SequenceEqual(inputHash);
       return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
