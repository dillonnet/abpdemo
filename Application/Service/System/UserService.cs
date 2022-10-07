using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Config;
using Application.Conts;
using Application.Dto.System.User;
using Application.Permissions;
using Domain.Consts;
using Domain.Entity.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Claims;

namespace Application.Service.System;

[Authorize]
public class UserService  : MyCrudAppService<User, UserDetailOutput, UserListOutput, Guid, 
    GetUserListInput, CreateUserInput, EditUserInput>
{
    protected JwtConfig JwtConfig { get; }
    public IPermissionDefinitionManager PermissionDefinitionManager { get; set; }
    public IRepository<PermissionGrant> PermissionGrantRepository { get; set; }
    public IPasswordHasher<User> PasswordHasher { get; set; }
    public IDistributedCache<UserInfoCacheItem, Guid> UserInfoCache{ get; set; }
    public UserService(
        IRepository<User, Guid> repository,
        IOptions<JwtConfig> jwtconfigOptions): base(repository)
    {
        JwtConfig = jwtconfigOptions.Value;
    }
    
    protected override string GetListPolicyName => MyPermissions.Users.Default;
    protected override string CreatePolicyName => MyPermissions.Users.Create;
    protected override string UpdatePolicyName => MyPermissions.Users.Update;
    protected override string DeletePolicyName => MyPermissions.Users.Delete;

    [AllowAnonymous]
    public async Task<SignInOutput> SignIn(SignInInput input)
    {
        var queryable = await Repository.GetQueryableAsync();
        var user = await queryable
            .Include(u => u.Roles).ThenInclude(r => r.Role).Where(u => u.UserName == input.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new UserFriendlyException("用户名不存在");
        }

        if (PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, input.password) != PasswordVerificationResult.Success)
        {
            throw new UserFriendlyException("密码错误");
        }

        var roles = user.Roles.Select(r => r.Role).ToList();
        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(AbpClaimTypes.Role, role.Name));
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = JwtConfig.Issuer,
            Audience = JwtConfig.Audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtConfig.key)),
                SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        return new SignInOutput
        {
            Token = stringToken
        };
    }
    
    public async Task<UserInfoOutput> GetUserInfo()
    {
        var cacheItem = await UserInfoCache.GetOrAddAsync(CurrentUser.Id.Value, async () =>
        {
            var queryable = await Repository.GetQueryableAsync();
            var user = await queryable
                .Include(u => u.Roles).ThenInclude(r => r.Role).FirstAsync(u => u.Id == CurrentUser.Id.Value);

            string[] permissions;
            var roleIds = user.Roles.Select(r => r.RoleId).ToArray();
            if (user.Roles.Any(r => r.Role.IsStatic && r.Role.Name == StaticRoleNames.Admin))
            {
                permissions = PermissionDefinitionManager.GetGroups()[0].GetPermissionsWithChildren().Select(p => p.Name).ToArray();
            }
            else
            {
                var permissionGrantQueryable = await PermissionGrantRepository.GetQueryableAsync();
                var roleStrs = roleIds.Select(r => r.ToString()).ToArray();
                 permissions = await permissionGrantQueryable.Where(pg => roleStrs.Contains(pg.ProviderKey)
                                                                             && pg.ProviderName ==
                                                                             SystemConsts.PERMISSION_PROVIDER_NAME)
                    .Select(pg => pg.Name).ToArrayAsync();
            }

            return new UserInfoCacheItem()
            {
                UserName = user.UserName,
                Permissions = permissions,
                Name = user.Name,
                RoleIds = roleIds,
            };
        }, () => new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) });

        return ObjectMapper.Map<UserInfoCacheItem, UserInfoOutput>(cacheItem);
    }

    protected override async Task<IQueryable<User>> CreateFilteredQueryAsync(GetUserListInput input)
    {
        var queryable = await Repository.GetQueryableAsync();
        return queryable.Include(u => u.Department).Include(u => u.Roles).ThenInclude(r => r.Role).WhereIf(!input.Filter.IsNullOrEmpty(), u => u.Name.Contains(input.Filter));
    }
    
    protected override async Task CheckCreateValidateAsync(CreateUserInput input)
    {
        await CheckUserNameExist(input.UserName);
        await CheckNameExist(input.Name);
    }

    protected override async Task CheckUpdateValidateAsync(Guid id, EditUserInput input)
    {
        await CheckUserNameExist(input.UserName, id);
        await CheckNameExist(input.Name, id);
    }
    
    protected override async Task<User> GetEntityByIdAsync(Guid id)
    {
        var queryable = await Repository.GetQueryableAsync();
        return await queryable.Include(u => u.Roles).ThenInclude(r => r.Role).FirstOrDefaultAsync(u => u.Id == id);
    }
    
    private async Task CheckUserNameExist(string userName, Guid? ignoreId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable.Where(r => r.UserName == userName).WhereIf(ignoreId.HasValue, r => r.Id != ignoreId.Value);
        if (await query.AnyAsync())
            throw new UserFriendlyException("用户名已存在");
    }
    
    private async Task CheckNameExist(string name, Guid? ignoreId = null)
    {
        var queryable = await Repository.GetQueryableAsync();
        var query = queryable.Where(r => r.Name == name).WhereIf(ignoreId.HasValue, r => r.Id != ignoreId.Value);
        if (await query.AnyAsync())
            throw new UserFriendlyException("姓名已存在");
    }
}
