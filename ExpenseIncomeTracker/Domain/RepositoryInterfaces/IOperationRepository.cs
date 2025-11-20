using Domain.Models;

namespace Domain.RepositoryInterfaces;

public interface IOperationRepository
{
    Task<IEnumerable<Operation>> GetAllOperationsAsync();
    Task<IEnumerable<Operation>> GetAllByCategoryAsync(Guid categoryId);
    Task<Operation> GetOperationByIdAsync(Guid id);
    Task<Operation> AddAsync(Operation operation);
    Task<Operation> UpdateAsync(Guid id, Operation operation);
    Task<int> DeleteAsync(Guid id);
    Task<IEnumerable<Operation>> GetOperationsByDateAsync(DateOnly date);
    Task<IEnumerable<Operation>> GetOperationsByPeriodAsync(DateOnly start, DateOnly end);
}