using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;

namespace Domain.Data;

public class MyEfCoreRepositoryRegistrar: RepositoryRegistrarBase<AbpDbContextRegistrationOptions>
{
    public MyEfCoreRepositoryRegistrar(AbpDbContextRegistrationOptions options) : base(options)
    {
    }

    protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
    {
        return dbContextType.Assembly.GetTypes().Where(x => typeof(IEntity).IsAssignableFrom(x));
    }

    protected override Type GetRepositoryType(Type dbContextType, Type entityType)
    {
        return typeof(EfCoreRepository<,>).MakeGenericType(dbContextType, entityType);
    }

    protected override Type GetRepositoryType(Type dbContextType, Type entityType, Type primaryKeyType)
    {
        return typeof(EfCoreRepository<,,>).MakeGenericType(dbContextType, entityType, primaryKeyType);
    }
}