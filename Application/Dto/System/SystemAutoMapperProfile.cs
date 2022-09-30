using Application.Dto.System.Department;
using Application.Dto.System.Role;
using Application.Dto.System.User;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Application.Dto.System;

public class SystemAutoMapperProfile: Profile, ITransientDependency
{
    public SystemAutoMapperProfile(IStringLocalizerFactory stringLocalizerFactory)
    {
        CreateMap<Domain.Entity.System.User, UserListOutput>();
        CreateMap<Domain.Entity.System.User, UserInfoOutput>();
        
        CreateMap<Domain.Entity.System.Role, RoleListOutput>();
        CreateMap<Domain.Entity.System.Role, RoleDetailOutput>()
            .ForMember(r => r.Permissons, i => i.Ignore());
        
        CreateMap<Domain.Entity.System.Department, DepartmentDto>();
        CreateMap<DepartmentDto, Domain.Entity.System.Department>()
            .ForMember(r => r.CreationTime, i => i.Ignore())
            .ForMember(r => r.Id, i => i.Ignore());
        CreateMap<Domain.Entity.System.Department, TreeOptionDto>()
            .ForMember(r => r.Value, i => i.MapFrom(d => d.Id.ToString())) ;
        
        CreateMap<PermissionDefinition, PermissionOutput>()
            .ForMember(pd => pd.DisplayName, i => i.MapFrom(o => o.DisplayName.Localize(stringLocalizerFactory).Value));
    }
}