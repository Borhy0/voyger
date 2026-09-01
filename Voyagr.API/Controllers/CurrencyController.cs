using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.DTOS.Currency;
using Voyagr.Application.Interfaces;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.API.Controllers;

[ApiController]
[Route("api/v1/currency")]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly IFavoriteCurrencyService _favoriteCurrencyService;

    public CurrencyController(
    ICurrencyService currencyService,
    IFavoriteCurrencyService favoriteCurrencyService)
    {
        _currencyService = currencyService;
        _favoriteCurrencyService = favoriteCurrencyService;
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert(
        [FromQuery] CurrencyEnum from,
        [FromQuery] CurrencyEnum to,
        [FromQuery] decimal amount)
    {
        try
        {
            var result =
                await _currencyService.ConvertAsync(
                    from,
                    to,
                    amount
                );

            return Ok(new
            {
                data = result
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("trend")]
    public async Task<IActionResult> Trend(
    [FromQuery] CurrencyEnum from,
    [FromQuery] CurrencyEnum to)
    {
        try
        {
            var result =
                await _currencyService.GetTrendAsync(
                    from,
                    to
                );

            return Ok(new
            {
                data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("favorites")]
    public async Task<IActionResult> GetFavorites()
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identity."
            });
        }

        var favorites =
            await _favoriteCurrencyService
                .GetFavoritesAsync(userId);

        return Ok(new
        {
            data = favorites
        });
    }
    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite(
    [FromBody] FavoriteCurrencyRequest request)
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identity."
            });
        }

        try
        {
            var favorite =
                await _favoriteCurrencyService
                    .AddFavoriteAsync(
                        userId,
                        request
                    );

            return Created(
                string.Empty,
                new
                {
                    data = favorite
                }
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }
    [HttpDelete("favorites/{id:guid}")]
    public async Task<IActionResult> DeleteFavorite(
    Guid id)
    {
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value;

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identity."
            });
        }

        try
        {
            await _favoriteCurrencyService
                .DeleteFavoriteAsync(
                    userId,
                    id
                );

            return Ok(new
            {
                message =
                    "Favorite currency pair deleted successfully."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}