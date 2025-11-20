using Dapper;
using DataAccess.Data;
using Domain.Models;
using Domain.RepositoryInterfaces;

namespace DataAccess.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    public AccountRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<Account> RegisterUserAsync(Account account)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"INSERT INTO accounts(username, firstname, lastname, role, passwordhash)
                VALUES (@UserName, @FirstName, @LastName, @Role, @PasswordHash)
                RETURNING id, username, firstname, lastname, role, passwordhash;";
        return await connection.QuerySingleOrDefaultAsync<Account>(sql, new
        {
            UserName = account.UserName,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Role = account.Role,
            PasswordHash = account.PasswordHash
        });
    }

    public async Task<Account> GetByUserName(string username)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"SELECT 
                    a.userName AS UserName, a.Id AS Id, a.firstname AS FirstName, 
                    a.lastname AS LastName, a.role AS role, a.passwordhash AS passwordhash
                    FROM accounts a WHERE a.username = @username";
        return  await connection.QueryFirstOrDefaultAsync<Account>(sql,  new { username } );
    }
    
}