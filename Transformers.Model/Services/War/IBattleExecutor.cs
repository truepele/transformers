using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    public interface IBattleExecutor
    {
        Transformer ExecuteBattle(Transformer t1, Transformer t2);
    }
}
