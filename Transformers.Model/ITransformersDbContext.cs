using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Transformers.Model.Entities;
using Transformers.Model.Enums;

namespace Transformers.Model
{
    public interface ITransformersDbContext
    {
        DbSet<Transformer> Transformers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IQueryable<Transformer> GetAllAutobots() => Transformers.Where(t => t.Allegiance == Allegiance.Autobot);
        IQueryable<Transformer> GetAllDecepticon() => Transformers.Where(t => t.Allegiance == Allegiance.Decepticon);
    }
}
