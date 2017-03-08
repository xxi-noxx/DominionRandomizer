using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Models.Entity;
using RandomSelector.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomSelector.Service
{
	/// <summary>
	/// ランダマインザTOP画面 Service層
	/// </summary>
	public class IndexService
	{
		/// <summary>
		/// ViewModel作成
		/// </summary>
		/// <returns></returns>
		public IndexViewModel CreateViewModel()
		{
			var model = new IndexViewModel()
			{
				IsPostRequest = false,
				Param = new IndexParam(),
				PromCardList = CardData.GetPromSupplyCardData()
			};
			model.Param.ExpansionIDList = Const.ExpansionData.Keys.Where(x => x != ExpansionID.Promotion && x != ExpansionID.Basic2nd && x != ExpansionID.Intrigue2nd);

			return model;
		}

		/// <summary>
		/// ViewModel作成（Post時）
		/// </summary>
		/// <param name="param">検索条件パラメータ</param>
		/// <returns></returns>
		public IndexViewModel CreateViewModel(IndexParam param)
		{
			var model = new IndexViewModel()
			{
				IsPostRequest = true,
				Param = param,
				PromCardList = CardData.GetPromSupplyCardData()
			};
			var condition = new CardChoiceCondition(param);

			// 使用するカードの選択
			var useCard = ChoiceUseCardList(condition);
			model.UseKingdomCardList = useCard.Item1;
			model.UseNotKingdomCardList = useCard.Item2;

			// 1枚目と2枚目の選択カードから植民地・白金貨、避難所を使用するか決定
			var firstCard = model.UseKingdomCardList.Single(x => x.SelectedNumber == 1);
			var secondCard = model.UseKingdomCardList.Single(x => x.SelectedNumber == 2);
			if (firstCard.ExpansionID == ExpansionID.Prosperity || (firstCard.ExpansionID == ExpansionID.DarkAges && secondCard.ExpansionID == ExpansionID.Prosperity))
			{
				model.IsUseColony = true;
			}
			if (firstCard.ExpansionID == ExpansionID.DarkAges || (firstCard.ExpansionID == ExpansionID.Prosperity && secondCard.ExpansionID == ExpansionID.DarkAges))
			{
				model.IsUseShelter = true;
			}

			// 災いカード設定
			if (model.UseKingdomCardList.Any(x => x.CardID == Const.YoungWitchCardID))
			{
				SetDisaster(model.UseKingdomCardList, condition);
			}
			// 闇市場デッキ作成
			if (model.UseKingdomCardList.Any(x => x.CardID == Const.DarkMarketCardID))
			{
				// 拡張情報をパラメータの値に戻す（錬金重み付け解除）
				condition.ExpansionIDList = param.ExpansionIDList;
				CreateDarkMarketDeck(model, condition);
			}

			return model;
		}

		/// <summary>
		/// 使用するカードを選択する
		/// </summary>
		/// <param name="condition">使用するカードの条件</param>
		/// <returns>使用するカード（Item1：王国カード／Item2：王国以外のカード）</returns>
		private Tuple<List<CardEntity>, List<CardEntity>> ChoiceUseCardList(CardChoiceCondition condition)
		{
			var kingdomCardList = new List<CardEntity>();
			var notKingdomCardList = new List<CardEntity>();

			// カード選択
			while (kingdomCardList.Count < condition.SelectCardCount)
			{
				var card = CardData.GetRandomSupplyCard(condition);
				if (card == null)
				{
					break;
				}
				if (card.Class == CardClass.NotKingdom)
				{
					if (notKingdomCardList.Count >= 2)
					{
						continue;
					}
					else
					{
						card.SelectedNumber = notKingdomCardList.Count + 1;
						notKingdomCardList.Add(card);
					}
				}
				else
				{
					card.SelectedNumber = kingdomCardList.Count + 1;
					kingdomCardList.Add(card);
				}
				condition.IgnoreCardIDList.Add(card.CardID);

				// 半分選択した所で錬金術重み付け
				if (kingdomCardList.Count == (condition.SelectCardCount / 2))
				{
					if (kingdomCardList.Where(x => x.ExpansionID == ExpansionID.Alchemy).Any())
					{
						for (int i = 0; i < 2; i++)
						{
							var alchemyCard = CardData.GetRandomSupplyCard(condition, ExpansionID.Alchemy);
							if (alchemyCard == null)
							{
								break;
							}
							alchemyCard.SelectedNumber = kingdomCardList.Count + 1;
							kingdomCardList.Add(alchemyCard);
							condition.IgnoreCardIDList.Add(alchemyCard.CardID);
						}
					}
					else
					{
						condition.ExpansionIDList = condition.ExpansionIDList.Where(x => x != ExpansionID.Alchemy);
					}
				}
			}

			return new Tuple<List<CardEntity>, List<CardEntity>>(kingdomCardList, notKingdomCardList);
		}

		/// <summary>
		/// 災いカードの設定
		/// </summary>
		/// <param name="selectedKingdomCard">選択されている王国カード</param>
		/// <param name="condition">検索条件</param>
		/// <returns>災いカードが設定されたか</returns>
		private bool SetDisaster(List<CardEntity> selectedKingdomCard, CardChoiceCondition condition)
		{
			var card = CardData.GetDisasterCard(condition);
			if (card == null)
			{
				// 残りに災いカード候補が存在しない場合は選択済から災いカードを選び、選ばれた災いカードの代わりになる王国カードを追加
				card = CardData.GetRandomKingdomCard(condition);
				var disasterCard = CardData.GetDisasterCard(selectedKingdomCard);
				if (card == null || disasterCard == null)
				{
					return false;
				}
				selectedKingdomCard.Single(x => x.CardID == disasterCard.CardID).Remark = Const.RemarkDisaster;
			}
			else
			{
				card.Remark = Const.RemarkDisaster;
			}
			card.SelectedNumber = selectedKingdomCard.Count + 1;
			selectedKingdomCard.Add(card);
			condition.IgnoreCardIDList.Add(card.CardID);

			return true;
		}

		/// <summary>
		/// 闇市場デッキの生成
		/// </summary>
		/// <param name="model">ViewModel</param>
		/// <param name="condition">選択条件</param>
		private void CreateDarkMarketDeck(IndexViewModel model, CardChoiceCondition condition)
		{
			// 除外カードに分割カードが含まれている場合、それらに紐づくカードも除外対象とする。
			var selectedSplitPilesCardIDList = Const.SplitPileRelation.Where(x => condition.IgnoreCardIDList.Contains(x.Key)).SelectMany(x => x.Value);
			if (selectedSplitPilesCardIDList != null && selectedSplitPilesCardIDList.Any())
			{
				condition.IgnoreCardIDList.AddRange(selectedSplitPilesCardIDList);
			}

			// 最大15枚のカードを闇市場デッキとする
			while (model.DarkMarketCardList.Count < 15)
			{
				var card = CardData.GetDarkMarketCard(condition);
				if (card == null)
				{
					break;
				}
				card.SelectedNumber = model.DarkMarketCardList.Count + 1;
				model.DarkMarketCardList.Add(card);
				condition.IgnoreCardIDList.Add(card.CardID);

				// 魔女娘が選ばれた場合の処理
				if (card.CardID == Const.YoungWitchCardID)
				{
					var hasDisaster = SetDisaster(model.UseKingdomCardList, condition);
					// 災いカードが選択出来なかった場合
					if (!hasDisaster)
					{
						// 闇市場デッキの中から１枚を災いカードとし、それを闇市場デッキから除く
						var disasterCard = CardData.GetDisasterCard(model.DarkMarketCardList);
						model.DarkMarketCardList.Remove(disasterCard);
						disasterCard.Remark = Const.RemarkDisaster;
						disasterCard.SelectedNumber = model.UseKingdomCardList.Count + 1;
						model.UseKingdomCardList.Add(disasterCard);
					}
				}
			}
		}
	}
}