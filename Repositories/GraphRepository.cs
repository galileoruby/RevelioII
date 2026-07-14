using Microsoft.EntityFrameworkCore;
using RevelioII.Data;
using RevelioII.Models;

namespace RevelioII.Repositories
{
    public class GraphRepository : IGraphRepository
    {
        private readonly AppDbContext _context;

        public GraphRepository(AppDbContext context)
        {
            _context = context;
        }

        // Node Operations
        public async Task<IEnumerable<Node>> GetNodesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Nodes.ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyCollection<Node> Nodes, IReadOnlyCollection<Relationship> Relationships)> GetGraphAsync(CancellationToken cancellationToken = default)
        {
            var nodes = await _context.Nodes
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var relationships = await _context.Relationships
                .AsNoTracking()
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .ToListAsync(cancellationToken);

            return (nodes, relationships);
        }

        public async Task<Node?> GetNodeByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Nodes.FindAsync([id], cancellationToken);
        }

        public async Task<Node> AddNodeAsync(Node node, CancellationToken cancellationToken = default)
        {
            _context.Nodes.Add(node);
            await _context.SaveChangesAsync(cancellationToken);
            return node;
        }

        public async Task UpdateNodeAsync(Node node, CancellationToken cancellationToken = default)
        {
            _context.Entry(node).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteNodeAsync(int id, CancellationToken cancellationToken = default)
        {
            var node = await _context.Nodes.FindAsync([id], cancellationToken);
            if (node != null)
            {
                // Relationships linked to this node should be explicitly removed 
                var relationships = await _context.Relationships
                    .Where(r => r.SourceNodeId == id || r.TargetNodeId == id)
                    .ToListAsync(cancellationToken);

                _context.Relationships.RemoveRange(relationships);
                _context.Nodes.Remove(node);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // Relationship Operations
        public async Task<IEnumerable<Relationship>> GetRelationshipsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Relationships
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .ToListAsync(cancellationToken);
        }

        public async Task<Relationship?> GetRelationshipByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Relationships
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<Relationship> AddRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default)
        {
            _context.Relationships.Add(relationship);
            await _context.SaveChangesAsync(cancellationToken);
            return relationship;
        }

        public async Task UpdateRelationshipAsync(Relationship relationship, CancellationToken cancellationToken = default)
        {
            _context.Entry(relationship).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteRelationshipAsync(int id, CancellationToken cancellationToken = default)
        {
            var relationship = await _context.Relationships.FindAsync([id], cancellationToken);
            if (relationship != null)
            {
                _context.Relationships.Remove(relationship);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
