using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RandomSelector.Common
{
    /// <summary>
    /// カード種別
    /// </summary>
    public enum CardClass : byte
    {
        /// <summary>その他（ランダムで選出されないもの）</summary>
        Other = 0,
        /// <summary>王国カード</summary>
        Kingdom = 1,
        /// <summary>王国ではないカード（イベント・ランドマーク）</summary>
        NotKingdom = 2
    }

    /// <summary>
    /// カード種類
    /// </summary>
    [Flags]
    public enum CardType
    {
        /// <summary>財宝</summary>
        Treasure = 1,
        /// <summary>勝利点</summary>
        Victory = 1 << 1,
        /// <summary>呪い</summary>
        Curse = 1 << 2,
        /// <summary>アクション</summary>
        Action = 1 << 3,
        /// <summary>リアクション</summary>
        Reaction = 1 << 4,
        /// <summary>アタック</summary>
        Attack = 1 << 5,
        /// <summary>持続</summary>
        Duration = 1 << 6,
        /// <summary>褒賞</summary>
        Prize = 1 << 7,
        /// <summary>廃墟</summary>
        Ruins = 1 << 8,
        /// <summary>避難所</summary>
        Shelter = 1 << 9,
        /// <summary>略奪者</summary>
        Looter = 1 << 10,
        /// <summary>騎士</summary>
        Knight = 1 << 11,
        /// <summary>イベント</summary>
        Event = 1 << 12,
        /// <summary>リザーブ</summary>
        Reserve = 1 << 13,
        /// <summary>トラベラー</summary>
        Traveller = 1 << 14,
        /// <summary>ランドマーク</summary>
        LandMark = 1 << 15,
        /// <summary>分割</summary>
        SplitPile = 1 << 16,
        /// <summary>集合</summary>
        Gathering = 1 << 17,
        /// <summary>城</summary>
        Castle = 1 << 18,
    }

    /// <summary>
    /// 拡張ID
    /// </summary>
    public enum ExpansionID : byte
    {
        /// <summary>基本セット</summary>
        /// <remarks>非表示用データにつき、DisplayAttributeを持たない</remarks>
        BasicSet = 0,
        /// <summary>基本</summary>
        [Display(Name = "基本", Order = 1)]
        Basic = 1,
        /// <summary>陰謀</summary>
        [Display(Name = "陰謀", Order = 3)]
        Intrigue = 2,
        /// <summary>海辺</summary>
        [Display(Name = "海辺", Order = 5)]
        Seaside = 3,
        /// <summary>錬金術</summary>
        [Display(Name = "錬金術", Order = 6)]
        Alchemy = 4,
        /// <summary>繁栄</summary>
        [Display(Name = "繁栄", Order = 7)]
        Prosperity = 5,
        /// <summary>収穫祭</summary>
        [Display(Name = "収穫祭", Order = 8)]
        Cornucopia = 6,
        /// <summary>異郷</summary>
        [Display(Name = "異郷", Order = 9)]
        Hinterlands = 7,
        /// <summary>暗黒時代</summary>
        [Display(Name = "暗黒時代", Order = 10)]
        DarkAges = 8,
        /// <summary>ギルド</summary>
        [Display(Name = "ギルド", Order = 11)]
        Guilds = 9,
        /// <summary>冒険</summary>
        [Display(Name = "冒険", Order = 12)]
        Adventures = 10,
        /// <summary>帝国</summary>
        [Display(Name = "帝国", Order = 13)]
        Empires = 11,
        /// <summary>基本2nd</summary>
        [Display(Name = "基本(2版)", Order = 2)]
        Basic2nd = 12,
        /// <summary>陰謀2nd</summary>
        [Display(Name = "陰謀(2版)", Order = 4)]
        Intrigue2nd = 13,
        /// <summary>Nocturne</summary>
        [Display(Name = "Nocturne", Order = 14)]
        Nocturne = 14,
        /// <summary>プロモ</summary>
        [Display(Name = "プロモ", Order = 99)]
        Promotion = 99,
    }

    /// <summary>
    /// アイテムコード
    /// </summary>
    [Flags]
    public enum ItemCode
    {
        /// <summary>抑留トークン</summary>
        [Display(Name = "抑留トークン", Order = 11)]
        EmbargoToken = 1,
        /// <summary>コイントークン</summary>
        [Display(Name = "コイントークン", Order = 12)]
        CoinToken = 1 << 1,
        /// <summary>ポーション</summary>
        [Display(Name = "ポーション", Order = 1)]
        Potion = 1 << 2,
        /// <summary>勝利点トークン</summary>
        [Display(Name = "勝利点トークン", Order = 13)]
        VPToken = 1 << 3,
        /// <summary>廃墟</summary>
        [Display(Name = "廃墟", Order = 21)]
        Ruins = 1 << 4,
        /// <summary>略奪品</summary>
        [Display(Name = "略奪品", Order = 22)]
        Spoils = 1 << 5,
        /// <summary>冒険トークン</summary>
        [Display(Name = "冒険トークン", Order = 14)]
        AdventureToken = 1 << 6,
        /// <summary>負債トークン</summary>
        [Display(Name = "負債トークン", Order = 15)]
        DebitToken = 1 << 7,
        /// <summary>Boons</summary>
        [Display(Name = "Boons", Order = 23)]
        Boons = 1 << 8,
        /// <summary>Hexes</summary>
        [Display(Name = "Hexes", Order = 24)]
        Hexes = 1 << 9
    }

    /// <summary>
    /// Heirlooms一覧
    /// </summary>
    public enum Heirloom
    {
        NoUse = 0,
        [Display(Name = "HauntedMirror")]
        HauntedMirror = 1,
        [Display(Name = "MagicLamp")]
        MagicLamp = 2,
        [Display(Name = "Goat")]
        Goat = 3,
        [Display(Name = "Pasture")]
        Pasture = 4,
        [Display(Name = "Pouch")]
        Pouch = 5,
        [Display(Name = "CursedGold")]
        CursedGold = 6,
        [Display(Name = "LuckyCoin")]
        LuckyCoin = 7
    }

    /// <summary>
    /// 定数クラス
    /// </summary>
    public class Const
    {
        /// <summary>
        /// 使用する物データDictionary（Key：ItemCode／Value：名称）
        /// </summary>
        public static readonly Dictionary<ItemCode, string> ItemName = new Dictionary<ItemCode, string>()
        {
            { ItemCode.EmbargoToken, "抑留トークン" },
            { ItemCode.CoinToken, "コイントークン" },
            { ItemCode.Potion, "ポーション" },
            { ItemCode.VPToken, "勝利点トークン" },
            { ItemCode.Ruins, "廃墟" },
            { ItemCode.Spoils, "略奪品" },
            { ItemCode.AdventureToken, "冒険トークン" },
            { ItemCode.DebitToken, "負債トークン" },
        };

        /// <summary>
        /// 分割カードのランダマイザーと実際のカードとの紐づけ（Key：ランダマイザーカードID／Value：紐づく分割カードID）
        /// </summary>
        public static readonly Dictionary<int, IEnumerable<int>> SplitPileRelation = new Dictionary<int, IEnumerable<int>>()
        {
            { 193, new int[] { 194, 195, 196, 197, 198, 199, 200, 201, 202, 203} },
            { 305, new int[] { 306, 307, 308, 309, 310, 311, 312, 313} },
            { 314, new int[] { 315, 337 } },
            { 321, new int[] { 322, 336 } },
            { 328, new int[] { 329, 326 } },
            { 334, new int[] { 335, 320 } },
            { 339, new int[] { 340, 303 } },
            { 378, new int[] { 379, 380 } }
        };

        /// <summary>
        /// 闇市場のカードID
        /// </summary>
        /// <remarks>追加で15種類の闇市場デッキを作る</remarks>
        public const int DarkMarketCardID = 84;
        /// <summary>
        /// 魔女娘のカードID
        /// </summary>
        /// <remarks>追加で災いカードを1枚選択する</remarks>
        public const int YoungWitchCardID = 144;
        /// <summary>
        /// オベリスクのカードID
        /// </summary>
        /// <remarks>サプライのアクションをランダムで1枚選ぶ</remarks>
        public const int ObeliskCardID = 370;

        /// <summary>
        /// 備考－災い
        /// </summary>
        public const string RemarkDisaster = "[災]";
        /// <summary>
        /// 備考－イベント
        /// </summary>
        public const string RemarkEvent = "[EV]";
        /// <summary>
        /// 備考－ランドマーク
        /// </summary>
        public const string RemarkLandMark = "[LM]";
    }
}