using Domain.Models;

namespace Domain.RepositoryInterfaces;

public interface IAccountRepository
{
    Task<Account> RegisterUserAsync(Account account);
    Task<Account> GetByUserName(string username);
}