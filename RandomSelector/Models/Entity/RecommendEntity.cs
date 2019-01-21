using System;
using System.Collections.Generic;
using CsvHelper.Configuration;
using RandomSelector.Common;

namespace RandomSelector.Models.Entity
{
    /// <summary>
    /// 公式推奨セットEntity
    /// </summary>
    public class RecommendSetEntity
    {
        /// <summary>公式推奨セットID</summary>
        public int RecommendSetID { get; set; }
        /// <summary>ベースの拡張ID</summary>
        public ExpansionID BaseExpansionID { get; set; }
        /// <summary>組み合わせる拡張のID（複数）</summary>
        public IEnumerable<ExpansionID> SubExpansionIDs { get; set; }
        /// <summary>公式推奨セット名称</summary>
        public string RecommendSetName { get; set; }
    }

    /// <summary>
    /// 公式推奨セットデータとEntityクラスのMapping
    /// </summary>
    public class RecommendSetEntityMap : CsvClassMap<RecommendSetEntity>
    {
        private IEnumerable<string> _subExpansionColumns = new[] { "SubExpansionID1", "SubExpansionID2" };

        public RecommendSetEntityMap()
        {
            Map(x => x.RecommendSetID).Name("RecommendSetID");
            Map(x => x.BaseExpansionID).Name("BaseExpansionID");
            Map(x => x.SubExpansionIDs).ConvertUsing<IEnumerable<ExpansionID>>(row =>
            {
                var result = new List<ExpansionID>();
                foreach (var col in _subExpansionColumns)
                {
                    ExpansionID subEx;
                    if (Enum.TryParse(row.GetField(col), out subEx))
                    {
                        result.Add(subEx);
                    }
                }
                return result;
            });
            Map(x => x.RecommendSetName).Name("RecommendSetName");
        }
    }

    /// <summary>
    /// 公式推奨セット 詳細Entity
    /// </summary>
    public class RecommendDetailEntity
    {
        /// <summary>公式推奨セットID</summary>
        public int RecommendSetID { get; set; }
        /// <summary>カードID</summary>
        public int CardID { get; set; }
        /// <summary>備考</summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 公式推奨セット詳細データとEntityのマッピング
    /// </summary>
    public class RecommendDetailEntityMap : CsvClassMap<RecommendDetailEntity>
    {
        public RecommendDetailEntityMap()
        {
            Map(x => x.RecommendSetID).Name("RecommendSetID");
            Map(x => x.CardID).Name("CardID");
            Map(x => x.Remark).Name("Remark");
        }
    }
}