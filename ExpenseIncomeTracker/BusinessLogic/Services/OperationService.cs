using Domain.Models;
using Domain.RepositoryInterfaces;
using Domain.ServiceInterfaces;

namespace BusinessLogic.Services;

public class OperationService : IOperationService
{
    private readonly IOperationRepository _repository;

    public OperationService(IOperationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Operation> CreateOperation(Operation operation)
    {
        return await _repository.AddAsync(operation);
    }

    public async Task<IEnumerable<Operation>> GetAllOperations()
    {
        return await _repository.GetAllOperationsAsync();
    }

    public async Task<IEnumerable<Operation>> GetAllByCategory(Guid categoryId)
    {
        return await _repository.GetAllByCategoryAsync(categoryId);
    }

    public async Task<IEnumerable<Operation>> GetOperationsByDate(DateOnly date)
    {
        return await _repository.GetOperationsByDateAsync(date);
    }

    public async Task<IEnumerable<Operation>> GetOperationsByPeriod(DateOnly start, DateOnly end)
    {
        return await _repository.GetOperationsByPeriodAsync(start, end);
    }

    public async Task<Operation> GetOperationById(Guid id)
    {
        return await _repository.GetOperationByIdAsync(id);
    }

    public async Task<Operation> UpdateOperationAsync(Guid id, Operation operation)
    {
        var updated = await _repository.UpdateAsync(id, operation);
        return updated;
    }

    public async Task<bool> DeleteOperation(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        return result > 0;
    }

    public decimal GetTotalIncomes(IEnumerable<Operation> operations)
    {
        return operations.Where(op =>
            op.Category?.CategoryTypes == CategoryTypes.Income).Sum(op => op.Amount);
    }

    public decimal GetTotalExpenses(IEnumerable<Operation> operations)
    {
        return operations.Where(op =>
            op.Category?.CategoryTypes == CategoryTypes.Expense).Sum(op => op.Amount);
    }
}

