namespace SynapseVue.Server.Models.System;

public class SystemState
{
    public int Id { get; set; }
    
    [Required, MaxLength(64)]
    public string Property { get; set; }

    [Required, MaxLength(64)]
    public string Value { get; set; }

    [Required, MaxLength(256)]
    public string Description { get; set; }
}
