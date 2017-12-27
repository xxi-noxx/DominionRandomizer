using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RandomSelector.Common;
using RandomSelector.Service;

namespace RandomSelector.Models.Param
{
    /// <summary>
    /// TOPページ パラメータ
    /// </summary>
    public class IndexParam : IValidatableObject
    {
        /// <summary>拡張一覧</summary>
        public IEnumerable<ExpansionID> ExpansionIDList { get; set; }
        /// <summary>選択されたプロモカード一覧</summary>
        public IEnumerable<int> SelectedPromCardID { get; set; } = Enumerable.Empty<int>();
        // TODO : 画面から取得
        /// <summary>カード選択数</summary>
        public int SelectCardCount { get; set; } = 10;

        /// <summary>優先する拡張</summary>
        [EnumDataType(typeof(ExpansionID))]
        public ExpansionID? PriorityExpansion { get; set; }

        /// <summary>錬金術の重み付けを行うか</summary>
        public bool IsWeightingAlchemy { get; set; }
        /// <summary>アクション増加を必ず含むか</summary>
        public bool IsMustPlusAction { get; set; }
        /// <summary>購入増加を必ず含むか</summary>
        public bool IsMustPlusBuy { get; set; }
        /// <summary>特殊勝利点を必ず含むか</summary>
        public bool IsMustSpecialVictory { get; set; }

        /// <summary>
        /// 検証処理
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!ExpansionIDList?.Any() ?? false)
            {
                yield return new ValidationResult("拡張が選択されていません。");
            }
            else if (!ExpansionIDList.Any(x => x != ExpansionID.Promotion))
            {
                yield return new ValidationResult("プロモのみの選択は出来ません。");
            }
            else if (PriorityExpansion.HasValue && !ExpansionIDList.Contains(PriorityExpansion.Value))
            {
                yield return new ValidationResult("優先拡張が使用されていません。");
            }
        }
    }

    /// <summary>
    /// カードの抽出条件
    /// </summary>
    public class CardChoiceCondition : IndexParam
    {
        /// <summary>
        /// カードの抽出条件を入力パラメータから作ります
        /// </summary>
        /// <param name="param">入力パラメータ</param>
        /// <param name="cardService">カード情報処理BusinessLogicクラスインスタンス</param>
        public CardChoiceCondition(IndexParam param, CardService cardService)
        {
            // TODO : AutoMapper
            ExpansionIDList = param.ExpansionIDList;
            IgnoreCardIDList = cardService.GetNotSelectedPromSupplyCardIDList(param.SelectedPromCardID).ToList();
            SelectCardCount = param.SelectCardCount;
            IsWeightingAlchemy = param.IsWeightingAlchemy;
            IsMustPlusAction = param.IsMustPlusAction;
            IsMustPlusBuy = param.IsMustPlusBuy;
            IsMustSpecialVictory = param.IsMustSpecialVictory;
            PriorityExpansion = param.PriorityExpansion;
        }

        /// <summary>除外するカード一覧</summary>
        public List<int> IgnoreCardIDList { get; set; }
    }
}