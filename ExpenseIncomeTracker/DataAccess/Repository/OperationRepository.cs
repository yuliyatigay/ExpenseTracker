using Dapper;
using DataAccess.Data;
using Domain.Models;
using Domain.RepositoryInterfaces;

namespace DataAccess.Repository;

public class OperationRepository : IOperationRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public OperationRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Operation>> GetAllOperationsAsync()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"SELECT 
        o.id AS Id, o.date AS Date, o.amount AS Amount, o.description AS Description, o.categoryid AS CategoryId,
        c.id AS Id, c.name AS Name, c.categorytypes AS CategoryTypes  
        FROM operations o
        LEFT JOIN category c ON c.id = o.categoryid";
        return await connection.QueryAsync<Operation, Category, Operation>(
            sql, (operation, category) =>{operation.Category = category; return operation;});
    }

    public async Task<IEnumerable<Operation>> GetAllByCategoryAsync(Guid categoryId)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"SELECT 
        o.id AS Id, o.date AS Date, o.amount AS Amount, o.description AS Description, o.categoryid AS CategoryId,
        c.id AS Id, c.name AS Name, c.categorytypes AS CategoryTypes  
        FROM operations o
        INNER JOIN category c ON c.id = o.categoryid
        WHERE o.categoryid = @categoryId;";
        return await connection.QueryAsync<Operation, Category, Operation>(
            sql, (operation, category) =>
            { operation.Category = category; return operation;},new { categoryId });
    }

    public async Task<Operation> GetOperationByIdAsync(Guid id)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"SELECT 
        o.id AS Id, o.date AS Date, o.amount AS Amount, o.description AS Description, o.categoryid AS CategoryId,
        c.id AS Id, c.name AS Name, c.categorytypes AS CategoryTypes  
        FROM operations o
        LEFT JOIN category c ON c.id = o.categoryid
        WHERE o.id = @id";
        var operations = await connection.QueryAsync<Operation, Category, Operation>(
            sql, (operation, category) =>
            { operation.Category = category; return operation;},new { id });
        return operations.SingleOrDefault();
    }

    public async Task<IEnumerable<Operation>> GetOperationsByDateAsync(DateOnly date)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"
        SELECT o.id AS Id, o.date AS Date, o.amount AS Amount, o.description AS Description, o.categoryid AS CategoryId,
        c.id AS Id, c.name AS Name, c.categorytypes AS CategoryTypes  
        FROM operations o
        LEFT JOIN category c ON c.id = o.categoryid
        WHERE o.date = @date;";
        return await connection.QueryAsync<Operation, Category, Operation>(
            sql, (operation, category) =>
            {operation.Category = category; return operation;},new { date });
    }

    public async Task<IEnumerable<Operation>> GetOperationsByPeriodAsync(DateOnly start, DateOnly end)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = @"SELECT 
                    o.id AS Id, o.date AS Date, o.amount AS Amount, o.description AS Description, o.categoryid AS CategoryId, 
                    c.id AS Id, c.name AS Name, c.categorytypes AS CategoryTypes 
                    FROM operations o LEFT JOIN category c ON c.id = o.categoryid
                    WHERE date BETWEEN @start AND @end";
        return await connection.QueryAsync<Operation, Category, Operation>(
            sql, (operation, category) =>
            {operation.Category = category; return operation;},new { start, end });
    }

    public async Task<Operation> AddAsync(Operation operation)
    {
        var sql = @"
        INSERT INTO operations (date, amount, description, categoryid)
        VALUES (@Date, @Amount, @Description, @CategoryId) 
        RETURNING id, date, amount, description, categoryid;";
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Operation>(sql, new
        {
            Date = operation.Date,
            Description = operation.Description,
            Amount = operation.Amount,
            CategoryId = operation.CategoryId
        });
    }

    public async Task<Operation> UpdateAsync(Guid id, Operation operation)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "UPDATE operations " +
                  "SET date = @Date, amount = @Amount, " +
                  "description = @Description, categoryid = @CategoryId " +
                  "WHERE id = @Id RETURNING id, date, amount, description, categoryid;";
        return await connection.QuerySingleOrDefaultAsync<Operation>(sql, new
        {
            Id = id,
            Date = operation.Date,
            Description = operation.Description,
            Amount = operation.Amount,
            CategoryId = operation.CategoryId
        });
        
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "DELETE FROM Operations WHERE id = @id";
        return await connection.ExecuteAsync(sql, new { id });
    }

}