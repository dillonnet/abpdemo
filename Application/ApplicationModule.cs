using Application.Dto.System;
using Application.Permissions;
using Domain;
using Domain.Entity.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace Application;

[DependsOn(typeof(DomainModule), 
    typeof(AbpDddApplicationModule), typeof(AbpAutoMapperModule))]
public class ApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.Configurators.Add(o =>
            {
                var systemAutoMapperProfile = o.ServiceProvider.GetRequiredService<SystemAutoMapperProfile>();
                o.MapperConfiguration.AddProfile(systemAutoMapperProfile);
            });
        });

        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<MyPermissionValueProvider>();
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Enabled;
        });
    }
}