using Domain.Models;

namespace Domain.RepositoryInterfaces;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);
    Task<Category> GetByIdAsync(Guid id);
    Task<Category> UpdateAsync(Guid id, Category category);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetCategotiesByTypeAsync(CategoryTypes categoryTypes);
    Task<int> DeleteAsync(Guid id);
}