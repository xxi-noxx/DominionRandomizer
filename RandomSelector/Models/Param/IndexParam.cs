using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Service;

namespace RandomSelector.Models.Param
{
    /// <summary>
    /// TOPページ パラメータ
    /// </summary>
    public class IndexParam : IValidatableObject
    {
        /// <summary>拡張一覧</summary>
        public IEnumerable<ExpansionID> ExpansionIDList { get; set; } = Enumerable.Empty<ExpansionID>();
        /// <summary>選択されたプロモカード一覧</summary>
        public IEnumerable<int> SelectedPromCardID { get; set; } = Enumerable.Empty<int>();
        // TODO : 画面から取得
        /// <summary>カード選択数</summary>
        public int SelectCardCount { get; set; } = 10;
        /// <summary>錬金術の重み付けを行うか</summary>
        public bool IsWeightingAlchemy { get; set; }

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
        }
    }

    /// <summary>
    /// カードの抽出条件
    /// </summary>
    public class CardChoiceCondition
    {
        /// <summary>
        /// カードの抽出条件を入力パラメータから作ります
        /// </summary>
        /// <param name="param">入力パラメータ</param>
        /// <param name="cardService">カード情報処理BusinessLogicクラスインスタンス</param>
        public CardChoiceCondition(IndexParam param, CardService cardService)
        {
            ExpansionIDList = param.ExpansionIDList;
            IgnoreCardIDList = cardService.GetNotSelectedPromSupplyCardIDList(param.SelectedPromCardID).ToList();
            SelectCardCount = param.SelectCardCount;
            IsWeightingAlchemy = param.IsWeightingAlchemy;
        }

        /// <summary>拡張一覧</summary>
        public IEnumerable<ExpansionID> ExpansionIDList { get; set; }
        /// <summary>除外するカード一覧</summary>
        public List<int> IgnoreCardIDList { get; set; }
        /// <summary>選択するカードの枚数</summary>
        public int SelectCardCount { get; set; }
        /// <summary>錬金術の重み付けを行うか</summary>
        public bool IsWeightingAlchemy { get; set; }
    }
}