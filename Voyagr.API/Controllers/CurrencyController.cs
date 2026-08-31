using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.Interfaces;
using CurrencyEnum = Voyagr.Domain.Enums.Currency;

namespace Voyagr.API.Controllers;

[ApiController]
[Route("api/v1/currency")]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrencyController(
        ICurrencyService currencyService)
    {
        _currencyService = currencyService;
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
}