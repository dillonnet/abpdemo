using Application.Permissions;
using Domain.Consts;
using Domain.Data;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace DbMigrator;

public class ServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public MyDbContext DbContext { get; set; }
    public IGuidGenerator GuidGenerator { get; set; }
    public IdentityUserManager UserManager { get; set; }
    public IdentityRoleManager RoleManager { get; set; }
    public IPermissionManager PermissionManager { get; set; }

    private const string systemMangerRoleStr = "systemManger";
    
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedRolesAndPermissions();
        await SeedUsers();
    }

    private async Task SeedRolesAndPermissions()
    {
        var adminRole = new IdentityRole(GuidGenerator.Create(), StaticRoleNames.Admin)
        {
            IsStatic = true
        };
        await RoleManager.CreateAsync(adminRole);
        
        var systemMangerRole = new IdentityRole(GuidGenerator.Create(), systemMangerRoleStr);
        await RoleManager.CreateAsync(systemMangerRole);

        await  PermissionManager.SetAsync(MyPermissions.Users.Default, RolePermissionValueProvider.ProviderName, systemMangerRoleStr, true);
        await PermissionManager.SetAsync(MyPermissions.Users.Create, RolePermissionValueProvider.ProviderName, systemMangerRoleStr, true);
        await PermissionManager.SetAsync(MyPermissions.Users.Update, RolePermissionValueProvider.ProviderName, systemMangerRoleStr, true);
        await PermissionManager.SetAsync(MyPermissions.Users.Default, RolePermissionValueProvider.ProviderName, systemMangerRoleStr, true);
        await PermissionManager.SetAsync(MyPermissions.Users.Delete, RolePermissionValueProvider.ProviderName, systemMangerRoleStr, true);
    }

    private async  Task SeedUsers()
    {
        for (int i = 0; i < 20; i++)
        {
            var userName = "admin" + i;
            var newUser = new IdentityUser(GuidGenerator.Create(), userName, userName + "@fake.com")
            {
                Name = userName,
            };
            var password = "123456";
            await UserManager.CreateAsync(newUser, password, validatePassword: false);
            await UserManager.AddToRoleAsync(newUser, StaticRoleNames.Admin);
        }

        for (int i = 0; i < 1; i++)
        {
            var userName = "system" + i;
            var newUser = new IdentityUser(GuidGenerator.Create(), userName, userName + "@fake.com")
            {
                Name = userName,
            };
            var password = "123456";
            await UserManager.CreateAsync(newUser, password, validatePassword: false);
            await UserManager.AddToRoleAsync(newUser, systemMangerRoleStr);
          
        }
    }
}