using SynapseVue.Server.Models.System;

namespace SynapseVue.Server.Data.Configurations.System;

public class SystemStateConfiguration : IEntityTypeConfiguration<SystemState>
{
    public void Configure(EntityTypeBuilder<SystemState> builder)
    {
        builder.HasData(
            new SystemState { Id = 1, Property = "Mode", Value = "Home", Description = "Actual mode of application" },
            new SystemState { Id = 2, Property = "Video length", Value = "30", Description = "Video lenght during motion detection (seconds)" }
        );
    }
}
