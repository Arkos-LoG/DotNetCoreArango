using Microsoft.AspNetCore.Mvc; // for making this class a Controller
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetCoreArango.Controllers
{
    public abstract class BaseController : Controller
    {
        public const string URLHELPER = "URLHELPER"; // public because we need it in ContactUrlResolver

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            context.HttpContext.Items[URLHELPER] = this.Url;// storing it in HttpContext so we have access to URLHELPER for this call somewhere else
        }
    }
}

