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
    }
}