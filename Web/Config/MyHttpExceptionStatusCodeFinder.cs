using System.Net;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Modularity;

namespace Web.Config;

public class MyHttpExceptionStatusCodeFinder: DefaultHttpExceptionStatusCodeFinder 
{
    public MyHttpExceptionStatusCodeFinder(IOptions<AbpExceptionHttpStatusCodeOptions> options) : base(options)
    {
    }

    public override HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
    {
        if (exception is IBusinessException)
        {
            return HttpStatusCode.OK;
        }
        return base.GetStatusCode(httpContext, exception);
    }
}