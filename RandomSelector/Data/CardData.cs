 using CsvHelper;
using CsvHelper.Configuration;
using RandomSelector.Common;
using RandomSelector.Models.Entity;
using RandomSelector.Models.ViewModel;
using RandomSelector.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RandomSelector.Data
{
	/// <summary>
	/// カードデータとの連結部分クラス
	/// </summary>
	public static class CardData
	{
		/// <summary>全カードデータ</summary>
		private static IEnumerable<CardEntity> AllCardData { get; }
		/// <summary>サプライとして選択されるカードデータ</summary>
		private static IEnumerable<CardEntity> SupplyCardData { get; }
		/// <summary>王国カードデータ</summary>
		private static IEnumerable<CardEntity> KingdomCardData { get; }

		/// <summary>ランダムクラス</summary>
		public static Random Rnd { get; } = new Random();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		static CardData()
		{
			// データ設定
			AllCardData = LoadCardData();
			SupplyCardData = AllCardData.Where(x => x.Class != CardClass.Other).ToArray();
			KingdomCardData = AllCardData.Where(x => x.Class == CardClass.Kingdom).ToArray();
		}

		/// <summary>
		/// カードデータ読込み
		/// </summary>
		/// <returns>全カードのデータ</returns>
		private static List<CardEntity> LoadCardData()
		{
			// CsvHelperの設定
			var conf = new CsvConfiguration()
			{
				HasHeaderRecord = true,
				Delimiter = "\t",
			};
			conf.RegisterClassMap<CardEntityMap>();

			// リソースファイルから取得し、List化して返す
			var assembly = Assembly.GetExecutingAssembly();
			using (var csv = new CsvReader(new StreamReader(new MemoryStream(Resources.CardData)), conf))
			{
				return csv.GetRecords<CardEntity>().ToList();
			}
		}

		/// <summary>
		/// プロモの王国カードデータ取得
		/// </summary>
		/// <returns>カードデータ一覧</returns>
		public static IEnumerable<CardEntity> GetPromSupplyCardData()
		{
			return SupplyCardData.Where(x => x.ExpansionID == ExpansionID.Promotion);
		}

		/// <summary>
		/// 選択されなかったプロモ王国カードのID一覧取得
		/// </summary>
		/// <param name="selectedCardIDList">選択したカードID一覧</param>
		/// <returns></returns>
		public static IEnumerable<int> GetNotSelectedPromSupplyCardIDList(IEnumerable<int> selectedCardIDList)
		{
			return GetPromSupplyCardData().Where(x => !selectedCardIDList.Contains(x.CardID)).Select(x => x.CardID);
		}

		/// <summary>
		/// ランダムでカードを1枚取得する
		/// </summary>
		/// <param name="choiceCardList">選択候補のカードデータ</param>
		/// <returns>ランダムに選択されたカード情報</returns>
		public static CardEntity GetRandomCard(IEnumerable<CardEntity> choiceCardList)
		{
			// 選択候補が空の場合はnullを返す
			if (choiceCardList == null || !choiceCardList.Any())
			{
				return null;
			}
			return choiceCardList.ToArray()[Rnd.Next(0, choiceCardList.Count())].Clone();
		}

		/// <summary>
		/// 指定のデータに対して絞り込み条件を設定
		/// </summary>
		/// <param name="target">対象データ</param>
		/// <param name="condition">絞り込み条件</param>
		/// <returns>絞り込み条件が適用されたデータ</returns>
		private static IEnumerable<CardEntity> SetCondition(IEnumerable<CardEntity> target, CardChoiceCondition condition)
		{
			return target.Where(x =>
				condition.ExpansionIDList.Contains(x.ExpansionID) &&
				!condition.ExpansionIDList.Contains(x.RerecordExpansionID) &&
				!condition.IgnoreCardIDList.Contains(x.CardID)
			);
		}

		/// <summary>
		/// サプライ候補を指定した条件で絞り込んでから、ランダムにカードを1枚取得する
		/// </summary>
		/// <param name="condition">絞り込み条件</param>
		/// <returns>ランダムに選択されたカード情報</returns>
		public static CardEntity GetRandomSupplyCard(CardChoiceCondition condition)
		{
			var condedCardData = SetCondition(SupplyCardData, condition);
			return GetRandomCard(condedCardData);
		}

		/// <summary>
		/// サプライ候補を拡張で絞り込んでから、ランダムにカードを1枚取得する
		/// </summary>
		/// <param name="condition">絞り込み条件</param>
		/// <param name="targetExpansion">絞り込む拡張</param>
		/// <returns>ランダムに選択されたカード情報</returns>
		public static CardEntity GetRandomSupplyCard(CardChoiceCondition condition, ExpansionID targetExpansion)
		{
			var condedCardData = SetCondition(SupplyCardData, condition);
			return GetRandomCard(condedCardData.Where(x => x.ExpansionID == targetExpansion));
		}

		/// <summary>
		/// 王国カード候補を指定した条件で絞り込んでから、ランダムにカードを1枚取得する
		/// </summary>
		/// <param name="condition">絞り込み条件</param>
		/// <returns>ランダムに選択されたカード情報</returns>
		public static CardEntity GetRandomKingdomCard(CardChoiceCondition condition)
		{
			var condedCardData = SetCondition(KingdomCardData, condition);
			return GetRandomCard(condedCardData);
		}
		
		/// <summary>
		/// 災いカードを1枚取得する
		/// </summary>
		/// <param name="condition">絞り込み条件</param>
		/// <returns>災いカード</returns>
		/// <remarks>残り候補に災いカードになる対象がない場合はnullを返します。</remarks>
		public static CardEntity GetDisasterCard(CardChoiceCondition condition)
		{
			var condedCardData = SetCondition(KingdomCardData, condition);
			return GetRandomCard(
				condedCardData.Where(x =>
					new int[] { 2, 3 }.Contains(x.TreasureCost) &&
					x.PortionCost <= 0 &&
					x.DebitCost <= 0
				)
			);
		}

		/// <summary>
		/// 選択済のカードから災いカードを1枚選定する
		/// </summary>
		/// <param name="selectedCardList">選択されているカード</param>
		/// <returns>災いカードとして選ばれたカード情報</returns>
		public static CardEntity GetDisasterCard(IEnumerable<CardEntity> selectedCardList)
		{
			return GetRandomCard(
				selectedCardList.Where(x =>
					x.Class == CardClass.Kingdom &&
					new int[] { 2, 3 }.Contains(x.TreasureCost) &&
					x.PortionCost <= 0 &&
					x.DebitCost <= 0
				)
			);
		}

		/// <summary>
		/// 闇市場デッキ用カードを1枚選定する
		/// </summary>
		/// <param name="condition">絞り込み条件</param>
		/// <returns>闇市場デッキ用カードデータ</returns>
		public static CardEntity GetDarkMarketCard(CardChoiceCondition condition)
		{
			var condedCardData = SetCondition(AllCardData, condition);
			return GetRandomCard(condedCardData.Where(x => x.IsDarkMarketCard));
		}
	}
}