using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using RandomSelector.Models.Entity;
using RandomSelector.Properties;

namespace RandomSelector.Data
{
    /// <summary>
    /// 公式推奨サプライ関連のデータ
    /// </summary>
    public static class RecommendData
    {
        /// <summary>推奨セットデータ</summary>
        public static IEnumerable<RecommendSetEntity> RecommendSetData { get; }
        /// <summary>推奨セット毎のカードデータ</summary>
        public static ILookup<int, CardEntity> RecommendCardLookup { get; }

        static RecommendData()
        {
            RecommendSetData = LoadRecommendSetData();
            RecommendCardLookup = LoadRecommendCardData();
        }

        /// <summary>
        /// 公式推奨セットの情報読み取り
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<RecommendSetEntity> LoadRecommendSetData()
        {
            // CsvHelperの設定
            var conf = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                Delimiter = "\t",
            };
            conf.RegisterClassMap<RecommendSetEntityMap>();

            // リソースファイルから取得し、List化して返す
            var assembly = Assembly.GetExecutingAssembly();
            using (var csv = new CsvReader(new StreamReader(new MemoryStream(Resources.RecommendSetData)), conf))
            {
                return csv.GetRecords<RecommendSetEntity>().ToList();
            }
        }

        /// <summary>
        /// 公式推奨セットで使用するのカード情報読み取り
        /// </summary>
        /// <returns></returns>
        private static ILookup<int, CardEntity> LoadRecommendCardData()
        {
            // CsvHelperの設定
            var conf = new CsvConfiguration()
            {
                HasHeaderRecord = true,
                Delimiter = "\t",
            };
            conf.RegisterClassMap<RecommendDetailEntityMap>();

            var assembly = Assembly.GetExecutingAssembly();
            using (var csv = new CsvReader(new StreamReader(new MemoryStream(Resources.RecommendSetDetailData)), conf))
            {
                var detailData = csv.GetRecords<RecommendDetailEntity>();
                return detailData.Join(
                    CardData.AllCardData,
                    rec => rec.CardID,
                    card => card.CardID,
                    (rec, card) =>
                    {
                        var recCard = card.Clone();
                        recCard.Remark = rec.Remark;
                        return new KeyValuePair<int, CardEntity>(rec.RecommendSetID, recCard);
                    }
                ).ToLookup(x => x.Key, x => x.Value);
            }
        }


    }
}