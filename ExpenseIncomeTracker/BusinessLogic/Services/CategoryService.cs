using Domain.Models;
using Domain.RepositoryInterfaces;
using Domain.ServiceInterfaces;

namespace BusinessLogic.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Category> Create(Category category)
    {
        return await _repository.CreateAsync(category);
    }

    public async Task<Category> GetById(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Category> Update(Guid id, Category category)
    {
        return await _repository.UpdateAsync(id, category);
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        return await _repository.GetAllCategoriesAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesByType(CategoryTypes categoryTypes)
    {
        return await _repository.GetCategotiesByTypeAsync(categoryTypes);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        return result > 0;
    }
}