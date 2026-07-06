using RevelioII.Models;
using RevelioII.DTOs;
using RevelioII.Repositories;

namespace RevelioII.Services
{
    public interface IGraphManagementService
    {
        // Nodes
        Task<IEnumerable<Node>> GetAllNodesAsync();
        Task<GraphViewDto> GetGraphViewAsync();
        Task<Node?> GetNodeAsync(int id);
        Task<Node> CreateNodeAsync(Node node);
        Task UpdateNodeAsync(Node node);
        Task DeleteNodeAsync(int id);

        // Relationships
        Task<IEnumerable<Relationship>> GetAllRelationshipsAsync();
        Task<Relationship?> GetRelationshipAsync(int id);
        Task<Relationship> CreateRelationshipAsync(Relationship relationship);
        Task UpdateRelationshipAsync(Relationship relationship);
        Task DeleteRelationshipAsync(int id);

        Task<Node> CreateNodeSlackAsync(Node node);
        Task NotifySlackAsync(string message);

    }
}
