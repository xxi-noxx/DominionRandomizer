using RandomSelector.Common;
using RandomSelector.Models.ViewModel;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RandomSelector.Filter
{
	/// <summary>
	/// 例外エラーハンドルFilterAttribute
	/// </summary>
	public class HandleErrorExAttribute : FilterAttribute, IExceptionFilter
	{
		/// <summary>
		/// Exception発生時処理
		/// </summary>
		/// <param name="filterContext"></param>
		public void OnException(ExceptionContext filterContext)
		{
			// 例外処理済み or カスタムエラーが有効な場合は処理無し
			if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) return;

			// StatusCodeの取得
			int statusCode = (int)HttpStatusCode.InternalServerError;
			if (filterContext.Exception is HttpException)
			{
				statusCode = (filterContext.Exception as HttpException).GetHttpCode();
			}

			// Log出力
			LogUtil.OutputMvcExceptionLog(
				statusCode,
				filterContext.RouteData.Values["controller"].ToString(),
				filterContext.RouteData.Values["action"].ToString(),
				filterContext.Exception
			);

			// ActionResultの設定
			filterContext.Result = CreateErrorActionResult(filterContext, statusCode);

			// レスポンス情報の設定
			filterContext.ExceptionHandled = true;
			filterContext.HttpContext.Response.Clear();
			filterContext.HttpContext.Response.StatusCode = statusCode;
			filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
		}

		/// <summary>
		/// エラー用ActionResultの生成
		/// </summary>
		/// <param name="filterContext"></param>
		/// <param name="statusCode"></param>
		/// <returns></returns>
		protected virtual ActionResult CreateErrorActionResult(ExceptionContext filterContext, int statusCode)
		{
			// HandleErrorInfoの作成
			var errorInfo = new HandleErrorInfo(
				filterContext.Exception,
				filterContext.RouteData.Values["controller"].ToString(),
				filterContext.RouteData.Values["action"].ToString());

			// ViewModel生成
			var model = new ErrorViewModel() { ErrorInfo = errorInfo };

			// /Views/Shared/Error.html をViewとして返す
			var result = new ViewResult()
			{
				ViewName = "~/Views/Shared/Error.cshtml",
				ViewData = new ViewDataDictionary<ErrorViewModel>(model)
			};
			result.ViewBag.StatusCode = ((HttpStatusCode)statusCode).ToString();
			return result;
		}
	}
}