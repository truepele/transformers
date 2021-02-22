using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Transformers.Model;
using Transformers.Model.Entities;

namespace Transformers.DataAccess
{
    public class TransformersDbContext: DbContext, ITransformersDbContext
    {
        private static readonly ValueConverter<ulong, byte[]> _rowVersionConverter =
            new(vm => BitConverter.GetBytes(vm), vp => BitConverter.ToUInt64(vp));

        public TransformersDbContext(DbContextOptions<TransformersDbContext> options) : base(options)
        {
        }

        public DbSet<Transformer> Transformers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transformer>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(Transformer.NameMaxLen);
                entity.Property(e => e.RowVersion)
                    .HasConversion(_rowVersionConverter)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasIndex(e => e.Name).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
