using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyagr.API.Extensions;
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
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        try
        {
            var trip =
                await _tripService.CreateAsync(
                    userId.Value,
                    request);

            return Created(
                $"/api/v1/trips/{trip.Id}",
                new
                {
                    data = trip
                });
        }
        catch (ArgumentException ex)
        {
            return this.BadRequestError(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var trip =
            await _tripService.GetByIdAsync(
                userId.Value,
                id);

        if (trip is null)
            return this.NotFoundError("Trip not found.");

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
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        try
        {
            var trip =
                await _tripService.UpdateAsync(
                    userId.Value,
                    id,
                    request);

            if (trip is null)
                return this.NotFoundError("Trip not found.");

            return Ok(new
            {
                data = trip
            });
        }
        catch (ArgumentException ex)
        {
            return this.BadRequestError(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        try
        {
            var result =
                await _tripService.GetPagedAsync(
                    userId.Value,
                    page,
                    pageSize);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return this.BadRequestError(ex.Message);
        }
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var trips =
            await _tripService.GetUpcomingAsync(
                userId.Value);

        return Ok(new
        {
            data = trips
        });
    }

    [HttpGet("past")]
    public async Task<IActionResult> GetPast()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var trips =
            await _tripService.GetPastAsync(
                userId.Value);

        return Ok(new
        {
            data = trips
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var deleted =
            await _tripService.DeleteAsync(
                userId.Value,
                id);

        if (!deleted)
            return this.NotFoundError("Trip not found.");

        return Ok(new
        {
            message = "Trip deleted successfully."
        });
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeleted()
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var trips =
            await _tripService.GetDeletedAsync(
                userId.Value);

        return Ok(new
        {
            data = trips
        });
    }

    [HttpPost("{id:guid}/recover")]
    public async Task<IActionResult> Recover(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var recovered =
            await _tripService.RecoverAsync(
                userId.Value,
                id);

        if (!recovered)
            return this.NotFoundError("Trip not found.");

        return Ok(new
        {
            message = "Trip recovered successfully."
        });
    }

    [HttpPatch("{id:guid}/offline")]
    public async Task<IActionResult> UpdateOffline(
        Guid id,
        [FromBody] UpdateTripOfflineRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId is null)
            return this.UnauthorizedError();

        var result =
            await _tripService.UpdateOfflineAsync(
                userId.Value,
                id,
                request);

        if (result is null)
            return this.NotFoundError("Trip not found.");

        return Ok(new
        {
            data = result
        });
    }

    private Guid? GetCurrentUserId()
    {
        var claim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim is null)
            return null;

        return Guid.TryParse(
            claim.Value,
            out var userId)
            ? userId
            : null;
    }
}