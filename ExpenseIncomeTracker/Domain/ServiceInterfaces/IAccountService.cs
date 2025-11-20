using Domain.Models;

namespace Domain.ServiceInterfaces;

public interface IAccountService
{
    Task RegisterAsync(Account account);
    Task<string?> LoginAsync(string username, string password);
}