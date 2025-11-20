using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Models;

public class Category
{
    [SwaggerSchema (ReadOnly = true)]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CategoryTypes CategoryTypes { get; set; }
    public List<Operation>? Operations { get; set; }
}