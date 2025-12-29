using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
        public bool Hidden { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public bool Active { get; set; } = true;

        public string? RefreshTokenHash { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }
        public DateTime? LastLoginOnUtc { get; set; }
        public void SetRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshTokenHash = ComputeSha256Hash(refreshToken);
            RefreshTokenExpiryTime = expiryTime;
        }

        private string ComputeSha256Hash(string rawData)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        public void RevokeRefreshToken()
        {
            RefreshTokenHash = null;
            RefreshTokenExpiryTime = null;
        }

        public void UpdateLastLogin()
        {
            LastLoginOnUtc = DateTime.UtcNow;
        }
        // Navigation properties
        public ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; } = new HashSet<ApplicationUserRole>();
    }
}
