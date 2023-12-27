namespace WebAppDz4.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string? Name { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public string PasswordDk { get; set; } = null!;
        public string? Avatar { get; set; }
        public DateTime RegisterDt { get; set; }
        public DateTime? DeleteDt { get; set; }
    }
}
