using Application.Permissions;
using Domain.Consts;
using Domain.Data;
using Domain.Entity.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace DbMigrator;

public class ServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public IRepository<Role> RoleRepository { get; set; }
    public IRepository<User> UserRepository { get; set; }
    public IRepository<Department> DepartmentRepository { get; set; }
    public IGuidGenerator GuidGenerator { get; set; }
    public IPasswordHasher<User> PasswordHasher { get; set; }

    private const string SystemMangerRoleStr = "系统设置管理员";
    
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedDepartments();
        await SeedRolesAndPermissions();
        await SeedUsers();
    }
    
    private async Task SeedDepartments()
    {
        var department = new Department()
        {
            Name = "总部",
        };
       await DepartmentRepository.InsertAsync(department, true);
    }

    private async Task SeedRolesAndPermissions()
    {
        var adminRole = new Role
        {
            Name = StaticRoleNames.Admin,
            Remark = "系统内置管理员",
            IsStatic = true
        };
        await RoleRepository.InsertAsync(adminRole, true);
       
        var systemMangerRole = new Role
        {
            Name = SystemMangerRoleStr
        };
        await RoleRepository.InsertAsync(systemMangerRole, true);
    }
    
    private async  Task SeedUsers()
    {
        var password = "123456";
        var adminRole = await RoleRepository.FirstAsync(r => r.Name == StaticRoleNames.Admin);
        
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
            await UserRepository.InsertAsync(newUser, true);
        }
        
        var systemManagerRole = await RoleRepository.FirstAsync(r => r.Name == SystemMangerRoleStr);
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
           await UserRepository.InsertAsync(newUser, true);
        }
    }
}