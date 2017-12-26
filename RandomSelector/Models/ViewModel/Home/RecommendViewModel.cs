using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RandomSelector.Common;
using RandomSelector.Data;

namespace RandomSelector.Models.ViewModel.Home
{
	public class RecommendViewModel
	{
		public IEnumerable<SelectListItem> RecommendDropDownItems {
			get
			{
				SelectListGroup group = null;
				foreach (var item in RecommendData.AllRecommendSetData.OrderByDescending(x => x.BaseExpansionID).ThenBy(x => x.RecommendSetID))
				{
					if ((group?.Name ?? "") != Const.ExpansionData[item.BaseExpansionID])
					{
						group = new SelectListGroup() { Name = Const.ExpansionData[item.BaseExpansionID] };
					}
					string text = item.RecommendSetName + (item.BaseExpansionID == item.SubExpansionID ? "" : "（＋" + Const.ExpansionData[item.SubExpansionID] + "）");
					
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