using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Transformers.WebApi.Infrastructure
{
    public class DbUpdateExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is DbUpdateConcurrencyException)
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Title = "Db update concurrency error",
                    Detail = "The entity has been concurrently updated",
                    Status = 409
                })
                {
                    StatusCode = 409,
                };
                context.ExceptionHandled = true;
            }
            else if (context.Exception is DbUpdateException)
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Title = "Db update error",
                    Detail = "The changes appeared to conflict with current state of db",
                    Status = 409
                })
                {
                    StatusCode = 409,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
