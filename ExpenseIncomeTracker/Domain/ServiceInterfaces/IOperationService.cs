using Domain.Models;

namespace Domain.ServiceInterfaces;

public interface IOperationService
{
    Task<IEnumerable<Operation>> GetAllOperations();
    Task<IEnumerable<Operation>> GetAllByCategory(Guid categoryId);
    Task<Operation> GetOperationById(Guid id);
    Task<Operation> CreateOperation(Operation operation);
    Task<Operation> UpdateOperationAsync(Guid id, Operation operation);
    Task<bool> DeleteOperation(Guid id);
    Task<IEnumerable<Operation>> GetOperationsByDate(DateOnly date);
    Task<IEnumerable<Operation>> GetOperationsByPeriod(DateOnly start, DateOnly end);
    decimal GetTotalIncomes(IEnumerable<Operation> operations);
    decimal GetTotalExpenses(IEnumerable<Operation> operations);
}