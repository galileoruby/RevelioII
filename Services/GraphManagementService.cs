using RevelioII.DTOs;
using RevelioII.Models;
using RevelioII.Repositories;

namespace RevelioII.Services
{
    public class GraphManagementService : IGraphManagementService
    {
        private readonly IGraphRepository _repository;
        private readonly HttpClient? _httpClient;

        public GraphManagementService(IGraphRepository repository, HttpClient? httpClient = null)
        {
            _repository = repository;
            _httpClient = httpClient;
        }

        public Task<IEnumerable<Node>> GetAllNodesAsync(CancellationToken cancellationToken = default) => _repository.GetNodesAsync(cancellationToken);

        public async Task<GraphViewDto> GetGraphViewAsync(CancellationToken cancellationToken = default)
        {
            var graph = await _repository.GetGraphAsync(cancellationToken);

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

        public Task<Node?> GetNodeAsync(int id, CancellationToken cancellationToken = default) => _repository.GetNodeByIdAsync(id, cancellationToken);

        public async Task<Node> CreateNodeAsync(Node node, CancellationToken cancellationToken = default)
        {
            var createdNode = await _repository.AddNodeAsync(node, cancellationToken);

            try
            {
                await NotifySlackAsync($"New node created: {node.Label}", cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                // Preserve node creation flow when notification delivery fails for reasons other than request cancellation.
            }

            return createdNode;
        }

        public Task UpdateNodeAsync(Node node, CancellationToken cancellationToken = default) => _repository.UpdateNodeAsync(node, cancellationToken);

        public Task DeleteNodeAsync(int id, CancellationToken cancellationToken = default) => _repository.DeleteNodeAsync(id, cancellationToken);

        public Task<IEnumerable<Relationship>> GetAllRelationshipsAsync(CancellationToken cancellationToken = default) => _repository.GetRelationshipsAsync(cancellationToken);

        public Task<Relationship?> GetRelationshipAsync(int id, CancellationToken cancellationToken = default) => _repository.GetRelationshipByIdAsync(id, cancellationToken);

        public async Task<Relationship> CreateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default)
        {
            // Validation: Ensure both Source and Target nodes exist before linking them
            var sourceNode = await _repository.GetNodeByIdAsync(relationship.SourceNodeId, cancellationToken);
            var targetNode = await _repository.GetNodeByIdAsync(relationship.TargetNodeId, cancellationToken);

            if (sourceNode == null)
                throw new ArgumentException($"Source Node with Id {relationship.SourceNodeId} does not exist.");

            if (targetNode == null)
                throw new ArgumentException($"Target Node with Id {relationship.TargetNodeId} does not exist.");

            return await _repository.AddRelationshipAsync(relationship, cancellationToken);
        }

        public Task UpdateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default) => _repository.UpdateRelationshipAsync(relationship, cancellationToken);

        public Task DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default) => _repository.DeleteRelationshipAsync(id, cancellationToken);


        public async Task<Node> CreateNodeSlackAsync(Node node, CancellationToken cancellationToken = default)
        {
            var createdNode = await _repository.AddNodeAsync(node, cancellationToken);
            await NotifySlackAsync($"New node created: {node.Label}", cancellationToken);
            return createdNode;
        }

        public async Task NotifySlackAsync(string message, CancellationToken cancellationToken = default)
        {
            var webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL") 
                ?? throw new InvalidOperationException("SLACK_WEBHOOK_URL not configured");

            var client = _httpClient ?? new HttpClient();

            try
            {
                await client.PostAsJsonAsync(webhookUrl, new { text = message }, cancellationToken);
            }
            finally
            {
                if (_httpClient is null)
                {
                    client.Dispose();
                }
            }
        }

    }



}
