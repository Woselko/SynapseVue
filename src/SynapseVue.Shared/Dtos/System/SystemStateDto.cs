namespace SynapseVue.Shared.Dtos.System;

[DtoResourceType(typeof(AppStrings))]
public class SystemStateDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = nameof(AppStrings.RequiredAttribute_ValidationError))]
    [Display(Name = nameof(AppStrings.Mode))]
    [MaxLength(64, ErrorMessage = nameof(AppStrings.MaxLengthAttribute_InvalidMaxLength))]
    public string? Mode { get; set; }
}