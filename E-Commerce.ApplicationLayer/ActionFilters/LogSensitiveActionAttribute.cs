
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace E_Commerce.ApplicationLayer.ActionFilters
{
    public class LogSensitiveActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.WriteLine("Sensitive Data");
        }
    }
}
