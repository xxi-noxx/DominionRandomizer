using RandomSelector.Models.ViewModel;
using RandomSelector.Service;
using System.Web;
using System.Web.Mvc;

namespace RandomSelector.Controllers
{
	/// <summary>
	/// Indexページ Controller部
	/// </summary>
	public class IndexController : Controller
    {
		/// <summary>Service層インスタンス</summary>
		private IndexService _service;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public IndexController()
		{
			_service = new IndexService();
		}

		/// <summary>
		/// TOPページ(Get時)
		/// </summary>
        [HttpGet]
        public ActionResult Index()
        {
			var model = _service.CreateViewModel();
            return View(model);
        }

		/// <summary>
		/// TOPページ(POST時)
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Index(IndexParam param)
		{
			IndexViewModel model;
			if (!ModelState.IsValid)
			{
				model = _service.CreateViewModel();
				model.Param = param;
				return View(model);
			}

			model = _service.CreateViewModel(param);
			if (model == null)
			{
				model = _service.CreateViewModel();
				model.Param = param;
				return View(model);
			}
			return View(model);
		}
	}
}