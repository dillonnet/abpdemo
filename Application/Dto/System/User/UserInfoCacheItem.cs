using Volo.Abp.Caching;

namespace Application.Dto.System.User;

[CacheName("USER:INFO")]
public class UserInfoCacheItem
{
    public string UserName { get; set; }
    
    public string[] Permissions { get; set; }
    
    public string Name { get; set; }
    
    public Guid[] RoleIds { get; set; }
}