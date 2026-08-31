using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Voyagr.Application.DTOs.Auth;
using Voyagr.Application.DTOS.Auth;
using Voyagr.Application.Interfaces;

namespace Voyagr.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(
        SignupRequest request)
    {
        try
        {
            var result =
                await _authService
                    .SignupAsync(request);

            return Created(
                string.Empty,
                new
                {
                    message =
                        "User registered successfully",
                    data = result
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

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        try
        {
            var result =
                await _authService
                    .LoginAsync(request);

            return Ok(new
            {
                message =
                    "Login successful",
                data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
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
            var user =
                await _authService
                    .GetCurrentUserAsync(userId);

            return Ok(new
            {
                data = user
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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
    [FromBody] LogoutRequest request)
    {
        await _authService.LogoutAsync(
            request.RefreshToken
        );

        return Ok(new
        {
            message = "Logged out successfully"
        });
    }
}
