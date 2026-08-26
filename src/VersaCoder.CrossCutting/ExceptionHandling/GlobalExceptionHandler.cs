using Microsoft.Extensions.Logging;

namespace VersaCoder.CrossCutting.ExceptionHandling;

public class GlobalExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(Exception exception, CancellationToken cancellationToken = default)
    {
        switch (exception)
        {
            case DomainException domainException:
                _logger.LogWarning(domainException, "Domain exception: {Code} - {Message}", 
                    domainException.Code, domainException.Message);
                break;

            case ValidationException validationException:
                _logger.LogWarning(validationException, "Validation failed with {ErrorCount} errors", 
                    validationException.Errors.Count);
                break;

            case NotFoundException notFoundException:
                _logger.LogWarning(notFoundException, "Entity not found: {EntityName} - {Key}", 
                    notFoundException.EntityName, notFoundException.Key);
                break;

            default:
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        await Task.CompletedTask;
    }
}
