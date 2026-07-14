using RevelioII.Models;

namespace RevelioII.Repositories
{
    public interface IGraphRepository
    {
        // Node Operations
        Task<IEnumerable<Node>> GetNodesAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyCollection<Node> Nodes, IReadOnlyCollection<Relationship> Relationships)> GetGraphAsync(CancellationToken cancellationToken = default);
        Task<Node?> GetNodeByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Node> AddNodeAsync(Node node, CancellationToken cancellationToken = default);
        Task UpdateNodeAsync(Node node, CancellationToken cancellationToken = default);
        Task DeleteNodeAsync(int id, CancellationToken cancellationToken = default);

        // Relationship Operations
        Task<IEnumerable<Relationship>> GetRelationshipsAsync(CancellationToken cancellationToken = default);
        Task<Relationship?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Relationship> AddRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default);
        Task UpdateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default);
        Task DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default);
    }
}
