using SynapseVue.Server.Models.Categories;

namespace SynapseVue.Server.Models.Products;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string? Name { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int PIN { get; set; }
    [MaxLength(512)]
    public string? Description { get; set; }
    public DateTimeOffset CreatedOn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastSuccessActivity { get; set; }
    [MaxLength(512)]
    public string? LastReadValue { get; set; }


    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    public int CategoryId { get; set; }
}
