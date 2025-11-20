namespace ExpenseTracker.DTOs;

public class OperationDto
{
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public Guid? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
}