using Application.Conts;
using Application.Dto;
using Application.Dto.System.Role;
using Application.Permissions;
using Domain.Entity.System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Z.EntityFramework.Plus;

namespace Application.Service.System;

public class RoleService : MyCrudAppService<Role, RoleDetailOutput, RoleListOutput, Guid, 
    GetRoleListInput, CreateOrEditRoleInput, CreateOrEditRoleInput>
{
    public IRepository<PermissionGrant> PermissionGrantRepository { get; set; }
    public RoleService(IRepository<Role, Guid> repository) : base(repository)
    {
    }
    
    protected override string GetListPolicyName => MyPermissions.Roles.Default;
    protected override string CreatePolicyName => MyPermissions.Roles.Create;
    protected override string UpdatePolicyName => MyPermissions.Roles.Update;
    protected override string DeletePolicyName => MyPermissions.Roles.Delete;
    
    public override async Task<RoleDetailOutput> CreateAsync(CreateOrEditRoleInput input)
    {
        var result = await base.CreateAsync(input);
        foreach (var permission in input.Permissions)
        {
            await PermissionGrantRepository.InsertAsync(new PermissionGrant()
            {
                ProviderKey = result.Id.ToString(),
                ProviderName = SystemConsts.PERMISSION_PROVIDER_NAME,
                Name = permission,
            });
        }
        return result;
    }

    public override async Task<RoleDetailOutput> GetAsync(Guid id)
    {
        var result = await base.GetAsync(id);
        var queryable = await PermissionGrantRepository.GetQueryableAsync();
        result.Permissions = await queryable.Where(pg =>
                pg.ProviderName == SystemConsts.PERMISSION_PROVIDER_NAME && pg.ProviderKey == result.Name)
            .Select(pg => pg.Name).ToArrayAsync();
        return result;
    }

    public override async Task<RoleDetailOutput> UpdateAsync(Guid id, CreateOrEditRoleInput input)
    {
        var result = await base.UpdateAsync(id, input);
        var queryable = await PermissionGrantRepository.GetQueryableAsync();
        await queryable.Where(p => p.ProviderName == SystemConsts.PERMISSION_PROVIDER_NAME && p.ProviderKey == input.Name)
            .DeleteAsync();
        foreach (var permission in input.Permissions)
        {
            await PermissionGrantRepository.InsertAsync(new PermissionGrant()
            {
                ProviderKey = id.ToString(),
                ProviderName = SystemConsts.PERMISSION_PROVIDER_NAME,
                Name = permission,
            });
        }
        return result;
    }
    
    public async  Task<List<DropDownOptionDto>> GetOptions()
    {
        var queryable = await Repository.GetQueryableAsync();
        var roles = await queryable.ToListAsync();
        return ObjectMapper.Map<List<Role>, List<DropDownOptionDto>>(roles);
    }
    
    protected override async Task<IQueryable<Role>> CreateFilteredQueryAsync(GetRoleListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();
        return queryable.WhereIf(!input.Filter.IsNullOrEmpty(), u => u.Name.Contains(input.Filter));
    }
    
    protected override async Task CheckCreateValidateAsync(CreateOrEditRoleInput input)
    {
        await CheckNameExist(input.Name);
    }

    protected override async Task CheckUpdateValidateAsync(Guid id, CreateOrEditRoleInput input)
    {
        await CheckNameExist(input.Name, id);
    }


    private async Task CheckNameExist(string name, Guid? ignoreId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable.Where(r => r.Name == name).WhereIf(ignoreId.HasValue, r => r.Id != ignoreId.Value);
        if (await query.AnyAsync())
            throw new UserFriendlyException("角色名已存在");
    }
}