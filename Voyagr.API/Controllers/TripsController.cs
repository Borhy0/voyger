using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyagr.Application.DTOS.Trips;
using Voyagr.Application.Interfaces;

namespace Voyagr.API.Controllers;

[ApiController]
[Route("api/v1/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateTripRequest request)
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
            var trip =
                await _tripService.CreateAsync(
                    userId,
                    request
                );

            return Created(
                $"/api/v1/trips/{trip.Id}",
                new
                {
                    data = trip
                }
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
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

        var trip =
            await _tripService.GetByIdAsync(
                userId,
                id
            );

        if (trip is null)
        {
            return NotFound(new
            {
                message = "Trip not found."
            });
        }

        return Ok(new
        {
            data = trip
        });
    }
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] CreateTripRequest request)
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
            var trip =
                await _tripService.UpdateAsync(
                    userId,
                    id,
                    request
                );

            if (trip is null)
            {
                return NotFound(new
                {
                    message = "Trip not found."
                });
            }

            return Ok(new
            {
                data = trip
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}