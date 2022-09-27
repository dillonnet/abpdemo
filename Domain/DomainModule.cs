using Domain.Data;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.Identity;

namespace Domain;

[DependsOn(
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(InfrastructureModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementDomainIdentityModule)
    )]
public class DomainModule: AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        AbpIdentityDbProperties.DbTablePrefix = "";
        AbpPermissionManagementDbProperties.DbTablePrefix = "";
        
        
        context.Services.AddAbpDbContext<MyDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
        
        Configure<AbpDataFilterOptions>(options =>
        {
            options.DefaultStates[typeof(IMultiTenant)] = new DataFilterState(isEnabled: false);
        });
    }
}