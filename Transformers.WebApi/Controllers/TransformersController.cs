using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transformers.Model;
using Transformers.Model.Entities;
using Transformers.Model.Enums;
using Transformers.Model.Services;
using Transformers.WebApi.Dto;

namespace Transformers.WebApi.Controllers
{
    [Route("transformers")]
    [ApiController]
    public class TransformersController : ControllerBase
    {
        private readonly ITransformersDbContext _dbContext;
        private readonly IOverallRatingCalcService _scoreCalculator;
        private readonly IMapper _mapper;


        public TransformersController(ITransformersDbContext dbContext, IOverallRatingCalcService scoreCalculator, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet(Name = "GetTransformers")]
        [ProducesResponseType(typeof(IAsyncEnumerable<TransformerDto>), StatusCodes.Status200OK)]
        public async IAsyncEnumerable<TransformerDto> Get(Allegiance allegiance = Allegiance.Undefined)
        {
            IQueryable<Transformer> query = _dbContext.Transformers.OrderBy(t => t.Name);
            if (allegiance != Allegiance.Undefined)
            {
                query = query.Where(t => t.Allegiance == allegiance);
            }

            await foreach (var t in query.AsAsyncEnumerable())
            {
                yield return _mapper.Map<TransformerDto>(t);
            }
        }

        [HttpGet("{id}", Name = "GetTransformer")]
        [ProducesResponseType(typeof(TransformerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingle(int id)
        {
            var entity = await _dbContext.Transformers.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TransformerDto>(entity));
        }

        [HttpGet("{id}/overallScore", Name = "GetOverallScore")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOverallScore(int id)
        {
            var entity = await _dbContext.Transformers.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var result = await _scoreCalculator.CalculateAsync(entity);
            return Ok(result);
        }

        [HttpPost(Name = "CreateTransformer")]
        [ProducesResponseType(typeof(TransformerDto), StatusCodes.Status200OK)]
        public async Task<TransformerDto> Create(NewTransformerDto dto)
        {
            var entity = _mapper.Map<Transformer>(dto);
            entity.OverallRating = await _scoreCalculator.CalculateAsync(entity);

            await _dbContext.Transformers.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TransformerDto>(entity);
        }

        [HttpPut("{id}", Name = "UpdateTransformer")]
        [ProducesResponseType(typeof(TransformerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateTransformerDto dto)
        {
            var entity = await _dbContext.Transformers.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            if (entity.RowVersion.ToString() != dto.RowVersion)
            {
                return Conflict();
            }

            _mapper.Map(dto, entity);
            entity.OverallRating = await _scoreCalculator.CalculateAsync(entity);
            await _dbContext.SaveChangesAsync();

            return Ok(_mapper.Map<TransformerDto>(entity));
        }

        [HttpDelete("{id}", Name = "DeleteTransformer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _dbContext.Transformers.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            _dbContext.Transformers.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
