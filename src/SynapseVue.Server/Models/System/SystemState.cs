namespace SynapseVue.Server.Models.System;

public class SystemState
{
    public int Id { get; set; }
    
    [Required, MaxLength(64)]
    public string Mode { get; set; }
}