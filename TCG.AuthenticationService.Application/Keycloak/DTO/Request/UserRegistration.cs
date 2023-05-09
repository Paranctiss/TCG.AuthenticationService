namespace TCG.CatalogService.Application.Keycloak.DTO.Request;

public class UserRegistration
{
    public Guid Sub { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Adress { get; set; }
    public string City { get; set; }
    public int CountryId { get; set; }
    public string PostalCode { get; set; }
    public int UserStateId { get; set; }
}