using Domain.Models;

namespace Domain.ServiceInterfaces;

public interface ICategoryService
{
    Task<Category> Create(Category category);
    Task<Category> GetById(Guid id);
    Task<Category> Update(Guid id, Category category);
    Task<IEnumerable<Category>> GetAll();
    Task<IEnumerable<Category>> GetCategoriesByType(CategoryTypes categoryTypes);
    Task<bool> DeleteAsync(Guid id);
}