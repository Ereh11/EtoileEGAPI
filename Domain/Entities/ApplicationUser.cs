using Microsoft.AspNetCore.Identity;
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
        public bool Locked { get; set; } = false;
        // Navigation properties
        public ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; } = new HashSet<ApplicationUserRole>();
    }
}
