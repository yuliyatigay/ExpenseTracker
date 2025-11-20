using Domain.Models;

namespace ExpenseTracker.DTOs;

public class CategoryDto
{
    public string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
}