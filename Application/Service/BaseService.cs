using Domain.Data;
using Volo.Abp.Application.Services;

namespace Application.Service;

public class BaseService : ApplicationService
{
    public MyDbContext DbContext { get; set; }
}


