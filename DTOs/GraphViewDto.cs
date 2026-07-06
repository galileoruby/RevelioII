namespace RevelioII.DTOs
{
    public class GraphViewDto
    {
        public IReadOnlyCollection<GraphNodeDto> Nodes { get; init; } = [];
        public IReadOnlyCollection<GraphEdgeDto> Edges { get; init; } = [];
    }
}
