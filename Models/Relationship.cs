namespace RevelioII.Models
{
    public class Relationship
    {
        public int Id { get; set; }
        public int SourceNodeId { get; set; }
        public int TargetNodeId { get; set; }
        public required string RelationType { get; set; }
        public string? Properties { get; set; } // Stored as JSON string

        // Navigation properties
        public virtual Node? SourceNode { get; set; }
        public virtual Node? TargetNode { get; set; }
    }
}
