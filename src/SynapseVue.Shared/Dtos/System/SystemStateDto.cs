namespace SynapseVue.Shared.Dtos.System;

[DtoResourceType(typeof(AppStrings))]
public class SystemStateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [Display(Name = nameof(AppStrings.Name))]
    [MaxLength(64, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    public string? Property { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [MaxLength(64, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    public string? Value { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [Display(Name = nameof(AppStrings.Description))]
    [MaxLength(256, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    public string? Description { get; set; }
}
