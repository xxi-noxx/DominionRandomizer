﻿using RandomSelector.Models.ViewModel;
using RandomSelector.Service;
using System.Web;
using System.Web.Mvc;

namespace RandomSelector.Controllers
{
	/// <summary>
	/// Indexページ Controller部
	/// </summary>
	public class HomeController : Controller
    {
		/// <summary>Service層インスタンス</summary>
		private IndexService _service;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HomeController()
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
		[ValidateAntiForgeryToken]
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

		/// <summary>
		/// 公式推奨セット一覧ページ
		/// </summary>
		/// <returns></returns>
		public ActionResult Recommend()
		{
			return View(new Models.ViewModel.Home.RecommendViewModel());
		}

		public ActionResult Veto()
		{
			return View("Maintenance");
		}
	}
}