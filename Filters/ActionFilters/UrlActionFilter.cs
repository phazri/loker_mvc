using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TempDtoShared.Utility;

namespace LokerMVC.Filters.ActionFilters;

public class UrlActionFilter(IOptions<ApiOptions> options, HttpClient httpClient) : IActionFilter
{
    private readonly string _urlMain = options.Value.UrlMySite?.Replace("https:", "").TrimEnd('/');
    private readonly string _urlApi = options.Value.ServiceApiLoker!.TrimEnd('/');
    
    public void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Items["UrlMain"] = _urlMain;
        context.HttpContext.Items["UrlApi"] = _urlApi;
    }

    public  void OnActionExecuted(ActionExecutedContext context)
    {
       
    }
}