using System;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War.BattleRules
{
    public class StrengthBattleRule : IBattleRule
    {
        public bool CheckIsVictor(Transformer t1, Transformer t2)
        {
            if (t1 == null)
            {
                throw new ArgumentNullException(nameof(t1));
            }

            if (t2 == null)
            {
                throw new ArgumentNullException(nameof(t2));
            }

            return t1.Strength - t2.Strength >= 3 && t2.Courage < 5;
        }
    }
}
