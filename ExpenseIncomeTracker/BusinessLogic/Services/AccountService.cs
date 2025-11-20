using Domain.Models;
using Domain.RepositoryInterfaces;
using Domain.ServiceInterfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IJwtService _jwtService;
    private IValidator<Account> _accountValidator;
    public AccountService(IAccountRepository accountRepository, 
        IJwtService jwtService, IValidator<Account> accountValidator)
    {
        _accountRepository = accountRepository;
        _jwtService = jwtService;
        _accountValidator = accountValidator;
    }
    public async Task RegisterAsync(Account account)
    {
        var accountFromDb = await _accountRepository.GetByUserName(account.UserName);
        if (accountFromDb is not null)
            throw new ArgumentException("Account already exists");
        var result = await _accountValidator.ValidateAsync(account);
        if (!result.IsValid)
            throw new ArgumentException(string.Join(",\n", result.Errors.Select
                (e => e.ErrorMessage)));
        var hashed = new PasswordHasher<Account>().HashPassword(account, account.PasswordHash);
        account.PasswordHash = hashed;
        account.Role = "user";
        await _accountRepository.RegisterUserAsync(account);
        
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var account = await _accountRepository.GetByUserName(username);
        if (account is null)
        {
            throw new ArgumentException("Account is not registered");
        }
        var result = new PasswordHasher<Account>()
            .VerifyHashedPassword(account, account.PasswordHash, password);
        
        if (result == PasswordVerificationResult.Failed)
        {
            throw new ArgumentException("Invalid username or password");
        }
        var response = _jwtService.GenerateJwtToken(account);
        return response;
    }
}