using System.Data;
using Dapper;
using DataAccess.Data;
using DataAccess.Repository;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Testcontainers.PostgreSql;

namespace ExpenseTrackerTests.IntegrationTests;

public class AccountRepositoryTests : IAsyncLifetime
{
    private PostgreSqlContainer _container { get; } = new PostgreSqlBuilder().
        WithImage("postgres:15").WithDatabase("expensetracker")
        .WithUsername("postgres").WithPassword("password").Build();
    private IDbConnectionFactory  _connectionFactory;
    private AccountRepository _repository;
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        _connectionFactory = new NpgsqlConnectionFactory(connectionString);
        _repository = new AccountRepository(_connectionFactory);
        var initializer = new DataBaseInitializer(connectionString);
        await initializer.InitializeAsync();
        Dapper.SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
    }

    [Fact]
    public async Task GetUserByName_ShoudReturnCorrectAccount()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var accounts = new List<Account>
        {
            new Account{ FirstName = "John", LastName = "Doe", UserName = "John", PasswordHash ="Password123" },
            new Account{ FirstName = "Anna", LastName = "Smith", UserName = "Anna", PasswordHash ="Password123" },
        };
        await InsertAccounts(connection,accounts);
         
        var accountsFromDb = await _repository.GetByUserName("John");
        Assert.NotNull(accountsFromDb);
        Assert.Equal(accounts[0].FirstName, accountsFromDb.FirstName);
    }
    
    [Fact]
    public async Task AddOperationAsyn_SholdCreateOperation()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var newAccount = new Account
        {
            UserName = "user",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = "Password123"
        };
        await _repository.RegisterUserAsync(newAccount);
        var account = await _repository.GetByUserName(newAccount.UserName);
        account.Should().NotBeNull();
        account.FirstName.Should().Be(newAccount.FirstName);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private string HashPassword(Account account)
    {
        var hashedPass = new PasswordHasher<Account>().HashPassword(account, account.PasswordHash);
        return hashedPass;
    }

    private async Task InsertAccounts(IDbConnection connection, List<Account> accounts)
    {
        const string sql = @"
        INSERT INTO public.accounts (id, username, firstname, lastname, role, passwordhash)
        VALUES (@Id, @UserName, @FirstName, @LastName, @Role, @PasswordHash);";

        foreach (var account in accounts)
        {
            account.Id = Guid.NewGuid();
            account.Role = string.IsNullOrWhiteSpace(account.Role) ? "user" : account.Role;
            account.PasswordHash = HashPassword(account);

            await connection.ExecuteAsync(sql, account);
        }
    }
}