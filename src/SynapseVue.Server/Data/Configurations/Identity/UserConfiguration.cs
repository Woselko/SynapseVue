using SynapseVue.Server.Models.Identity;

namespace SynapseVue.Server.Data.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        const string userName = "wojciech.wlas@gmail.com";

        builder.HasData([new()
        {
            Id = 1,
            EmailConfirmed = true,
            LockoutEnabled = true,
            Gender = Gender.Male,
            BirthDate = new DateTime(1997, 1, 18),
            FullName = "SynapseVue Admin Account",
            UserName = userName,
            Email = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            NormalizedEmail = userName.ToUpperInvariant(),
            SecurityStamp = "959ff4a9-4b07-4cc1-8141-c5fc033daf83",
            ConcurrencyStamp = "315e1a26-5b3a-4544-8e91-2760cd28e231",
            PasswordHash = "AQAAAAIAAYagAAAAEP0v3wxkdWtMkHA3Pp5/JfS+42/Qto9G05p2mta6dncSK37hPxEHa3PGE4aqN30Aag==", // 123456
            RoleId = 1
        }]);
    }
}
