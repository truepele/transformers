using System.Collections.Generic;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    public interface IBattlePareIterator
    {
        IAsyncEnumerable<(Transformer transformer1, Transformer transformer2)> Iterate();
    }
}
