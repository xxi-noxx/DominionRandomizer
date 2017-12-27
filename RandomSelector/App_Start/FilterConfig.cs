using RandomSelector.Filter;
using System.Web.Mvc;

namespace RandomSelector.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorExAttribute());
        }
    }
}