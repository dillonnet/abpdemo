using Application.Dto.System.Department;
using Application.Dto.System.Role;
using Application.Dto.System.User;
using Application.Permissions;
using AutoMapper;
using Domain.Entity.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;

namespace Application.Dto.System;

public class SystemAutoMapperProfile: Profile, ITransientDependency
{
    public SystemAutoMapperProfile(IStringLocalizerFactory localizerFactory, IPasswordHasher<Domain.Entity.System.User> passwordHasher)
    {
        CreateMap<Domain.Entity.System.User, UserListOutput>()
            .ForMember(u => u.RoleIds, o => o.MapFrom(u => u.Roles.Select(r => r.RoleId).ToArray()));
        CreateMap<UserInfoCacheItem, UserInfoOutput>();
        CreateMap<Domain.Entity.System.User, UserDetailOutput>()
            .IncludeBase<Domain.Entity.System.User, UserListOutput>();;

        CreateMap<EditUserInput, Domain.Entity.System.User>()
            .ForMember(u => u.Roles, i => i.MapFrom((u, d) => u.RoleIds.Select(r => new UserRole(d.Id, r))));
        CreateMap<CreateUserInput, Domain.Entity.System.User>()
            .IncludeBase<EditUserInput, Domain.Entity.System.User>()
            .ForMember(u => u.PasswordHash, i => i.MapFrom((u, d) => passwordHasher.HashPassword(d, u.Password)));

        
        CreateMap<Domain.Entity.System.Role, RoleListOutput>();
        CreateMap<Domain.Entity.System.Role, RoleDetailOutput>()
            .ForMember(r => r.Permissions, i => i.Ignore());
        CreateMap<CreateOrEditRoleInput, Domain.Entity.System.Role>();
        CreateMap<Domain.Entity.System.Role, DropDownOptionDto>()
            .ForMember(r => r.Value, i => i.MapFrom(d => d.Id.ToString()));


        CreateMap<Domain.Entity.System.Department, DepartmentListOutput>();
        CreateMap<CreateOrEditDepartmentInput, Domain.Entity.System.Department>();
        CreateMap<Domain.Entity.System.Department, TreeOptionDto>()
            .ForMember(r => r.Value, i => i.MapFrom(d => d.Id.ToString())) ;
        
        CreateMap<PermissionDefinition, TreeOptionDto>()
            .ForMember(pd => pd.Value, i => i.MapFrom(o => o.Name))
            .ForMember(pd => pd.Name, i => i.MapFrom(o => o.DisplayName.Localize(localizerFactory)));
    }
}