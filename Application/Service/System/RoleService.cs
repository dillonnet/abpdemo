using Application.Conts;
using Application.Dto.System.Role;
using Application.Dto.System.User;
using Application.Permissions;
using Domain.Entity.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Z.EntityFramework.Plus;

namespace Application.Service.System;

public class RoleService : BaseService
{
    [Authorize(MyPermissions.Roles.Default)]
    public  async Task<PagedResultDto<RoleListOutput>> GetList(GetUserListInput input)
    {
        var query = DbContext.Set<Role>().WhereIf(!input.Filter.IsNullOrEmpty(), u => u.Name.Contains(input.Filter)).OrderByDescending(u => u.Name);
        var count = await  query.CountAsync();
        var list = await  query.PageBy(input).ToListAsync();

        return new PagedResultDto<RoleListOutput>(
            count,
            ObjectMapper.Map<List<Role>, List<RoleListOutput>>(list)
        );
    }
    
    [Authorize(MyPermissions.Roles.Create)]
    public async Task Create(CreateOrEditRoleInput input)
    {
        await CheckRoleNameExist(input.Name, input.Id);
        var role = new Role
        {
            Name = input.Name,
            Remark = input.Remark
        };
        DbContext.Set<Role>().Add(role);
        foreach (var permission in input.Permissions)
        {
            await DbContext.Set<PermissionGrant>().AddAsync(new PermissionGrant()
            {
                ProviderKey = input.Name,
                ProviderName = "R",
                Name = permission,
            });
        }
    }

    public async Task<RoleDetailOutput> Get(Guid id)
    {
        var role = await DbContext.Set<Role>().FindAsync(id);
        if(role == null)
            throw new EntityNotFoundException();

        var roleOutput = ObjectMapper.Map<Role, RoleDetailOutput>(role);
        roleOutput.Permissons = await DbContext.Set<PermissionGrant>().Where(pg =>
                pg.ProviderName == SystemConsts.PERMISSION_PROVIDER_NAME && pg.ProviderKey == role.Name)
            .Select(pg => pg.Name).ToArrayAsync();

        return roleOutput;
    }

    [Authorize(MyPermissions.Roles.Update)]
    public async Task Update(CreateOrEditRoleInput input)
    {
        var role = await DbContext.Set<Role>().FindAsync(input.Id.Value);
        if (role == null)
            throw new EntityNotFoundException();

        role.Name = input.Name;
        role.Remark = input.Remark;

        await DbContext.Set<PermissionGrant>().Where(p => p.ProviderName == SystemConsts.PERMISSION_PROVIDER_NAME && p.ProviderKey == input.Name)
            .DeleteAsync();
        foreach (var permission in input.Permissions)
        {
            await DbContext.Set<PermissionGrant>().AddAsync(new PermissionGrant()
            {
                ProviderKey = input.Name,
                ProviderName = SystemConsts.PERMISSION_PROVIDER_NAME,
                Name = permission,
            });
        }
    }

    [Authorize(MyPermissions.Roles.Delete)]
    public async Task Delete(Guid id)
    {
        var role = await DbContext.Set<Role>().FindAsync(id);
        if(role == null)
            throw new EntityNotFoundException();

         DbContext.Set<Role>().Remove(role);
    }

    private async Task CheckRoleNameExist(string name, Guid? ignoreId)
    {
        var query = DbContext.Set<Role>().Where(r => r.Name == name).WhereIf(ignoreId.HasValue, r => r.Id != ignoreId.Value);
        if (await query.AnyAsync())
            throw new UserFriendlyException("角色名已存在");
    }
}