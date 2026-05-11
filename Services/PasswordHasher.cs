using System.Security.Cryptography;

namespace ACE_Deployment_Tracking.Services
{
    public static class PasswordHasher
    {
        // PBKDF2 with SHA-256, stored as: {iterations}.{saltBase64}.{hashBase64}
        public static string Hash(string password)
        {
            const int iterations = 100_000;
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);
            return $"{iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;
            int iterations = int.Parse(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] expected = Convert.FromBase64String(parts[2]);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] actual = pbkdf2.GetBytes(expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
    }
}