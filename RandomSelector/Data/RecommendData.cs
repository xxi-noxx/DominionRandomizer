using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using RandomSelector.Models.Entity;
using RandomSelector.Properties;

namespace RandomSelector.Data
{
	public static class RecommendData
	{
		public static IEnumerable<RecommendSetEntity> AllRecommendSetData { get; }
		public static ILookup<int, CardEntity> AllRecommendDetailLookup { get; }

		static RecommendData()
		{
			AllRecommendSetData = LoadRecommendData();
			AllRecommendDetailLookup = LoadRecommendDetailData();
		}

		private static IEnumerable<RecommendSetEntity> LoadRecommendData()
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

		private static ILookup<int, CardEntity> LoadRecommendDetailData()
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