using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Config;
using Application.Dto.Account;
using Application.Dto.User;
using Application.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace Application.Service;

public class UserService : BaseService
{
    protected JwtConfig JwtConfig { get; }
    public IdentityUserManager IdentityUserManager { get; set; }
    public UserService(IOptions<JwtConfig> jwtconfigOptions)
    {
        JwtConfig = jwtconfigOptions.Value;
    }

    [Authorize(MyPermissions.Users.Default)]
    public  async Task<PagedResultDto<UserListDto>> GetListAsync(GetUserListInput input)
    {
        var query = DbContext.Users.WhereIf(!input.Filter.IsNullOrEmpty(), u => u.UserName.Contains(input.Filter)).OrderByDescending(u => u.CreationTime);
        var count = await  query.CountAsync();
        var list = await  query.PageBy(input).ToListAsync();

        return new PagedResultDto<UserListDto>(
            count,
            ObjectMapper.Map<List<IdentityUser>, List<UserListDto>>(list)
        );
    }
    
    [HttpPost]
    public async Task<SignInOutput> SignIn(SignInInput input)
    {
        var user = await DbContext.Users.Include(u => u.Roles).Where(u => u.UserName == input.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new UserFriendlyException("用户名不存在");
        }

        if (!await IdentityUserManager.CheckPasswordAsync(user, input.password))
        {
            throw new UserFriendlyException("密码错误");
        }

        var roles = await IdentityUserManager.GetRolesAsync(user);
        var claims = new List<Claim>()
        {
            new (JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, user.Email),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(AbpClaimTypes.Role, role));
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
}
