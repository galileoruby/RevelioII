namespace RevelioII.DTOs
{
    public class GraphEdgeDto
    {
        public int Id { get; set; }
        public int SourceNodeId { get; set; }
        public int TargetNodeId { get; set; }
        public required string RelationType { get; set; }
        public string? Properties { get; set; }
        public string SourceLabel { get; set; } = string.Empty;
        public string TargetLabel { get; set; } = string.Empty;
    }
}
