
namespace Domain.Entity.System;

public class UserRole : Volo.Abp.Domain.Entities.Entity
{
    public UserRole(Guid userId, Guid roleId)
    {
        this.UserId = userId;
        this.RoleId = roleId;
    }

    public Guid UserId { get; set; }
    
    public Guid RoleId { get; set; }
    
    public User User { get; set; }
    
    public Role Role { get; set; }
    
    public override object[] GetKeys()
    {
        return new object[] { UserId, RoleId };
    }
}