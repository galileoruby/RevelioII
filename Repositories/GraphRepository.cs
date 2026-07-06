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
        public async Task<IEnumerable<Node>> GetNodesAsync()
        {
            return await _context.Nodes.ToListAsync();
        }

        public async Task<(IReadOnlyCollection<Node> Nodes, IReadOnlyCollection<Relationship> Relationships)> GetGraphAsync()
        {
            var nodes = await _context.Nodes
                .AsNoTracking()
                .ToListAsync();

            var relationships = await _context.Relationships
                .AsNoTracking()
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .ToListAsync();

            return (nodes, relationships);
        }

        public async Task<Node?> GetNodeByIdAsync(int id)
        {
            return await _context.Nodes.FindAsync(id);
        }

        public async Task<Node> AddNodeAsync(Node node)
        {
            _context.Nodes.Add(node);
            await _context.SaveChangesAsync();
            return node;
        }

        public async Task UpdateNodeAsync(Node node)
        {
            _context.Entry(node).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNodeAsync(int id)
        {
            var node = await _context.Nodes.FindAsync(id);
            if (node != null)
            {
                // Relationships linked to this node should be explicitly removed 
                var relationships = await _context.Relationships
                    .Where(r => r.SourceNodeId == id || r.TargetNodeId == id)
                    .ToListAsync();

                _context.Relationships.RemoveRange(relationships);
                _context.Nodes.Remove(node);
                await _context.SaveChangesAsync();
            }
        }

        // Relationship Operations
        public async Task<IEnumerable<Relationship>> GetRelationshipsAsync()
        {
            return await _context.Relationships
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .ToListAsync();
        }

        public async Task<Relationship?> GetRelationshipByIdAsync(int id)
        {
            return await _context.Relationships
                .Include(r => r.SourceNode)
                .Include(r => r.TargetNode)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Relationship> AddRelationshipAsync(Relationship relationship)
        {
            _context.Relationships.Add(relationship);
            await _context.SaveChangesAsync();
            return relationship;
        }

        public async Task UpdateRelationshipAsync(Relationship relationship)
        {
            _context.Entry(relationship).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRelationshipAsync(int id)
        {
            var relationship = await _context.Relationships.FindAsync(id);
            if (relationship != null)
            {
                _context.Relationships.Remove(relationship);
                await _context.SaveChangesAsync();
            }
        }
    }
}
