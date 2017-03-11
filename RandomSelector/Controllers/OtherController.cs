using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RandomSelector.Controllers
{
    public class OtherController : Controller
    {
        // GET: Other
        public ActionResult Index()
        {
			return View("Maintenance");
		}

		public ActionResult Contact()
		{
			return View("Maintenance");
		}
    }
}