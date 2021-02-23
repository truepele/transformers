using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    internal class WarService : IWarService
    {
        private readonly ITransformersDbContext _dbContext;
        private readonly IBattleExecutor _battleExecutor;
        private readonly IBattlePareIterator _battlePareIterator;
        private readonly string[] _specialNames;

        public WarService(ITransformersDbContext dbContext,
            IBattleExecutor battleExecutor,
            IBattlePareIterator battlePareIterator,
            IOptions<WarSettings> warOptions)
        {
            _specialNames = warOptions?.Value.SpecialNames ?? throw new ArgumentNullException(nameof(warOptions));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _battleExecutor = battleExecutor ?? throw new ArgumentNullException(nameof(battleExecutor));
            _battlePareIterator = battlePareIterator ?? throw new ArgumentNullException(nameof(battlePareIterator));
        }

        public async IAsyncEnumerable<Transformer> PerformWar()
        {
            var autobotsHaveSpecialName = await _dbContext.GetAllAutobots()
                .Where(a => _specialNames.Contains(a.Name)).AnyAsync();
            var decepticonsHaveSpecialName = await _dbContext.GetAllDecepticon()
                .Where(a => _specialNames.Contains(a.Name)).AnyAsync();
            if (autobotsHaveSpecialName && decepticonsHaveSpecialName)
            {
                yield break;
            }

            await foreach ((Transformer transformer1, Transformer transformer2) in _battlePareIterator.Iterate())
            {
                var victor = _battleExecutor.ExecuteBattle(transformer1, transformer2);
                if (victor != null)
                {
                    yield return victor;
                }
            }
        }
    }
}
