namespace RevelioII.DTOs
{
    public class GraphNodeDto
    {
        public int Id { get; set; }
        public required string Label { get; set; }
        public required string Type { get; set; }
        public string? Properties { get; set; }
    }
}
