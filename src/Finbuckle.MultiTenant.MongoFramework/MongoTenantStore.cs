using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;
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

    public async Task<bool> AddAsync(TTenantInfo tenantInfo)
    {
        _context.Set<TTenantInfo>().Add(tenantInfo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(TTenantInfo tenantInfo)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Id == tenantInfo.Id);

        if (existing == null)
        {
            return false;
        }

        _context.Set<TTenantInfo>().Update(tenantInfo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveAsync(string identifier)
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

    public async Task<TTenantInfo?> GetByIdentifierAsync(string identifier)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Identifier == identifier);

        return existing;
    }

    public async Task<TTenantInfo?> GetAsync(string id)
    {
        var existing = await _context.Set<TTenantInfo>()
            .SingleOrDefaultAsync(ti => ti.Id == id);

        return existing;
    }

    public async Task<IEnumerable<TTenantInfo>> GetAllAsync()
    {
        return await _context.Set<TTenantInfo>().ToListAsync();
    }

    public async Task<IEnumerable<TTenantInfo>> GetAllAsync(int take, int skip)
    {
        return await _context.Set<TTenantInfo>().AsQueryable().Skip(skip).Take(take).ToListAsync();
    }
}
