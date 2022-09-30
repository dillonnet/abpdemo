using Application.Permissions;
using Domain.Consts;
using Domain.Data;
using Domain.Entity.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace DbMigrator;

public class ServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public MyDbContext DbContext { get; set; }
    public IGuidGenerator GuidGenerator { get; set; }
    public IPasswordHasher<User> PasswordHasher { get; set; }

    private const string SystemMangerRoleStr = "系统设置管理员";
    
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedRolesAndPermissions();
        await SeedUsers();
    }

    private async Task SeedRolesAndPermissions()
    {
        var adminRole = new Role
        {
            Name = StaticRoleNames.Admin,
            Remark = "系统内置管理员",
            IsStatic = true
        };
        DbContext.Set<Role>().Add(adminRole);
       
        var systemMangerRole = new Role
        {
            Name = SystemMangerRoleStr
        };
        DbContext.Set<Role>().Add(systemMangerRole);

      

        await DbContext.SaveChangesAsync();
    }

    private async  Task SeedUsers()
    {
        var password = "123456";
        var adminRole = await DbContext.Set<Role>().FirstAsync(r => r.Name == StaticRoleNames.Admin);
        for (int i = 0; i < 20; i++)
        {
            var userName = "admin" + i;
            var newUser = new User(GuidGenerator.Create(), userName, userName)
            {
                Name = userName,
            };

            newUser.PasswordHash = PasswordHasher.HashPassword(newUser, password);
            newUser.Roles = new List<UserRole>()
            {
                new (newUser.Id, adminRole.Id)
            };
            DbContext.Set<User>().Add(newUser);
        }
        
        var systemManagerRole = await DbContext.Set<Role>().FirstAsync(r => r.Name == SystemMangerRoleStr);
        for (int i = 0; i < 1; i++)
        {
            var userName = "system" + i;
            var newUser = new User(GuidGenerator.Create(), userName, userName)
            {
                Name = userName,
            };
            
            newUser.PasswordHash = PasswordHasher.HashPassword(newUser, password);
            newUser.Roles = new List<UserRole>()
            {
                new (newUser.Id, systemManagerRole.Id)
            };
            DbContext.Set<User>().Add(newUser);
          
        }
        await DbContext.SaveChangesAsync();
    }
}