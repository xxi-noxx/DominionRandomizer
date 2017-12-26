using System.Collections.Generic;
using System.Linq;
using RandomSelector.Common;
using RandomSelector.Data;
using RandomSelector.Models.Entity;
using RandomSelector.Models.Param;

namespace RandomSelector.Models.ViewModel
{
    /// <summary>
    /// TOPページViewModel
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>プロモカード一覧</summary>
        public IEnumerable<CardEntity> PromCardList { get; set; }

        /// <summary>Postリクエストか</summary>
        public bool IsPostRequest { get; set; }
        /// <summary>パラメータ</summary>
        public IndexParam Param { get; set; }

        /// <summary>使用する王国カード一覧</summary>
        public List<CardEntity> UseKingdomCardList { get; set; } = new List<CardEntity>();
        /// <summary>使用する王国以外のカード一覧</summary>
        public List<CardEntity> UseNotKingdomCardList { get; set; } = new List<CardEntity>();
        /// <summary>使用する闇市場のカード一覧</summary>
        public List<CardEntity> DarkMarketCardList { get; set; } = new List<CardEntity>();
        /// <summary>災いカード</summary>
        public CardEntity DisasterCard { get; set; }

        /// <summary>植民地・白金貨を使用するか</summary>
        public bool IsUseColony
        {
            get { return UseKingdomCardList.Any(x => new[] { 1, 2 }.Contains(x.SelectedNumber) && x.ExpansionID == ExpansionID.Prosperity); }
        }
        /// <summary>避難所を使用するか</summary>
        public bool IsUseShelter
        {
            get { return UseKingdomCardList.Any(x => new[] { 1, 2 }.Contains(x.SelectedNumber) && x.ExpansionID == ExpansionID.DarkAges); }
        }
        /// <summary>ポーションを使用するか</summary>
        public bool IsUsePotion
        {
            get { return ((UseKingdomCardList?.Any(x => x.PortionCost > 0) ?? false) || (DarkMarketCardList?.Any(x => x.PortionCost > 0) ?? false)); }
        }

        /// <summary>
        /// 指定した物を使用するか判断
        /// </summary>
        /// <param name="item">アイテム</param>
        /// <returns>使用する場合True</returns>
        public bool IsUseItem(ItemCode item)
        {
            var useCard = UseKingdomCardList.Concat(UseNotKingdomCardList).Concat(DarkMarketCardList);
            return (useCard.Any(x => x.UseItem.HasFlag(item)));
        }

        /// <summary>
        /// 仕様する王国カード一覧に災いカードを追加して返す
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CardEntity> GetUseKingdomListAddDisaster()
        {
            var result = new List<CardEntity>(UseKingdomCardList);
            if (DisasterCard != null)
            {
                result.Add(DisasterCard);
            }

            return result;
        }

        /// <summary>
        /// オベリスク(LandMark)の対象サプライカード名を取得
        /// </summary>
        /// <returns>カード名</returns>
        public string GetObeliskTargetSuuplyCardName()
        {
            var supplyCard = new List<string>(UseKingdomCardList.Where(x => x.Type.HasFlag(CardType.Action)).Select(x => x.Name));
            // 廃墟を使用する場合はそれも選択肢に加える
            if (IsUseItem(ItemCode.Ruins))
            {
                supplyCard.Add("廃墟");
            }

            var randomValue = CardData.Rnd.Next(0, supplyCard.Count);
            return supplyCard[randomValue];
        }
    }
}