using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Models.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RandomSelector.Models.ViewModel
{
	/// <summary>
	/// TOPページViewModel
	/// </summary>
	public class IndexViewModel
	{
		/// <summary>プロモカード一覧</summary>
		public IEnumerable<CardEntity> PromCardList { get; set; }

		/// <summary>Postリクエストか</summary>
		public bool IsPostRequest { get; set; }
		/// <summary>パラメータ</summary>
		public IndexParam Param { get; set; }

		/// <summary>植民地・白金貨を使用するか</summary>
		public bool IsUseColony { get; set; } = false;
		/// <summary>避難所を使用するか</summary>
		public bool IsUseShelter { get; set; } = false;
		/// <summary>ポーションを使用するか</summary>
		public bool IsUsePotion
		{
			get
			{
				return ((UseKingdomCardList?.Any(x => x.PortionCost > 0) ?? false) || (DarkMarketCardList?.Any(x => x.PortionCost > 0) ?? false));
			}
		}

		/// <summary>使用する王国カード一覧</summary>
		public List<CardEntity> UseKingdomCardList { get; set; } = new List<CardEntity>();
		/// <summary>使用する王国以外のカード一覧</summary>
		public List<CardEntity> UseNotKingdomCardList { get; set; } = new List<CardEntity>();
		/// <summary>使用する闇市場のカード一覧</summary>
		public List<CardEntity> DarkMarketCardList { get; set; } = new List<CardEntity>();

		/// <summary>
		/// 指定した物を使用するか判断
		/// </summary>
		/// <param name="item">アイテム</param>
		/// <returns>使用する場合True</returns>
		public bool IsUseItem(ItemCode item)
		{
			var useCard = UseKingdomCardList.Concat(UseNotKingdomCardList).Concat(DarkMarketCardList);
			return (useCard.Any(x => x.UseItem.HasFlag(item)));
		}

		/// <summary>
		/// オベリスク(LandMark)の対象サプライカード名を取得
		/// </summary>
		/// <returns>カード名</returns>
		public string GetObeliskTargetSuuplyCardName()
		{
			var supplyCard = new List<string>(UseKingdomCardList.Where(x => x.Type.HasFlag(CardType.Action)).Select(x => x.Name));
			//supplyCard.AddRange(new string[] { "銅貨", "銀貨", "金貨", "屋敷", "公領", "属州", "呪い" });
			//if (IsUseColony)
			//{
			//	supplyCard.AddRange(new string[] { "白金貨", "植民地" });
			//}
			//if (IsUseItem(ItemCode.Potion))
			//{
			//	supplyCard.Add("ポーション");
			//}
			if (IsUseItem(ItemCode.Ruins))
			{
				supplyCard.Add("廃墟");
			}
			//if (IsUseItem(ItemCode.Spoils))
			//{
			//	supplyCard.Add("略奪品");
			//}

			var randomValue = CardData.Rnd.Next(0, supplyCard.Count);
			return supplyCard[randomValue];
		}
    }

	/// <summary>
	/// TOPページ パラメータ
	/// </summary>
	public class IndexParam : IValidatableObject
	{
		/// <summary>拡張一覧</summary>
		public IEnumerable<ExpansionID> ExpansionIDList { get; set; } = Enumerable.Empty<ExpansionID>();
		/// <summary>選択されたプロモカード一覧</summary>
		public IEnumerable<int> SelectedPromCardID { get; set; } = Enumerable.Empty<int>();
		// TODO : 画面から取得
		/// <summary>カード選択数</summary>
		public int SelectCardCount { get; set; } = 10;

		/// <summary>
		/// 検証処理
		/// </summary>
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (ExpansionIDList == null || !ExpansionIDList.Any())
			{
				yield return new ValidationResult("拡張が選択されていません。");
			}
			else if (!ExpansionIDList.Any(x => x != ExpansionID.Promotion))
			{
				yield return new ValidationResult("プロモのみの選択は出来ません。");
			}
		}
	}

	/// <summary>
	/// カードの抽出条件
	/// </summary>
	public class CardChoiceCondition
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="param">入力パラメータ</param>
		public CardChoiceCondition(IndexParam param)
		{
			ExpansionIDList = param.ExpansionIDList;
			IgnoreCardIDList = CardData.GetNotSelectedPromSupplyCardIDList(param.SelectedPromCardID).ToList();
			SelectCardCount = param.SelectCardCount;
		}

		/// <summary>拡張一覧</summary>
		public IEnumerable<ExpansionID> ExpansionIDList { get; set; }
		/// <summary>除外するカード一覧</summary>
		public List<int> IgnoreCardIDList { get; set; }
		/// <summary>選択するカードの枚数</summary>
		public int SelectCardCount { get; set; }
	}
}