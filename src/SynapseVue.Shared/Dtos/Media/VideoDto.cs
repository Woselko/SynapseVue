using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseVue.Shared.Dtos.Media;

[DtoResourceType(typeof(AppStrings))]
public class VideoDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [MaxLength(64, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.Name))]
    public string Name { get; set; }

    [Display(Name = nameof(AppStrings.IsProcessed))]
    public bool IsProcessed { get; set; } = false;

    [Display(Name = nameof(AppStrings.IsPersonDetected))]
    public bool IsPersonDetected { get; set; } = false;

    [Display(Name = nameof(AppStrings.LastReadValue))]
    public DateTimeOffset? CreatedAt { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [MaxLength(512, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.Name))]
    public string FilePath { get; set; }

    [MaxLength(512, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.DetectedObjects))]
    public string DetectedObjects { get; set; }

    [MaxLength(512, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    [Display(Name = nameof(AppStrings.Description))]
    public string? Description { get; set; }

    [Display(Name = nameof(AppStrings.Size))]
    public long FileSize { get; set; }

}
