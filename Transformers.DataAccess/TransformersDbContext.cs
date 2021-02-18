using Microsoft.EntityFrameworkCore;
using Transformers.Model;
using Transformers.Model.Entities;

namespace Transformers.DataAccess
{
    public class TransformersDbContext: DbContext, ITransformersDbContext
    {
        public TransformersDbContext(DbContextOptions<TransformersDbContext> options) : base(options)
        {
        }

        public DbSet<Transformer> Transformers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transformer>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(Transformer.NameMaxLen);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
