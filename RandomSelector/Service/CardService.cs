using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Models.Entity;
using RandomSelector.Models.Param;

namespace RandomSelector.Service
{
    /// <summary>
    /// カード情報取得系のBusinessLogicクラス
    /// </summary>
    public class CardService
    {
        /// <summary>ランダムクラス</summary>
        private Random _rnd { get; } = new Random();

        /// <summary>
        /// プロモの王国カードデータ取得
        /// </summary>
        /// <returns>カードデータ一覧</returns>
        public IEnumerable<CardEntity> GetPromSupplyCardData()
        {
            return CardData.SupplyCardData.Where(x => x.ExpansionID == ExpansionID.Promotion);
        }

        /// <summary>
        /// 選択されなかったプロモ王国カードのID一覧取得
        /// </summary>
        /// <param name="selectedCardIDList">選択したカードID一覧</param>
        /// <returns></returns>
        public IEnumerable<int> GetNotSelectedPromSupplyCardIDList(IEnumerable<int> selectedCardIDList)
        {
            return GetPromSupplyCardData().Where(x => !selectedCardIDList.Contains(x.CardID)).Select(x => x.CardID);
        }

        /// <summary>
        /// ランダムでカードを1枚取得します。
        /// </summary>
        /// <param name="choiceCardList">取得元のカード一覧</param>
        /// <param name="condition">取得条件</param>
        /// <param name="predicate">追加条件</param>
        /// <returns>カード情報</returns>
        /// <remarks>取得できなかった場合はnullを返します</remarks>
        public CardEntity GetRandomCard(
            IEnumerable<CardEntity> choiceCardList,
            CardChoiceCondition condition,
            Func<CardEntity, bool> predicate = null)
        {
            var condedList = choiceCardList;
            if (condition != null)
            {
                condedList = condedList.Where(x =>
                condition.ExpansionIDList.Contains(x.ExpansionID) &&
                !condition.ExpansionIDList.Contains(x.RerecordExpansionID) &&
                !condition.IgnoreCardIDList.Contains(x.CardID));
            }
            if (predicate != null)
            {
                condedList = condedList.Where(predicate);
            }

            var randomIdx = _rnd.Next(0, condedList.Count());
            return condedList.Where((_, i) => i == randomIdx).SingleOrDefault()?.Clone();
        }

        /// <summary>
        /// 災いカードの絞り込み条件を取得します。
        /// </summary>
        /// <returns></returns>
        public Func<CardEntity, bool> GetDisasterCondition()
        {
            // 1. 王国カード
            // 2. 2～3金コスト
            // 3. コストは財宝のみ
            return x => x.Class == CardClass.Kingdom && new[] { 2, 3 }.Contains(x.TreasureCost) && x.IsTreasureCostOnly;
        }
    }
}