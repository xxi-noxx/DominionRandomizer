using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RandomSelector.Common;
using RandomSelector.Data;

namespace RandomSelector.Models.ViewModel
{
    /// <summary>
    /// 公式推奨セット画面のViewModel
    /// </summary>
    public class RecommendViewModel
    {
        /// <summary>
        /// 推奨セットのDropDownList一覧
        /// </summary>
        public IEnumerable<SelectListItem> RecommendDropDownItems
        {
            get
            {
                SelectListGroup group = null;
                foreach (var item in RecommendData.RecommendSetData.OrderByDescending(x => x.BaseExpansionID).ThenBy(x => x.RecommendSetID))
                {
                    if ((group?.Name ?? "") != item.BaseExpansionID.ToDisplayName())
                    {
                        group = new SelectListGroup() { Name = item.BaseExpansionID.ToDisplayName() };
                    }
                    string text = item.RecommendSetName + (item.BaseExpansionID == item.SubExpansionID ? "" : "（＋" + item.BaseExpansionID.ToDisplayName() + "）");

                    yield return new SelectListItem()
                    {
                        Value = item.RecommendSetID.ToString(),
                        Text = text,
                        Group = group
                    };
                }
            }
        }
    }
}