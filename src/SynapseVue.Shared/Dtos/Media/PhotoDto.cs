using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseVue.Shared.Dtos.Media;

[DtoResourceType(typeof(AppStrings))]
public class PhotoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [MaxLength(64, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.Name))]
    public string Name { get; set; }

    public byte[] Data { get; set; }

    [Display(Name = nameof(AppStrings.LastReadValue))]
    public DateTimeOffset? CreatedAt { get; set; }

    [MaxLength(512, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.Description))]
    public string? Description { get; set; }
}
