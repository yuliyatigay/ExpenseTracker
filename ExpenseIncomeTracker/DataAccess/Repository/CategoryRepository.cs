using Dapper;
using DataAccess.Data;
using Domain.Models;
using Domain.RepositoryInterfaces;

namespace DataAccess.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CategoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var sql = "INSERT INTO category (Name, categorytypes) VALUES (@Name, @CategoryTypes) " +
                        "RETURNING id, name, categorytypes; "; 
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Category>(sql, new
        {
            Name = category.Name,
            CategoryTypes = category.CategoryTypes
        });
    }

    public async Task<Category> UpdateAsync(Guid id, Category category)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "UPDATE category SET name = @Name, categorytypes = @CategoryTypes WHERE id = @id " +
                  "RETURNING id, name, categorytypes; ";
        return await connection.QuerySingleOrDefaultAsync<Category>(sql, new
        {
            id = id,
            Name = category.Name,
            CategoryTypes = category.CategoryTypes
        });
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "SELECT * FROM category";
        return await connection.QueryAsync<Category>(sql);
    }

    public async Task<IEnumerable<Category>> GetCategotiesByTypeAsync(CategoryTypes categoryTypes)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = $"SELECT * FROM category WHERE categorytypes = @categoryTypes";
        var result = await connection.QueryAsync<Category>(sql, new { categoryTypes });
        return result;
    }
    public async Task<Category> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "SELECT * FROM category WHERE id = @id";
        return await connection.QueryFirstOrDefaultAsync<Category>( sql, new { id });
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var sql = "DELETE FROM category WHERE id = @id";
        return await connection.ExecuteAsync(sql, new { id });
    }
}