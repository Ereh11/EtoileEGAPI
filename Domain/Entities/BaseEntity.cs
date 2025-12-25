namespace Domain.Entities
{
    public class BaseEntity<IDType> where IDType : struct
    {
        public IDType Id { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOnUtc { get; set; } = DateTime.UtcNow;
        public int? DisplayOrder { get; set; } = 1;
        public bool Hidden { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public bool Locked { get; set; } = false;
        public bool Active { get; set; } = true;
    }
}
