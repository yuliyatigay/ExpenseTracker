using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Models;

public class Operation
{
    [SwaggerSchema(ReadOnly = true)]
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
}