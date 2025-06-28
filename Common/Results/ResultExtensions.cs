using Microsoft.AspNetCore.Mvc;

namespace BT3_TH.Common.Results;

/// <summary>
/// Extension methods for converting Result objects to ActionResult for ASP.NET Core controllers
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a Result to an appropriate ActionResult
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                201 => new StatusCodeResult(201), // Created
                204 => new NoContentResult(), // No Content
                _ => new OkResult() // OK
            };
        }

        return result.StatusCode switch
        {
            400 => new BadRequestObjectResult(new { error = result.ErrorMessage }),
            401 => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
            404 => new NotFoundObjectResult(new { error = result.ErrorMessage }),
            422 => new UnprocessableEntityObjectResult(new { error = result.ErrorMessage }),
            _ => new ObjectResult(new { error = result.ErrorMessage }) { StatusCode = result.StatusCode }
        };
    }

    /// <summary>
    /// Converts a Result<T> to an appropriate ActionResult
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                201 => new CreatedResult(string.Empty, result.Data), // Created
                _ => new OkObjectResult(result.Data) // OK
            };
        }

        return result.StatusCode switch
        {
            400 => new BadRequestObjectResult(new { error = result.ErrorMessage }),
            401 => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
            404 => new NotFoundObjectResult(new { error = result.ErrorMessage }),
            422 => new UnprocessableEntityObjectResult(new { error = result.ErrorMessage }),
            _ => new ObjectResult(new { error = result.ErrorMessage }) { StatusCode = result.StatusCode }
        };
    }

    /// <summary>
    /// Converts a Result to ActionResult for non-generic controllers
    /// </summary>
    public static IActionResult ToIActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                201 => new StatusCodeResult(201), // Created
                204 => new NoContentResult(), // No Content
                _ => new OkResult() // OK
            };
        }

        return result.StatusCode switch
        {
            400 => new BadRequestObjectResult(new { error = result.ErrorMessage }),
            401 => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
            404 => new NotFoundObjectResult(new { error = result.ErrorMessage }),
            422 => new UnprocessableEntityObjectResult(new { error = result.ErrorMessage }),
            _ => new ObjectResult(new { error = result.ErrorMessage }) { StatusCode = result.StatusCode }
        };
    }

    /// <summary>
    /// Converts a Result<T> to IActionResult for non-generic controllers
    /// </summary>
    public static IActionResult ToIActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.StatusCode switch
            {
                201 => new CreatedResult(string.Empty, result.Data), // Created
                _ => new OkObjectResult(result.Data) // OK
            };
        }

        return result.StatusCode switch
        {
            400 => new BadRequestObjectResult(new { error = result.ErrorMessage }),
            401 => new UnauthorizedObjectResult(new { error = result.ErrorMessage }),
            404 => new NotFoundObjectResult(new { error = result.ErrorMessage }),
            422 => new UnprocessableEntityObjectResult(new { error = result.ErrorMessage }),
            _ => new ObjectResult(new { error = result.ErrorMessage }) { StatusCode = result.StatusCode }
        };
    }
} 