using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.DTOS.Common;

namespace Voyagr.API.Extensions;

public static class ControllerExtensions
{
    public static IActionResult NotFoundError(
        this ControllerBase controller,
        string message)
    {
        return controller.NotFound(
            new ApiErrorResponse
            {
                Message = message,
                Errors = null
            });
    }

    public static IActionResult UnauthorizedError(
        this ControllerBase controller,
        string message = "Unauthorized.")
    {
        return controller.Unauthorized(
            new ApiErrorResponse
            {
                Message = message,
                Errors = null
            });
    }

    public static IActionResult BadRequestError(
        this ControllerBase controller,
        string message)
    {
        return controller.BadRequest(
            new ApiErrorResponse
            {
                Message = message,
                Errors = null
            });
    }
}