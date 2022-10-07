using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using JetBrains.Annotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Domain.Entity.System;

public class Department: FullAuditedAggregateRoot<Guid>
{
    [MaxLength(32)]
    public string Number { get; set; }
    
    [NotNull]
    [MaxLength(32)]
    public string Code { get;  set; }
    
    [NotNull]
    [MaxLength(32)]
    public string Name { get; set; }
    
    public Guid? ParentId { get;  set; }
    
    public Department Parent { get; set; }
    
    public int Sort { get; set; }
    
    public ICollection<Department> Children { get; set; }

    public DataStatus Status { get; set; }
    
    [MaxLength(256)]
    public string Remark { get; set; }
}