using System.Data;
using Dapper;
using DataAccess.Data;
using DataAccess.Repository;
using Domain.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Testcontainers.PostgreSql;

namespace ExpenseTrackerTests.IntegrationTests;

public class OperationRepositoryTests :  IAsyncLifetime
{
    private PostgreSqlContainer _container { get; } = new PostgreSqlBuilder().
        WithImage("postgres:15").WithDatabase("expensetracker")
        .WithUsername("postgres").WithPassword("password").Build();
    private IDbConnectionFactory  _connectionFactory;
    private OperationRepository _repository;
    private List<Category> _categories;
    private List<Operation> _operations;
    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        _connectionFactory = new NpgsqlConnectionFactory(connectionString);
        _repository = new OperationRepository(_connectionFactory);
        var initializer = new DataBaseInitializer(connectionString);
        await initializer.InitializeAsync();
        _categories = new List<Category>
        {
            new Category
            {
                Id =  Guid.NewGuid(),
                Name = "Test2",
                CategoryTypes = CategoryTypes.Income,
            },
            new Category 
            { 
                Id =  Guid.NewGuid(),
                Name = "Test",
                CategoryTypes = CategoryTypes.Expense
            }
       };
        _operations = new List<Operation>
        {
            new Operation
            {
                Id = Guid.NewGuid(), Date = new DateOnly(2025, 1, 1),
                Amount = 100, Description = "operation 1", CategoryId = _categories[0].Id
            },
            new Operation
            {
                Id = Guid.NewGuid(), Date = new DateOnly(2025, 2, 1),
                Amount = 200, Description = "operation 2", CategoryId = _categories[1].Id
            }
        };
        Dapper.SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
    }
    
    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task AddOperationAsyn_SholdCreateOperation()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        var newOperation = new Operation
        {
            Date = new DateOnly(2025, 5, 5),
            Amount = 100,
            Description = "New operation",
            CategoryId = _categories[1].Id
        };
        await _repository.AddAsync(newOperation);
        var operations = await _repository.GetAllByCategoryAsync(_categories[1].Id);
        operations.Should().HaveCount(1);
    }

   [Fact]
    public async Task UpdateOperationAsync_SholdUpdateOperation()
    {
        var id = _operations[0].Id;
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        var updatedOperation = new Operation
            {Amount = 200, Date = new DateOnly(2025, 5, 5), 
                Description = "Updated operation", Id = id, CategoryId = _categories[1].Id};
        
        await _repository.UpdateAsync(id,updatedOperation);
        var oprationFromDb = await _repository.GetOperationByIdAsync(id);
        
        oprationFromDb.Amount.Should().Be(updatedOperation.Amount);
    }

    [Fact]
    public async Task DeleteOperationAsync_SholdDeleteOperation()
    {
        var id = _operations[1].Id;
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        
        var Isdeleted =  await _repository.DeleteAsync(id) > 0;
        var oprationFromDb = await _repository.GetOperationByIdAsync(id);
        
        oprationFromDb.Should().BeNull();
        Isdeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetOprationByDate_SholdGetOperationsByDate()
    {
        var date = _operations[1].Date;
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);

        var operationsFromDb = await _repository.GetOperationsByDateAsync(date);
        operationsFromDb.Should().OnlyContain(o => o.Date == date);
        operationsFromDb.Should().OnlyContain(o=> o.Category.Id == _categories[1].Id);
    }

    [Fact]
    public async Task GetOprationByDate_SholdGetOperationsByPeriod()
    {
        var startDate = _operations[0].Date;
        var endDate = _operations[1].Date;
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        
        var operationsFromDb = await _repository.GetOperationsByPeriodAsync(startDate, endDate);
        
        operationsFromDb.Should().HaveCount(_operations.Count);
        operationsFromDb.Should().OnlyContain(o => o.Date == startDate || o.Date == endDate);
    }

    [Fact]
    public async Task GetOperationById_SholdReturn_CorrectOperation()
    {
        var operation = _operations[1];
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        
        var operationFromDb = await _repository.GetOperationByIdAsync(operation.Id);
        
        operationFromDb.Id.Should().Be(operation.Id);
        operationFromDb.Amount.Should().Be(operation.Amount);
        operationFromDb.Description.Should().Be(operation.Description);
    }

    [Fact]
    public async Task GetAllOperations_SholdReturn_OperationList()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        
        var operationsFromDb = await _repository.GetAllOperationsAsync();
        
        operationsFromDb.Count().Should().Be(_operations.Count);
    }
    [Fact]
    public async Task GetOperationsByCategoryId_SholdReturn_OperationList()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        await InsertCategory(connection, _categories);
        await InsertOperation(connection, _operations);
        var updated = new Operation
        {
            Id = _operations[0].Id, Date = _operations[0].Date, Amount = _operations[0].Amount, 
            Description = _operations[0].Description, CategoryId = _categories[1].Id
        };
        var actual = await _repository.UpdateAsync(updated.Id, updated);
        var result = await _repository.GetAllByCategoryAsync(_categories[1].Id);
        result.Should().Contain(r=>r.Id == updated.Id);
        result.Should().Contain(r=>r.CategoryId == updated.CategoryId);

    }
    private async Task InsertOperation( IDbConnection connection, IEnumerable<Operation> operations)
    {
        foreach (var operation in operations)
        {
            await connection.ExecuteAsync("INSERT INTO operations (id, date, amount, description, categoryid) " + 
                                          "VALUES (@Id, @Date, @Amount, @Description, @CategoryId)", operation);
        }
    }
    private async Task InsertCategory(IDbConnection connection, List<Category> categories)
    {
        foreach (var category in categories)
        {
            await connection.ExecuteAsync("INSERT INTO category (id, name, categorytypes) " +
                                          "VALUES (@Id, @Name, @CategoryTypes)", category);
        }
    }
}