using RevelioII.Models;
using RevelioII.DTOs;
using RevelioII.Repositories;

namespace RevelioII.Services
{
    public interface IGraphManagementService
    {
        // Nodes
        Task<IEnumerable<Node>> GetAllNodesAsync(CancellationToken cancellationToken = default);
        Task<GraphViewDto> GetGraphViewAsync(CancellationToken cancellationToken = default);
        Task<Node?> GetNodeAsync(int id, CancellationToken cancellationToken = default);
        Task<Node> CreateNodeAsync(Node node, CancellationToken cancellationToken = default);
        Task UpdateNodeAsync(Node node, CancellationToken cancellationToken = default);
        Task DeleteNodeAsync(int id, CancellationToken cancellationToken = default);

        // Relationships
        Task<IEnumerable<Relationship>> GetAllRelationshipsAsync(CancellationToken cancellationToken = default);
        Task<Relationship?> GetRelationshipAsync(int id, CancellationToken cancellationToken = default);
        Task<Relationship> CreateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default);
        Task UpdateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default);
        Task DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default);

        Task<Node> CreateNodeSlackAsync(Node node, CancellationToken cancellationToken = default);
        Task NotifySlackAsync(string message, CancellationToken cancellationToken = default);

    }
}
