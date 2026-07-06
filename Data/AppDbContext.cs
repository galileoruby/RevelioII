using Microsoft.EntityFrameworkCore;
using RevelioII.Models;

namespace RevelioII.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Node> Nodes { get; set; }
        public DbSet<Relationship> Relationships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Relationship>(entity =>
            {
                entity.HasOne(r => r.SourceNode)
                    .WithMany(n => n.OutgoingRelationships)
                    .HasForeignKey(r => r.SourceNodeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.TargetNode)
                    .WithMany(n => n.IncomingRelationships)
                    .HasForeignKey(r => r.TargetNodeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
