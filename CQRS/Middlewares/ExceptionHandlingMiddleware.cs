namespace CQRS.API.Middlewares
{
    using FluentValidation;
    using System.Net;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Default to 500
            var statusCode = (int)HttpStatusCode.InternalServerError;
            object response = new { error = "An unexpected error occurred."  };

            // Handle specific exception types
            if (exception is ValidationException validationEx)
            {
                statusCode = (int)HttpStatusCode.BadRequest; // 400
                response = new
                {
                    message = "Validation failed",
                    errors = validationEx.Errors.Select(x => new
                    {
                        Property = x.PropertyName,
                        Error = x.ErrorMessage
                    })
                };
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }

}
