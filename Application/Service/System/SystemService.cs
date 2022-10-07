using Application.Dto;
using Application.Dto.System;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;

namespace Application.Service.System;

public class SystemService : ApplicationService
{
    public IPermissionDefinitionManager PermissionDefinitionManager { get; set; }

    public async Task<ICollection<TreeOptionDto>> GetPermissions()
    {
        var permissions = PermissionDefinitionManager.GetGroups()[0].Permissions;
        return ObjectMapper.Map<IReadOnlyCollection<PermissionDefinition>, ICollection<TreeOptionDto>>(permissions);
    }
}