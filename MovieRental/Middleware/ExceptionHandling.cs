using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieRental.Movie;
using MovieRental.Rental;
using System.Text.Json;

namespace MovieRental.Middleware
{
    public class ExceptionHandling
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandling> _logger;

        public ExceptionHandling(RequestDelegate next, ILogger<ExceptionHandling> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int status;
            string title;
            string detail = ex.Message;

            // Map known exceptions to HTTP responses
            switch (ex)
            {
                case ArgumentException _:
                    status = StatusCodes.Status400BadRequest;
                    title = "Invalid argument";
                    break;
                case InvalidOperationException _:
                    status = StatusCodes.Status400BadRequest;
                    title = "Invalid operation";
                    break;
                case DbUpdateException _:
                    status = StatusCodes.Status500InternalServerError;
                    title = "Database error";
                    detail = "A database error occurred while processing the request.";
                    break;
                case MovieFailedException _:
                    status = StatusCodes.Status402PaymentRequired;
                    title = "Movie failed";
                    break;
                case PaymentFailedException _:
                    status = StatusCodes.Status402PaymentRequired;
                    title = "Payment failed";
                    break;
                default:
                    status = StatusCodes.Status500InternalServerError;
                    title = "An unexpected error occurred";
                    detail = "An internal server error occurred. Try again later.";
                    break;
            }

            var problem = new ProblemDetails
            {
                Type = "about:blank",
                Title = title,
                Detail = detail,
                Status = status,
                Instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;
            return context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
        }
    } 
}
