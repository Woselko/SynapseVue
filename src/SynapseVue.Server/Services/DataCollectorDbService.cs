namespace SynapseVue.Server.Services;

public class DataCollectorDbService
{
    public readonly AppDbContext _context;

    public DataCollectorDbService(AppDbContext context)
    {
        _context = context;
    }
}
