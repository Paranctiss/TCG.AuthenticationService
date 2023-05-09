namespace TCG.CatalogService.Application.Keycloak.DTO.Request;

public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
}