using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transformers.Model.Services;

namespace Transformers.WebApi.Controllers
{
    [Route("war")]
    [ApiController]
    public class WarController : ControllerBase
    {
        private readonly IWarService _warService;

        public WarController(IWarService warService)
        {
            _warService = warService ?? throw new ArgumentNullException(nameof(warService));
        }


        [HttpGet(Name = "PerformWar")]
        [ProducesResponseType(typeof(IAsyncEnumerable<int>), StatusCodes.Status200OK)]
        public async IAsyncEnumerable<int> Get()
        {
            await foreach (var victor in _warService.PerformWar())
            {
                yield return victor.Id;
            }
        }
    }
}
