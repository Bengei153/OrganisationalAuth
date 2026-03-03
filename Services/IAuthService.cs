using OrganisationalAuth.Entities;
using OrganisationalAuth.Models;

namespace OrganisationalAuth.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto request);
    Task<TokenResponseDto?> LoginAsync (UserDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
}
