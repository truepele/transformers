using System.Collections.Generic;
using Transformers.Model.Entities;

namespace Transformers.Model.Services
{
    public interface IWarService
    {
        IAsyncEnumerable<Transformer> PerformWar();
    }
}
