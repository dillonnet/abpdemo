using Domain.Data;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace Application.Service;

public class BaseService : ApplicationService
{
    public MyDbContext DbContext { get; set; }
}


