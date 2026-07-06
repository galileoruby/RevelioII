namespace RevelioII.Models
{
    public class Node
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Label { get; set; }
        public string? Properties { get; set; } // Stored as JSON string

        // Navigation properties
        public virtual ICollection<Relationship> OutgoingRelationships { get; set; } = new List<Relationship>();
        public virtual ICollection<Relationship> IncomingRelationships { get; set; } = new List<Relationship>();
    }
}
