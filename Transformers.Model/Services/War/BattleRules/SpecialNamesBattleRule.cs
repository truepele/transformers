using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War.BattleRules
{
    public class SpecialNamesBattleRule : IBattleRule
    {
        private readonly string[] _specialNames;

        public SpecialNamesBattleRule(IOptions<WarSettings> options)
        {
            _specialNames = options?.Value?.SpecialNames ?? throw new ArgumentNullException(nameof(options));
        }

        public bool CheckIsVictor(Transformer t1, Transformer t2)
        {
            if (t1 == null)
            {
                throw new ArgumentNullException(nameof(t1));
            }

            return _specialNames.Contains(t1.Name);
        }
    }
}
