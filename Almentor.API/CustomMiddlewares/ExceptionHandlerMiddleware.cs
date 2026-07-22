using Almentor.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Almentor.API.CustomMiddlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
                if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound && !httpContext.Response.HasStarted)
                {
                    var problem = new ProblemDetails()
                    {
                        Title = "error while processing http request end point not found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"EndPoint {httpContext.Request.Path} not found",
                        Instance = httpContext.Request.Path
                    };
                    await httpContext.Response.WriteAsJsonAsync(problem);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "something went wrong");
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var problem = new ProblemDetails()
                {
                    Title = "An Unexpected error occured",
                    Detail = ex.Message,
                    Instance = httpContext.Request.Path,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError,

                    },

                };
                httpContext.Response.StatusCode = problem.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(problem);
            }
        }
    }

    
}
