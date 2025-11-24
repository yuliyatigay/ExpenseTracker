using System.Data;
using Dapper;
using DataAccess.Data;
using DataAccess.Repository;
using Domain.Models;
using FluentAssertions;
using Testcontainers.PostgreSql;

namespace ExpenseTrackerTests.IntegrationTests;

public class CategoryRepositoryTests : IAsyncLifetime
{
    private PostgreSqlContainer _container { get; } = new PostgreSqlBuilder().
        WithImage("postgres:15").WithDatabase("expensetracker")
        .WithUsername("postgres").WithPassword("password").Build();
    private IDbConnectionFactory  _connectionFactory;
    private CategoryRepository _repository;
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        _connectionFactory = new NpgsqlConnectionFactory(connectionString);
        _repository = new CategoryRepository(_connectionFactory);
        var initializer = new DataBaseInitializer(connectionString);
        await initializer.InitializeAsync();
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
    }

    public static IEnumerable<object[]> categories => new List<object[]>
    {
        new object[]
        { 
            new List<Category>
            {
                new Category()
                {
                    Id = Guid.NewGuid(), Name = "Income", CategoryTypes = CategoryTypes.Income
                },
                new Category
                {
                    Id = Guid.NewGuid(), Name = "Expense", CategoryTypes = CategoryTypes.Expense
                }
            }
        }
    };
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
    [Fact]
    public async Task CreateAsync()
    {
        var newCategory = new Category
        {
            Name = "Income",
            CategoryTypes = CategoryTypes.Income
        }; 
        
        var actual = await _repository.CreateAsync(newCategory);
        var categoryFromDb = await _repository.GetAllCategoriesAsync();
        
        categoryFromDb.Should().ContainSingle(c=>c.Id == actual.Id);
        categoryFromDb.Should().ContainSingle(c=>c.Name == newCategory.Name);
        categoryFromDb.Should().HaveCount(1);
    }

    [Theory]
    [MemberData(nameof(categories))]
    public async Task GetById_ShouldReturnCorrectCategory(List<Category> categories)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategories(connection, categories);
        
        var categoryFromDb = await _repository.GetByIdAsync(categories[0].Id);
        
        categoryFromDb.Should().BeEquivalentTo(categories[0]);
    }

    [Theory]
    [MemberData(nameof(categories))]
    public async Task Update_ShouldReturnCorrectCategory(List<Category> categories)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategories(connection, categories);
        var updated = new Category
        {
            Id = categories[0].Id, Name = "updated", CategoryTypes = CategoryTypes.Expense,
        };
        
        await _repository.UpdateAsync(updated.Id, updated);
        var categoryFromDb = await _repository.GetByIdAsync(updated.Id);
        
        categoryFromDb.Should().BeEquivalentTo(updated);
    }

    [Theory]
    [MemberData(nameof(categories))]
    public async Task GetAll_ShouldReturnAllCategories(List<Category> categories)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategories(connection, categories);
        
        var categoriesFromDb = await _repository.GetAllCategoriesAsync();
        
        categoriesFromDb.Should().HaveCount(categories.Count);
        categoriesFromDb.Should().ContainSingle(c => c.Id == categories[0].Id);
    }

    [Theory]
    [MemberData(nameof(categories))]
    public async Task GetCategoriesByType_ShouldReturnCorrectCategories(List<Category> categories)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategories(connection, categories);
        var expectedType = categories[0].CategoryTypes;
        
        var categoriesFromDb = await _repository.GetCategotiesByTypeAsync(expectedType);
        
        categoriesFromDb.Should().OnlyContain(c=>c.CategoryTypes == expectedType);
       
    }

    [Theory]
    [MemberData(nameof(categories))]
    public async Task Delete_ShouldDeleteCorrectCategory(List<Category> categories)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategories(connection, categories);
        
        await _repository.DeleteAsync(categories[0].Id);
        var categoryFromDb = await _repository.GetByIdAsync(categories[0].Id);
        var categoriesFromDb = await _repository.GetAllCategoriesAsync();
        
        categoryFromDb.Should().BeNull();
        categoriesFromDb.Should().HaveCount(categories.Count - 1);
    }

    private async Task InsertCategories(IDbConnection connection, List<Category> categories)
    {
        foreach (var category in categories)
        {
            await connection.ExecuteAsync("INSERT INTO category (id, name, categorytypes) " +
                                          "VALUES (@Id, @Name, @CategoryTypes)", category);
        }
    }
    private async Task InsertCategory(IDbConnection connection, Category category)
    {
        await connection.ExecuteAsync("INSERT INTO category (id, name, categorytypes) " +
                                      "VALUES (@Id, @Name, @CategoryTypes)", category);
    }
}