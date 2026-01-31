using DrugStockWeb.Filters;
using System.Web.Mvc;

namespace DrugStockWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute()); // <-- Add this line
            filters.Add(new ValidateHttpMethodAttribute { AllowedMethods = new[] { "GET", "POST" } });

            filters.Add(new RequireHttpsAttribute());

        }
    }
}
