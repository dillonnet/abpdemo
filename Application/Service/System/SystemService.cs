using Application.Dto.System;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;

namespace Application.Service.System;

public class SystemService : ApplicationService
{
    public IPermissionDefinitionManager PermissionDefinitionManager { get; set; }

    public async Task<ICollection<PermissionOutput>> GetPermissions()
    {
        var permissions = PermissionDefinitionManager.GetGroups()[0].Permissions;
        return ObjectMapper.Map<IReadOnlyCollection<PermissionDefinition>, ICollection<PermissionOutput>>(permissions);
    }
}