namespace TCG.AuthenticationService.Domain;

public class UserState
{
    public int Id { get; set; }
    public string State { get; set; }
    public ICollection<User> Users { get; set; }
}