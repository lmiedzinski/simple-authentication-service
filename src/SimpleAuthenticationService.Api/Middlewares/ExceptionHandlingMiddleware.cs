using Microsoft.AspNetCore.Mvc;
using SimpleAuthenticationService.Application.Exceptions;
using SimpleAuthenticationService.Domain.Abstractions;

namespace SimpleAuthenticationService.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
            };

            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
    
    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationError",
                "Validation error",
                "One or more validation errors has occurred",
                validationException.Errors),
            NotFoundException notFoundException => new ExceptionDetails(
                StatusCodes.Status404NotFound,
                "NotFound",
                "Not Found",
                notFoundException.Message,
                null),
            UserAccountNotFoundOrGivenPasswordIsIncorrectException unauthorizedException => new ExceptionDetails(
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                "Unauthorized",
                unauthorizedException.Message,
                null),
            ConcurrentAccessException concurrentAccessException => new ExceptionDetails(
                StatusCodes.Status409Conflict,
                "ConcurrentAccess",
                "Concurrent Access",
                concurrentAccessException.Message,
                null),
            LoginAlreadyTakenException loginAlreadyTakenException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "LoginAlreadyTaken",
                "Login Already Taken",
                loginAlreadyTakenException.Message,
                null),
            DomainException domainException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "BusinessLogicError",
                "Business Logic Error",
                domainException.Message,
                null),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server error",
                "An unexpected error has occurred",
                null)
        };
    }
    
    private record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}