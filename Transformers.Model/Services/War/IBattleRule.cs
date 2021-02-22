using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    public interface IBattleRule
    {
        public bool CheckIsVictor(Transformer t1, Transformer t2);
    }
}