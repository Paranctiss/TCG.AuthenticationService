namespace TCG.AuthenticationService.Domain;

public class User
{
    public int Id { get; set; }
    public Guid Sub { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public DateTime BirthDate { get; set; }
    public string Adress { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime LastConnection { get; set; }
    public int FidelityPoint { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }

    public Country Country { get; set; }
    public int CountryId { get; set; }

    public UserState UserState { get; set; }
    public int UserStateId { get; set; }
}