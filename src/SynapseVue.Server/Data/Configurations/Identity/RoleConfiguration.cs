using SynapseVue.Server.Models.Identity;

namespace SynapseVue.Server.Data.Configurations.Identity;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData
        (
            new Role
            {
                Id = 1,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1"
            },
            new Role
            {
                Id = 2,
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = "1"
            }
        );
    }
}

