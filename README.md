[![Actions Status](https://github.com/JohnCampionJr/Finbuckle.MultiTenant.MongoFramework/workflows/.NET%20Core%20Coverage%20(Ubuntu)/badge.svg)](https://github.com/JohnCampionJr/MiniScaffoldCSharp/actions)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/f2cec74cb05346b88e9bb54488198c54)](https://www.codacy.com/gh/JohnCampionJr/MiniScaffoldCSharp/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=JohnCampionJr/MiniScaffoldCSharp&amp;utm_campaign=Badge_Grade)
[![codecov](https://codecov.io/gh/JohnCampionJr/Finbuckle.MultiTenant.MongoFramework/branch/main/graph/badge.svg?token=OIGAKLPE23)](undefined)

# Finbuckle.MultiTenant.MongoFramework
This is an integration of [MongoFramework](https://github.com/TurnerSoftware/MongoFramework) and [Finbuckle.MultiTenant](https://github.com/Finbuckle/Finbuckle.MultiTenant).

## Features

Working samples are provided for all of the following.

### MongoTenantStore

Uses MongoFramework to provide a store for Finbuckle.MultiTenant.  

Any MongoDbContext can be provided via injection to this store.   
It does not require a specific interface or base class.
  
### Isolated Data Per Tenant
Provides isolated data, either with shared data or database per tenant or both.

The MongoTenantStore allows for an option DefaultConnectionString that is added 
to any tenants that do not have their own ConnectionString

The MongoPerTenantConnection then accepts the ITenantInfo and uses its ConnectionString
to connect to MongoDb.

The MongoPerDbContext passes along the Id from ITenantInfo to be used with a MongoTenantContext
That context (in MongoFramework) provides isolated data access based on the tenant Id.

### MongoDb optimized TenantInfo
TenantInfo optimized for MongoDb (Index on identifier, "Tenants" table name)  
This is completely optional, but provided as a convenience.  You can use any ITenantInfo you'd like.

### Samples
- DataIsolationSample - how to use the data isolation features
- MongoTenantStoreSample - how to implement MongoTenantStore
- CombinedSample - how to use both the MongoTenantStore and data isolation in one project