using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.EventBus;

namespace Domain.Entity.System;

public class User: FullAuditedAggregateRoot<Guid>
{
    public User()
    {
    }
    
    public const int MAX_LENGTH_USER_NAME = 16;
    public const int MAX_LENGTH_NAME = 16;
    public const int MAX_LENGTH_PASSWORD_HASH = 256;

    public User(Guid id, string userName, string name)
    {
        Id = id;
        UserName = userName;
        Name = name;
        ConcurrencyStamp = Guid.NewGuid().ToString();
        SecurityStamp = Guid.NewGuid().ToString();
        Roles = new Collection<UserRole>();
    }

    [MaxLength(MAX_LENGTH_USER_NAME)]
    public string UserName { get; set; }
    [MaxLength(MAX_LENGTH_NAME)]
    public string Name { get; set; }
    
    [MaxLength(MAX_LENGTH_PASSWORD_HASH)]
    public string PasswordHash { get; set; }
        
    /// <summary>
    /// 一个随机的字符串，当修改用户认证信息时需要更改，如（密码修改）
    /// </summary>
    [MaxLength(64)]
    public string SecurityStamp { get; set; }
    
    public Guid? DepartmentId { get; set; }
    
    public Department Department { get; set; }
    
    public virtual ICollection<UserRole> Roles { get;  set; }
}