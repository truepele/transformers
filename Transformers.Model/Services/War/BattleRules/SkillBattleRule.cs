using System;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War.BattleRules
{
    public class SkillBattleRule : IBattleRule
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

            return t1.Skill - t2.Skill >= 5;
        }
    }
}
