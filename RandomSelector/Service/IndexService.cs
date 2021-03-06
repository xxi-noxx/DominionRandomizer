﻿using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Models.Entity;
using RandomSelector.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using RandomSelector.Models.Param;

namespace RandomSelector.Service
{
    /// <summary>
    /// ランダマインザTOP画面 Service層
    /// </summary>
    public class IndexService
    {
        /// <summary>カード情報取得クラス</summary>
        private CardService _cardService;

        /// <summary>
        /// ランダマイザTOP画面 コンストラクタ
        /// </summary>
        /// <param name="cardService">カード情報取得クラスのインスタンス</param>
        public IndexService(CardService cardService)
        {
            _cardService = cardService;
        }

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
                PromCardList = _cardService.GetPromSupplyCardData()
            };
            // 初期選択からは「基本」「陰謀」「プロモ」を外しておく
            var exclusionExpansion = new[] { ExpansionID.Basic, ExpansionID.Promotion, ExpansionID.Intrigue };
            model.Param.ExpansionIDList = EnumUtil.GetDisplayValues<ExpansionID>().Where(x => !exclusionExpansion.Contains(x));
            // 錬金術の重み付けは初期False
            model.Param.IsWeightingAlchemy = false;

            return model;
        }

        /// <summary>
        /// ViewModel作成（Post時）
        /// </summary>
        /// <param name="param">検索条件パラメータ</param>
        /// <returns></returns>
        public IndexViewModel CreateViewModel(IndexParam param)
        {
            //_cardService = new CardService();
            var model = new IndexViewModel()
            {
                IsPostRequest = true,
                Param = param,
                PromCardList = _cardService.GetPromSupplyCardData()
            };
            var condition = new CardChoiceCondition(param, _cardService);

            // 使用するカードの選択
            var useCard = ChoiceUseCardList(condition);
            model.UseKingdomCardList = useCard.Item1;
            model.UseNotKingdomCardList = useCard.Item2;

            // TODO : 災いと闇市の仕様
            // 災いカード設定
            if (model.UseKingdomCardList.Any(x => x.CardID == Const.YoungWitchCardID))
            {
                // サプライ一覧だとEV/LMが含まれてしまう為、王国カードから取得する。
                var disaster = GetDisaster(CardData.KingdomCardData, condition);
                if (disaster != null)
                {
                    model.DisasterCard = disaster;
                    condition.IgnoreCardIDList.Add(disaster.CardID);
                }
                else
                {
                    // 災いカードが残りの王国カードから取れなかった場合は、選択済のカード一覧から探して入れ替える
                    disaster = GetDisaster(model.UseKingdomCardList, null);
                    disaster.SelectedNumber = 0;
                    model.DisasterCard = disaster;
                    model.UseKingdomCardList.Remove(model.UseKingdomCardList.Single(x => x.CardID == disaster.CardID));

                    // 代わりに王国カードに加えるカードを取得
                    var addKingdomCard = _cardService.GetRandomCard(CardData.KingdomCardData, condition);
                    addKingdomCard.SelectedNumber = model.UseKingdomCardList.Count + 1;
                    model.UseKingdomCardList.Add(addKingdomCard);
                    condition.IgnoreCardIDList.Add(addKingdomCard.CardID);
                }
            }

            // 闇市場デッキ作成
            if (model.UseKingdomCardList.Any(x => x.CardID == Const.DarkMarketCardID) || (model.DisasterCard?.CardID ?? 0) == Const.DarkMarketCardID)
            {
                // 拡張情報をパラメータの値に戻す（錬金重み付け解除）
                condition.ExpansionIDList = param.ExpansionIDList;
                CreateDarkMarketDeck(model, condition);
            }

            // 使用するHeirloomsを集める
            model.UseHeirlooms = model.UseKingdomCardList.Where(x => x.HeirloomCard != Heirloom.NoUse).Select(x => x.HeirloomCard).ToArray();

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

            CardEntity selectedCard;
            // アクション権必須
            if (condition.IsMustPlusAction)
            {
                selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.IsPlusAction);
                condition.IgnoreCardIDList.Add(selectedCard.CardID);
            }
            // 購入権必須
            if (condition.IsMustPlusBuy)
            {
                selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.IsPlusBuy);
                condition.IgnoreCardIDList.Add(selectedCard.CardID);
            }
            // 特殊勝利点必須
            if (condition.IsMustSpecialVictory)
            {
                selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.IsSpecialVictory);
                condition.IgnoreCardIDList.Add(selectedCard.CardID);
            }

            // カード選択
            while (kingdomCardList.Count < condition.SelectCardCount)
            {
                if (condition.PriorityExpansion.HasValue)
                {
                    // 優先拡張が選択されている場合は、王国カードが半分選択されるまでその拡張からのみ選択する。
                    if (kingdomCardList.Count < (condition.SelectCardCount / 2))
                    {
                        selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.ExpansionID == condition.PriorityExpansion);
                    }
                    else
                    {
                        selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.ExpansionID != condition.PriorityExpansion);
                    }
                }
                else
                {
                    selectedCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition);
                }


                if (selectedCard == null)
                {
                    break;
                }

                if (selectedCard.Class == CardClass.NotKingdom)
                {
                    // 王国以外のカードは最大2枚
                    if (notKingdomCardList.Count >= 2)
                    {
                        continue;
                    }
                    selectedCard.SelectedNumber = notKingdomCardList.Count + 1;
                    notKingdomCardList.Add(selectedCard);
                }
                else
                {
                    selectedCard.SelectedNumber = kingdomCardList.Count + 1;
                    kingdomCardList.Add(selectedCard);
                }
                condition.IgnoreCardIDList.Add(selectedCard.CardID);

                // 半分選択した所で錬金術重み付け
                if (kingdomCardList.Count == (condition.SelectCardCount / 2) && condition.IsWeightingAlchemy)
                {
                    if (kingdomCardList.Where(x => x.ExpansionID == ExpansionID.Alchemy).Any())
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            var alchemyCard = _cardService.GetRandomCard(CardData.SupplyCardData, condition, x => x.ExpansionID == ExpansionID.Alchemy);
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
        /// <param name="choiceCardList">災いカードを選択する元のカード一覧</param>
        /// <param name="condition">検索条件</param>
        /// <returns>災いカード</returns>
        private CardEntity GetDisaster(IEnumerable<CardEntity> choiceCardList, CardChoiceCondition condition)
        {
            var result = _cardService.GetRandomCard(choiceCardList, condition, _cardService.GetDisasterCondition());
            if (result != null)
            {
                result.Remark = Const.RemarkDisaster;
            }
            return result;
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
            // TODO : 魔女娘を一時的に外す（仕様が未確定な為）
            condition.IgnoreCardIDList.Add(Const.YoungWitchCardID);

            // 最大25枚のカードを闇市場デッキとする
            while (model.DarkMarketCardList.Count < 25)
            {
                var card = _cardService.GetRandomCard(CardData.AllCardData, condition, x => x.IsDarkMarketCard);
                if (card == null)
                {
                    break;
                }
                card.SelectedNumber = model.DarkMarketCardList.Count + 1;
                model.DarkMarketCardList.Add(card);
                condition.IgnoreCardIDList.Add(card.CardID);
            }
        }
    }
}