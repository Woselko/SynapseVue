using SynapseVue.Server.Models.System;

namespace SynapseVue.Server.Data.Configurations.System;

public class SystemStateConfiguration : IEntityTypeConfiguration<SystemState>
{
    public void Configure(EntityTypeBuilder<SystemState> builder)
    {
        builder.HasData(
            // new() { Id = 1, Mode = "Home"});
            new SystemState { Id = 1, Mode = "Home" });
    }
}