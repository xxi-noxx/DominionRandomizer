using CsvHelper.Configuration;
using RandomSelector.Common;

namespace RandomSelector.Models.Entity
{
	/// <summary>
	/// カードデータEntity
	/// </summary>
	public class CardEntity
	{
		/// <summary>カードID</summary>
		public int CardID { get; set; }

		/// <summary>拡張ID</summary>
		public ExpansionID ExpansionID { get; set; }
		/// <summary>再録元拡張ID</summary>
		public ExpansionID RerecordExpansionID { get; set; }

		/// <summary>カード名称</summary>
		public string Name { get; set; }
		/// <summary>カード名称（カナ）</summary>
		public string NameKana { get; set; }
		/// <summary>カード名称（英語）</summary>
		public string NameEnglish { get; set; }

		/// <summary>財宝コスト</summary>
		public int TreasureCost { get; set; }
		/// <summary>ポーションコスト</summary>
		public int PortionCost { get; set; }
		/// <summary>借金コスト</summary>
		public int DebitCost { get; set; }
		/// <summary>コスト（表示用）</summary>
		public string DisplayCost { get; set; }
		/// <summary>並び順</summary>
		public int SortOrder
		{
			get { return (TreasureCost == 0 ? DebitCost : TreasureCost); }
		}
		
		/// <summary>闇市場デッキ候補か</summary>
		public bool IsDarkMarketCard { get; set; }
		/// <summary>使用する物</summary>
		public ItemCode UseItem { get; set; }
		/// <summary>種別（王国カードか 等）</summary>
		public CardClass Class { get; set; }
		/// <summary>種類（アクションか 等）</summary>
		public CardType Type { get; set; }

		/// <summary>選択された順番</summary>
		public int SelectedNumber { get; set; }
		/// <summary>備考（災いか、オベリスクで選択されたか 等）</summary>
		public string Remark { get; set; }

		/// <summary>
		/// インスタンスの複製
		/// </summary>
		/// <returns></returns>
		public CardEntity Clone()
		{
			return (CardEntity)MemberwiseClone();
		}
	}

	/// <summary>
	/// データファイルとEntityクラスのMapping
	/// </summary>
	public class CardEntityMap : CsvClassMap<CardEntity>
	{
		public CardEntityMap()
		{
			Map(x => x.CardID).Name("CardID");
			Map(x => x.ExpansionID).Name("ExpansionID");
			Map(x => x.RerecordExpansionID).Name("RerecordExpansionID");
			Map(x => x.Name).Name("Name");
			Map(x => x.NameKana).Name("NameKana");
			Map(x => x.NameEnglish).Name("NameEnglish");
			Map(x => x.TreasureCost).Name("TreasureCost");
			Map(x => x.PortionCost).Name("PortionCost");
			Map(x => x.DebitCost).Name("DebitCost");
			Map(x => x.DisplayCost).Name("DisplayCost");
			Map(x => x.Class).Name("Class");
			Map(x => x.Type).Name("Type");
			Map(x => x.IsDarkMarketCard).Name("IsDarkMarketCard");
			Map(x => x.UseItem).Name("UseItem");
		}
	}
}