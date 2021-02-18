using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transformers.Model;
using Transformers.Model.Entities;

namespace Transformers.WebApi.Controllers
{
    [Route("transformers")]
    [ApiController]
    public class TransformersController : ControllerBase
    {
        private readonly ITransformersDbContext _dbContext;


        public TransformersController(ITransformersDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        [HttpGet(Name = "GetTransformers")]
        [ProducesResponseType(typeof(IEnumerable<Transformer>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Transformer>> GetAll()
        {
            return await _dbContext.Transformers.ToListAsync();
        }

    }
}
