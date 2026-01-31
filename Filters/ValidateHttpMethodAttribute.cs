using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DrugStockWeb.Filters
{
    public class ValidateHttpMethodAttribute : ActionFilterAttribute
    {
        public string[] AllowedMethods { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var method = request.HttpMethod;

            // بررسی متدهای POST و PUT برای Content-Length
            if ((method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                 method.Equals("PUT", StringComparison.OrdinalIgnoreCase)))
            {
                if (request.Headers["Content-Length"] == null || request.ContentLength <= 0)
                {
                    filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.LengthRequired, "Content-Length Required");
                    return;
                }
            }

            // بررسی متدهای مجاز
            if (AllowedMethods != null && !AllowedMethods.Contains(method, StringComparer.OrdinalIgnoreCase))
            {
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.MethodNotAllowed, "Method Not Allowed");
            }

            base.OnActionExecuting(filterContext);
        }
    }
}