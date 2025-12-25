using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUserRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<ApplicationUser> Users { get; set; } = new HashSet<ApplicationUser>();
    }
}
