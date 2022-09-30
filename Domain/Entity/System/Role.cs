using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Entity.System;

public class Role: FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    
    public bool IsStatic { get; set; }
    
    public string Remark { get; set; }
}