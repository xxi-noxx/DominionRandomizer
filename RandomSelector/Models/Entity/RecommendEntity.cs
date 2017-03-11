using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper.Configuration;
using RandomSelector.Common;

namespace RandomSelector.Models.Entity
{
	public class RecommendSetEntity
	{
		public int RecommendSetID { get; set; }
		public ExpansionID BaseExpansionID { get; set; }
		public ExpansionID SubExpansionID { get; set; }
		public string RecommendSetName { get; set; }
	}

	/// <summary>
	/// データファイルとEntityクラスのMapping
	/// </summary>
	public class RecommendSetEntityMap : CsvClassMap<RecommendSetEntity>
	{
		public RecommendSetEntityMap()
		{
			Map(x => x.RecommendSetID).Name("RecommendSetID");
			Map(x => x.BaseExpansionID).Name("BaseExpansionID");
			Map(x => x.SubExpansionID).Name("SubExpansionID");
			Map(x => x.RecommendSetName).Name("RecommendSetName");
		}
	}

	public class RecommendDetailEntity
	{
		public int RecommendSetID { get; set; }
		public int CardID { get; set; }
		public string Remark { get; set; }
	}

	public class RecommendDetailEntityMap : CsvClassMap<RecommendDetailEntity>
	{
		public RecommendDetailEntityMap()
		{
			Map(x => x.RecommendSetID).Name("RecommendSetID");
			Map(x => x.CardID).Name("CardID");
			Map(x => x.Remark).Name("Remark");
		}
	}
}