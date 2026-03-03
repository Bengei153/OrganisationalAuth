namespace OrganisationalAuth.Models;

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string role {  get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Organisation { get; set; } = string.Empty;
}
