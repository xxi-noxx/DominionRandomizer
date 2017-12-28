using NLog;
using System;
using System.Text;

namespace RandomSelector.Common
{
    /// <summary>
    /// Logクラス
    /// </summary>
    public static class LogUtil
    {
        /// <summary>Loggerメンバ変数</summary>
        private static readonly Logger _logger;

        static LogUtil()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// 例外ログ出力
        /// </summary>
        /// <param name="ex">例外クラス</param>
        public static void OutputMvcExceptionLog(int statusCode, string controllerName, string actionName, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Httpステータス  ：" + statusCode);
            sb.AppendLine("コントローラー名：" + controllerName);
            sb.AppendLine("アクション名    ：" + actionName);
            sb.AppendLine("例外クラス      ：" + ex.GetType().ToString());
            sb.AppendLine("例外メッセージ  ：" + ex.Message);
            sb.AppendLine("StackTrace      ：" + ex.StackTrace);
            _logger.Error(sb.ToString());
        }
    }
}