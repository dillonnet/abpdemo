using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Config;
using Application.Dto.System.User;
using Application.Permissions;
using Domain.Entity.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Security.Claims;

namespace Application.Service.System;

[Authorize]
public class UserService : BaseService
{
    protected JwtConfig JwtConfig { get; }
    public IPasswordHasher<User> PasswordHasher { get; set; }
    public UserService(IOptions<JwtConfig> jwtconfigOptions)
    {
        JwtConfig = jwtconfigOptions.Value;
    }

    [Authorize(MyPermissions.Users.Default)]
    public  async Task<PagedResultDto<UserListOutput>> GetList(GetUserListInput input)
    {
        var query = DbContext.Set<User>().WhereIf(!input.Filter.IsNullOrEmpty(), u => u.UserName.Contains(input.Filter)).OrderByDescending(u => u.CreationTime);
        var count = await  query.CountAsync();
        var list = await  query.PageBy(input).ToListAsync();

        return new PagedResultDto<UserListOutput>(
            count,
            ObjectMapper.Map<List<User>, List<UserListOutput>>(list)
        );
    }
    
    [AllowAnonymous]
    public async Task<SignInOutput> SignIn(SignInInput input)
    {
        var user = await DbContext.Set<User>()
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
        var user = await DbContext.Set<User>()
            .Include(u => u.Roles).ThenInclude(r => r.Role).FirstAsync(u => u.Id == CurrentUser.Id.Value);

        return ObjectMapper.Map<User, UserInfoOutput>(user);
    }
}
