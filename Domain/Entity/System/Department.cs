using Domain.Enums;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Entity.System;

public class Department: FullAuditedAggregateRoot<Guid>
{
    public string Number { get; set; }
    
    [NotNull]
    public string Code { get;  set; }
    
    [NotNull]
    public string Name { get; set; }
    
    public Guid? ParentId { get;  set; }
    
    public Department Parent { get; set; }
    
    public int Sort { get; set; }
    
    public ICollection<Department> Children { get; set; }

    public DataStatus Status { get; set; }
    
    public string Remark { get; set; }
}