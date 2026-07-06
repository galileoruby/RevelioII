using RevelioII.DTOs;
using RevelioII.Models;
using RevelioII.Repositories;

namespace RevelioII.Services
{
    public class GraphManagementService : IGraphManagementService
    {
        private readonly IGraphRepository _repository;

        public GraphManagementService(IGraphRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Node>> GetAllNodesAsync() => _repository.GetNodesAsync();

        public async Task<GraphViewDto> GetGraphViewAsync()
        {
            var graph = await _repository.GetGraphAsync();

            return new GraphViewDto
            {
                Nodes = graph.Nodes
                    .Select(node => new GraphNodeDto
                    {
                        Id = node.Id,
                        Label = node.Label,
                        Type = node.Type,
                        Properties = node.Properties
                    })
                    .ToArray(),
                Edges = graph.Relationships
                    .Select(relationship => new GraphEdgeDto
                    {
                        Id = relationship.Id,
                        SourceNodeId = relationship.SourceNodeId,
                        TargetNodeId = relationship.TargetNodeId,
                        RelationType = relationship.RelationType,
                        Properties = relationship.Properties,
                        SourceLabel = relationship.SourceNode?.Label ?? $"Node {relationship.SourceNodeId}",
                        TargetLabel = relationship.TargetNode?.Label ?? $"Node {relationship.TargetNodeId}"
                    })
                    .ToArray()
            };
        }

        public Task<Node?> GetNodeAsync(int id) => _repository.GetNodeByIdAsync(id);

        public Task<Node> CreateNodeAsync(Node node)
        {
            // Business logic/validation could go here
            //var _ = this.CreateNodeSlackAsync(node); // Notify Slack asynchronously without awaiting

            var _ = NotifySlackAsync($"New node created: {node.Label}");

            return _repository.AddNodeAsync(node);
        }

        public Task UpdateNodeAsync(Node node) => _repository.UpdateNodeAsync(node);

        public Task DeleteNodeAsync(int id) => _repository.DeleteNodeAsync(id);

        public Task<IEnumerable<Relationship>> GetAllRelationshipsAsync() => _repository.GetRelationshipsAsync();

        public Task<Relationship?> GetRelationshipAsync(int id) => _repository.GetRelationshipByIdAsync(id);

        public async Task<Relationship> CreateRelationshipAsync(Relationship relationship)
        {
            // Validation: Ensure both Source and Target nodes exist before linking them
            var sourceNode = await _repository.GetNodeByIdAsync(relationship.SourceNodeId);
            var targetNode = await _repository.GetNodeByIdAsync(relationship.TargetNodeId);

            if (sourceNode == null)
                throw new ArgumentException($"Source Node with Id {relationship.SourceNodeId} does not exist.");

            if (targetNode == null)
                throw new ArgumentException($"Target Node with Id {relationship.TargetNodeId} does not exist.");

            return await _repository.AddRelationshipAsync(relationship);
        }

        public Task UpdateRelationshipAsync(Relationship relationship) => _repository.UpdateRelationshipAsync(relationship);

        public Task DeleteRelationshipAsync(int id) => _repository.DeleteRelationshipAsync(id);


        public async Task<Node> CreateNodeSlackAsync(Node node)
        {
            var createdNode = await _repository.AddNodeAsync(node);
            await NotifySlackAsync($"New node created: {node.Label}");
            return createdNode;
        }

        public async Task NotifySlackAsync(string message)
        {
            var webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL") 
                ?? throw new InvalidOperationException("SLACK_WEBHOOK_URL not configured");
            
            using var client = new HttpClient();
            await client.PostAsJsonAsync(webhookUrl, new { text = message });
        }

    }



}
