using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using RandomSelector.Common;
using RandomSelector.Data;

namespace RandomSelector.Models.ViewModel.Home
{
	public class RecommendViewModel
	{
		public SelectList RecommendDropDownItems {
			get
			{
				var result = new List<dynamic>();
				foreach (var item in RecommendData.AllRecommendSetData.OrderByDescending(x => x.BaseExpansionID).ThenBy(x => x.RecommendSetID))
				{
					string text = item.RecommendSetName + (item.BaseExpansionID == item.SubExpansionID ? "" : "（＋" + Const.ExpansionData[item.SubExpansionID] + "）");
					result.Add(new { Value = item.RecommendSetID, Text = text, Group = Const.ExpansionData[item.BaseExpansionID] });
				}

				return new SelectList(result, "Value", "Text", dataGroupField: "Group", selectedValue: null);
			}
		}
	}
}