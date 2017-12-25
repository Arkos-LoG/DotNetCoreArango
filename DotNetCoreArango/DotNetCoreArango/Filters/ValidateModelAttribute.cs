using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreArango.Filters
{
    // use OnActionExcuting Context to look at ModelState; 
    // we can put this attribute on the class level for the controller to do validation on the models
    public class ValidateModelAttribute : ActionFilterAttribute                                                 
    {
        // short cuts going to the Action if this fails
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (!context.ModelState.IsValid)
            {
                // represents bad request being returned
                // because we are setting the result here, we are saying don't call the action; we already have the result
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }
    }
}
