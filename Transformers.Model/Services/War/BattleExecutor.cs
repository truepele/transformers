using System.Collections.Generic;
using System.Linq;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    internal class BattleExecutor : IBattleExecutor
    {
        private readonly IBattleRule[] _rules;

        public BattleExecutor(IEnumerable<IBattleRule> rules)
        {
            _rules = rules.ToArray();
        }

        public Transformer ExecuteBattle(Transformer t1, Transformer t2)
        {
            if (t1 == null)
            {
                return t2;
            }
            if (t2 == null)
            {
                return t1;
            }

            foreach (var rule in _rules)
            {
                if (rule.CheckIsVictor(t1, t2))
                {
                    return t1;
                }
                if (rule.CheckIsVictor(t2, t1))
                {
                    return t2;
                }
            }

            // TODO: Overall rank comparison rule!

            return null;
        }
    }
}
