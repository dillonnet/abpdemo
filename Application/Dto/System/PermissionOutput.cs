using System.Collections.ObjectModel;

namespace Application.Dto.System;

public class PermissionOutput
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public ICollection<PermissionOutput> Children { get; set; }
}