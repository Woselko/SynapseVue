namespace SynapseVue.Server.Models.Identity;

public class Role : IdentityRole<int>
{
    public override int Id { get => base.Id; set => base.Id = value; }

    public override string? Name { get => base.Name; set => base.Name = value; }
}

