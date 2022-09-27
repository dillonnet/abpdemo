using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Application.Dtos;

namespace Application.Dto.User;

public class GetUserListInput: PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}