using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoFramework;
using MongoFramework.Linq;

// ReSharper disable once CheckNamespace
namespace Finbuckle.MultiTenant.Stores;

public class MongoTenantStore<TTenantInfo> : IMultiTenantStore<TTenantInfo>
    where TTenantInfo : class, ITenantInfo, new()
{
    private readonly IMongoTenantStoreContext _context;

    public MongoTenantStore(IMongoTenantStoreContext context)
    {
        if (context is IMongoDbTenantContext)
        {
            throw new ArgumentException("Context provided to a MongoTenantStore must not be IMongoDbTenantContext", nameof(context));
        }
        _context = context;
    }

    public async Task<bool> TryAddAsync(TTenantInfo tenantInfo)
    {
        _context.Set<TTenantInfo>().Add(tenantInfo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryUpdateAsync(TTenantInfo tenantInfo)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Id == tenantInfo.Id);

        if (existing == null)
        {
            return false;
        }

        existing.Identifier = tenantInfo.Identifier;
        existing.Name = tenantInfo.Name;
        existing.ConnectionString = tenantInfo.ConnectionString;

        _context.Set<TTenantInfo>().Update(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryRemoveAsync(string identifier)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Identifier == identifier);

        if (existing == null)
        {
            return false;
        }

        _context.Set<TTenantInfo>().Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TTenantInfo> TryGetByIdentifierAsync(string identifier)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Identifier == identifier);

        return existing;
    }

    public async Task<TTenantInfo> TryGetAsync(string id)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Id == id);

        return existing;
    }

    public async Task<IEnumerable<TTenantInfo>> GetAllAsync()
    {
        return await _context.Set<TTenantInfo>().ToListAsync();
    }
}
