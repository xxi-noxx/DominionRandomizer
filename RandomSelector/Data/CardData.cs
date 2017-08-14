using CsvHelper;
using CsvHelper.Configuration;
using RandomSelector.Common;
using RandomSelector.Models.Entity;
using RandomSelector.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RandomSelector.Models.Param;

namespace RandomSelector.Data
{
    /// <summary>
    /// カードデータとの連結部分クラス
    /// </summary>
    public static class CardData
    {
        /// <summary>全カードデータ</summary>
        public static IEnumerable<CardEntity> AllCardData { get; }
        /// <summary>サプライとして選択されるカードデータ</summary>
        public static IEnumerable<CardEntity> SupplyCardData { get; }
        /// <summary>王国カードデータ</summary>
        public static IEnumerable<CardEntity> KingdomCardData { get; }

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
        private static IEnumerable<CardEntity> LoadCardData()
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
                return csv.GetRecords<CardEntity>().ToArray();
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
            if (!choiceCardList?.Any() ?? false)
            {
                return null;
            }

            var randomIdx = Rnd.Next(0, choiceCardList.Count());
            return choiceCardList.Where((_, i) => i == randomIdx).Single().Clone();
        }

        public static CardEntity GetRandomCard(
            IEnumerable<CardEntity> choiceCardList,
            CardChoiceCondition condition,
            Func<CardEntity, bool> predicate = null)
        {
            var condedList = choiceCardList.Where(x =>
                condition.ExpansionIDList.Contains(x.ExpansionID) &&
                !condition.ExpansionIDList.Contains(x.RerecordExpansionID) &&
                !condition.IgnoreCardIDList.Contains(x.CardID));
            if (predicate != null)
            {
                condedList = condedList.Where(predicate);
            }

            var randomIdx = Rnd.Next(0, choiceCardList.Count());
            return condedList.Where((_, i) => i == randomIdx).Single().Clone();
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
        /// 闇市場デッキ用カードを1枚選定する
        /// </summary>
        /// <param name="condition">絞り込み条件</param>
        /// <returns>闇市場デッキ用カードデータ</returns>
        public static CardEntity GetDarkMarketCard(CardChoiceCondition condition)
        {
            var condedCardData = SetCondition(AllCardData, condition).Where(x => x.IsDarkMarketCard);
            return GetRandomCard(condedCardData);
        }
    }
}