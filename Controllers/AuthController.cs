using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrganisationalAuth.Entities;
using OrganisationalAuth.Models;
using OrganisationalAuth.Services;
namespace OrganisationalAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{

    public static User user = new();

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto request)
    {
        var user = await authService.RegisterAsync(request);

        if (user is null)
            return BadRequest("User already exists");

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
    {
        var result = await authService.LoginAsync(request);
        if (result is null)
            return BadRequest("Invalid username or password");

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshTokenAsync(request);
        if (result is null || result.AccessToken is null || result.RefreshToken is null)
            return Unauthorized("Invalid refresh token");

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated!");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminOnlyEndpoint()
    {
        return Ok("You are an admin");
    }


}
