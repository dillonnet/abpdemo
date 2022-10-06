using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.EventBus;

namespace Domain.Entity.System;

public class User: FullAuditedAggregateRoot<Guid>
{
    public User()
    {
    }

    public User(Guid id, string userName, string name)
    {
        Id = id;
        UserName = userName;
        Name = name;
        ConcurrencyStamp = Guid.NewGuid().ToString();
        SecurityStamp = Guid.NewGuid().ToString();
        Roles = new Collection<UserRole>();
    }

    public string UserName { get; set; }
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
        
    /// <summary>
    /// 一个随机的字符串，当修改用户认证信息时需要更改，如（密码修改）
    /// </summary>
    public string SecurityStamp { get; set; }
    
    public Guid? DepartmentId { get; set; }
    
    public Department Department { get; set; }
    
    public virtual ICollection<UserRole> Roles { get;  set; }
}