using AutoMapper;
using Volo.Abp.Identity;

namespace Application.Dto.User;

public class UserAutoMapperProfile: Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<IdentityUser, UserListDto>();
    }
}