using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Transformers.Model.Entities;

namespace Transformers.Model.Services.War
{
    internal class BattlePareIterator : IBattlePareIterator
    {
        private readonly ITransformersDbContext _dbContext;
        private readonly BattlePareIteratorSettings _settings;


        public BattlePareIterator(ITransformersDbContext dbContext, IOptions<BattlePareIteratorSettings> options = null)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _settings = options?.Value ?? new BattlePareIteratorSettings { PageSize = 1000 };
        }


        public async IAsyncEnumerable<(Transformer transformer1, Transformer transformer2)> Iterate()
        {
            var page = 0;
            var autobotsHaveNextPage = true;
            var decepticonsHaveNextPage = true;

            while (autobotsHaveNextPage || decepticonsHaveNextPage)
            {
                IEnumerable<Transformer> autobots;
                if (autobotsHaveNextPage)
                {
                    var list = await _dbContext.GetAllAutobots().OrderByDescending(t => t.Rank)
                        .Skip(page * _settings.PageSize).Take(_settings.PageSize)
                        .ToListAsync();
                    autobots = list;
                    autobotsHaveNextPage = list.Count == _settings.PageSize;
                }
                else
                {
                    autobots = Enumerable.Empty<Transformer>();
                }

                IEnumerable<Transformer> decepticons;
                if (decepticonsHaveNextPage)
                {
                    var list = await _dbContext.GetAllDecepticon().OrderByDescending(t => t.Rank)
                        .Skip(page * _settings.PageSize).Take(_settings.PageSize)
                        .ToListAsync();
                    decepticons = list;
                    decepticonsHaveNextPage = list.Count == _settings.PageSize;
                }
                else
                {
                    decepticons = Enumerable.Empty<Transformer>();
                }

                using var aEnumerator = autobots.GetEnumerator();
                using var dEnumerator = decepticons.GetEnumerator();

                aEnumerator.MoveNext();
                dEnumerator.MoveNext();
                while (aEnumerator.Current != null || dEnumerator.Current != null)
                {
                    yield return (aEnumerator.Current, dEnumerator.Current);
                    aEnumerator.MoveNext();
                    dEnumerator.MoveNext();
                }

                page++;
            }
        }
    }
}
