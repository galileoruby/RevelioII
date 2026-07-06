using RevelioII.Models;

namespace RevelioII.Repositories
{
    public interface IGraphRepository
    {
        // Node Operations
        Task<IEnumerable<Node>> GetNodesAsync();
        Task<(IReadOnlyCollection<Node> Nodes, IReadOnlyCollection<Relationship> Relationships)> GetGraphAsync();
        Task<Node?> GetNodeByIdAsync(int id);
        Task<Node> AddNodeAsync(Node node);
        Task UpdateNodeAsync(Node node);
        Task DeleteNodeAsync(int id);

        // Relationship Operations
        Task<IEnumerable<Relationship>> GetRelationshipsAsync();
        Task<Relationship?> GetRelationshipByIdAsync(int id);
        Task<Relationship> AddRelationshipAsync(Relationship relationship);
        Task UpdateRelationshipAsync(Relationship relationship);
        Task DeleteRelationshipAsync(int id);
    }
}
