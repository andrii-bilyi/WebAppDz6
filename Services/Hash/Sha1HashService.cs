namespace WebAppDz4.Services.Hash
{
    public class Sha1HashService : IHashService
    {
        public string HexString(string input)
        {
            using var hasher = System.Security.Cryptography.SHA1.Create();
            byte[] bytes = hasher.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(input)
            );
            return Convert.ToHexString(bytes);
        }
    }
}
