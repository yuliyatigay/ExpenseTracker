namespace Domain.Models;

public class Account
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; } = string.Empty;
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
}