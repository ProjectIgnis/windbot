using System;
using System.Collections.Generic;
using System.Linq;

namespace RyzealAI.Prototype
{
    // ============================================================
    // 0. 이 파일의 목적 / 이번 확장에서 바뀐 것
    // ============================================================
    // - 카드 20개로 늘어나면서, 조건 없는/단순한 카드까지 전부 함수로
    //   따로 짜는 건 유지보수가 안 되므로 "데이터 선언 + 해석기" 구조로 전환.
    // - Star / Plugin / Cross 는 여전히 다른 카드의 파워 총합을 참조해야
    //   해서(Tier1/Tier2) 예전처럼 전용 함수로 남겨둠.
    // - 나머지(Ice/Sword/Ext/Node/PlasmaHole + 핸드트랩/튜터/기타 전부)는
    //   SimpleCardValue(BasePower + Modifier[]) 선언만으로 처리.
    //
    // *** 버그 수정 ***
    //   기존 코드는 "몬스터인가"(CardKindTable.IsMonster)로
    //   "Ryzeal 몬스터 존재/총합"을 판정했음. 이제 비Ryzeal 몬스터(핸드트랩 등)가
    //   추가되면서 이 가정이 깨짐 -> RyzealMonsterTag로 별도 분리.
    //
    // *** 데이터 미확정 항목 (TODO, 실제 값으로 교체 필요) ***
    //   1) CardId.MulcharmyPurulia / Sakitama / TrapHole / SkullMeister / LightningCrash
    //      실제 카드ID를 몰라서 임의의 음수 placeholder를 넣어뒀음.
    //   2) CardLevelTable: Sakitama의 "다른 level4가 없을 경우" 조건 판정용.
    //      Ryzeal 몬스터들의 실제 레벨을 몰라서 비어있음 -> 지금은 항상
    //      "다른 level4 없음"으로 평가됨. 실제 레벨 채워야 정확해짐.
    //   3) GameState.XyzExistsAnywhere: 엑시즈 소재 판정 로직 미구현이라
    //      여전히 외부에서 값을 세팅해줘야 함(Node 카드용).
    // ============================================================


    #region 카드 ID / 태그 정의

    public static class CardId
    {
        // --- Ryzeal 코어 ---
        public const int StarRyzeal = 84433129;
        public const int IceRyzeal = 8633261;
        public const int SwordRyzeal = 35844557;
        public const int NodeRyzeal = 72238166;
        public const int ExtRyzeal = 34022970;
        public const int RyzealPlugin = 60394026;
        public const int RyzealCross = 6798031;
        public const int RyzealPlasmaHole = 33787730;

        // --- 핸드트랩 ---
        public const int AshBlossomAndJoyousSpring = 14558127;
        public const int MaxxC = 23434538;
        public const int MulcharmyFuwalos = 42141493;
        public const int MulcharmyPurulia = 84192580;
        public const int NibiruThePrimalBeing = 27204311;
        public const int GhostBelle = 73642296; // Ghost Belle & Haunted Mansion
        public const int InfiniteImpermanence = 10045474;
        public const int GhostOgreAndSnowRabbit = 59438930; // AshBlossom과 동일값 처리
        public const int SkullMeister = 67750322;

        // --- 튜터 / 기타 ---
        public const int SeventhTachyon = 7477101;
        public const int Bonfire = 85106525;
        public const int LightningCrash = 89753095;
        public const int TripleTacticsTalent = 25311006;
        public const int CrossoutDesignator = 65681983;
        public const int GravediggersTrapHole = 31548215;
        public const int PotOfProsperity = 84211599;        // etc, 보정없음 80
        public const int Sakitama = 67972302;

        // --- 엑스트라 덱 (전부 Xyz) ---
        public const int SuperStarslayerTYPHON = 93039339;
        public const int MereologicAggregator = 9940036;
        public const int RyzealDetonator = 34909328;
        public const int Number104Masquerade = 2061963;
        public const int ExosisterKarmael = 77675029;
        public const int RyzealDuoDrive = 7511613;
        public const int DarkRebellionXyzDragon = 90091224;
        public const int EclipseTwins = 45852939;
        public const int FullArmoredUtopicRayLancer = 1269512;
        public const int TornadoDragon = 6983839;
        public const int EvilswarmExcitonKnight = 46772449;
        public const int TraptrixRafflesia = 6511113;
    }

    public enum CardKind { Monster, Spell }

    public static class CardKindTable
    {
        public static readonly Dictionary<int, CardKind> Kind = new Dictionary<int, CardKind>
        {
            // Ryzeal 코어
            { CardId.StarRyzeal, CardKind.Monster },
            { CardId.IceRyzeal, CardKind.Monster },
            { CardId.SwordRyzeal, CardKind.Monster },
            { CardId.NodeRyzeal, CardKind.Monster },
            { CardId.ExtRyzeal, CardKind.Monster },
            { CardId.RyzealPlugin, CardKind.Spell },
            { CardId.RyzealCross, CardKind.Spell },
            { CardId.RyzealPlasmaHole, CardKind.Spell },

            // 핸드트랩
            { CardId.AshBlossomAndJoyousSpring, CardKind.Monster },
            { CardId.MaxxC, CardKind.Monster },
            { CardId.MulcharmyFuwalos, CardKind.Monster },
            { CardId.MulcharmyPurulia, CardKind.Monster },
            { CardId.NibiruThePrimalBeing, CardKind.Monster },
            { CardId.GhostBelle, CardKind.Monster },
            { CardId.GhostOgreAndSnowRabbit, CardKind.Monster },
            { CardId.SkullMeister, CardKind.Monster },
            { CardId.InfiniteImpermanence, CardKind.Spell }, // 함정

            // 튜터/기타
            { CardId.SeventhTachyon, CardKind.Spell },
            { CardId.PotOfProsperity, CardKind.Spell },
            { CardId.Bonfire, CardKind.Spell },
            { CardId.LightningCrash, CardKind.Spell },
            { CardId.Sakitama, CardKind.Monster },
            { CardId.GravediggersTrapHole, CardKind.Spell },
            { CardId.TripleTacticsTalent, CardKind.Spell },
            { CardId.CrossoutDesignator, CardKind.Spell },

            // 엑스트라 덱 (전부 Xyz 몬스터)
            { CardId.SuperStarslayerTYPHON, CardKind.Monster },
            { CardId.MereologicAggregator, CardKind.Monster },
            { CardId.RyzealDetonator, CardKind.Monster },
            { CardId.Number104Masquerade, CardKind.Monster },
            { CardId.ExosisterKarmael, CardKind.Monster },
            { CardId.RyzealDuoDrive, CardKind.Monster },
            { CardId.DarkRebellionXyzDragon, CardKind.Monster },
            { CardId.EclipseTwins, CardKind.Monster },
            { CardId.FullArmoredUtopicRayLancer, CardKind.Monster },
            { CardId.TornadoDragon, CardKind.Monster },
            { CardId.EvilswarmExcitonKnight, CardKind.Monster },
            { CardId.TraptrixRafflesia, CardKind.Monster },
        };

        public static bool IsMonster(int cardId)
        {
            CardKind k;
            return Kind.TryGetValue(cardId, out k) && k == CardKind.Monster;
        }
    }

    // "Ryzeal 몬스터 파워 총합" 계산에 실제로 들어가는 카드만 담는 태그.
    // (버그수정 지점: CardKindTable.IsMonster와 절대 혼용하지 말 것)
    public static class RyzealMonsterTag
    {
        public static readonly HashSet<int> Set = new HashSet<int>
        {
            CardId.StarRyzeal, CardId.IceRyzeal, CardId.SwordRyzeal,
            CardId.NodeRyzeal, CardId.ExtRyzeal,
        };

        public static bool IsRyzealMonster(int cardId) => Set.Contains(cardId);
    }

    // "hand에 Ryzeal spell/trap 카드가 있을 경우" (Seventh Tachyon 조건용)
    public static class RyzealSpellTrapTag
    {
        public static readonly HashSet<int> Set = new HashSet<int>
        {
            CardId.RyzealPlugin, CardId.RyzealCross, CardId.RyzealPlasmaHole,
        };

        public static bool IsRyzealSpellTrap(int cardId) => Set.Contains(cardId);
    }

    // 엑스트라덱 Xyz 카드 전체 (Node의 "XYZ 몬스터 존재" 판정 자동화용)
    public static class ExtraDeckXyzTag
    {
        public static readonly HashSet<int> Set = new HashSet<int>
        {
            CardId.SuperStarslayerTYPHON, CardId.MereologicAggregator, CardId.RyzealDetonator,
            CardId.Number104Masquerade, CardId.ExosisterKarmael, CardId.RyzealDuoDrive,
            CardId.DarkRebellionXyzDragon, CardId.EclipseTwins, CardId.FullArmoredUtopicRayLancer,
            CardId.TornadoDragon, CardId.EvilswarmExcitonKnight, CardId.TraptrixRafflesia,
        };

        public static bool IsXyz(int cardId) => Set.Contains(cardId);
    }

    // Sakitama "다른 level4" 조건 등에 사용. 실제 레벨 데이터 반영됨.
    public static class CardLevelTable
    {
        public static readonly Dictionary<int, int> Level = new Dictionary<int, int>
        {
            { CardId.NibiruThePrimalBeing, 11 },
            { CardId.StarRyzeal, 4 },
            { CardId.IceRyzeal, 4 },
            { CardId.SwordRyzeal, 4 },
            { CardId.NodeRyzeal, 4 },
            { CardId.ExtRyzeal, 4 },
            { CardId.MulcharmyFuwalos, 4 },
            { CardId.MulcharmyPurulia, 4 },
            { CardId.Sakitama, 4 },
            { CardId.SkullMeister, 4 },
            { CardId.AshBlossomAndJoyousSpring, 3 },
            { CardId.GhostOgreAndSnowRabbit, 3 },
            { CardId.GhostBelle, 3 },
            { CardId.MaxxC, 2 },
        };

        public static int GetLevel(int cardId)
        {
            int lv;
            return Level.TryGetValue(cardId, out lv) ? lv : -1; // -1 = 레벨 미확인 (몬스터가 아니거나 미등록)
        }
    }

    #endregion


    #region GameState

    public enum Zone { Hand, Field, Grave, Banished }

    public class GameState
    {
        public Dictionary<Zone, List<int>> Zones = new Dictionary<Zone, List<int>>
        {
            { Zone.Hand, new List<int>() },
            { Zone.Field, new List<int>() },
            { Zone.Grave, new List<int>() },
            { Zone.Banished, new List<int>() },
        };

        public bool NormalSummonUsed = false;
        public bool IsFirstPlayer = true;
        public int Turn = 1;

        public bool HasXyzMonsterInZones(params Zone[] zones)
        {
            return zones.SelectMany(z => Zones[z]).Any(ExtraDeckXyzTag.IsXyz);
        }

        public bool ExistsInZone(int cardId, params Zone[] zones)
        {
            return zones.Any(z => Zones[z].Contains(cardId));
        }

        public int CountInZones(int cardId, params Zone[] zones)
        {
            return zones.Sum(z => Zones[z].Count(id => id == cardId));
        }

        public IEnumerable<int> RyzealMonstersInZonesExcluding(int excludeCardId, params Zone[] zones)
        {
            return zones.SelectMany(z => Zones[z])
                        .Where(id => id != excludeCardId && RyzealMonsterTag.IsRyzealMonster(id));
        }

        // "hand에 중복 카드" = Junk Resource. 카드 하나라도 손에 2장 이상이면 그 카드는 junk.
        public bool IsJunkResourceInHand(int cardId)
        {
            return Zones[Zone.Hand].Count(id => id == cardId) >= 2;
        }

        public bool HasAnyJunkResourceInHand()
        {
            return Zones[Zone.Hand].GroupBy(id => id).Any(g => g.Count() >= 2);
        }

        public bool HasRyzealSpellTrapInHand()
        {
            return Zones[Zone.Hand].Any(RyzealSpellTrapTag.IsRyzealSpellTrap);
        }

        public bool HasOtherLevelInZone(int level, int excludeCardId, params Zone[] zones)
        {
            return zones.SelectMany(z => Zones[z])
                        .Any(id => id != excludeCardId && CardLevelTable.GetLevel(id) == level);
        }
    }

    #endregion


    #region 선언형 카드 가치 (Tier 0: 다른 카드 파워를 참조하지 않는 카드 전용)

    public enum ModifierType { Add, Multiply }

    public class ValueModifier
    {
        public Func<GameState, bool> Condition; // 순수 GameState 기반 판정만 허용 (다른 카드 파워 참조 금지)
        public ModifierType Type;
        public double Value;
        public string Description; // 디버깅/로그용

        public ValueModifier(Func<GameState, bool> condition, ModifierType type, double value, string description)
        {
            Condition = condition;
            Type = type;
            Value = value;
            Description = description;
        }
    }

    public class SimpleCardValue
    {
        public int CardId;
        public double BasePower;
        public List<ValueModifier> Modifiers = new List<ValueModifier>();
    }

    public static class SimpleCardEvaluator
    {
        // 계산 순서 고정: Base + 모든 Add 모디파이어 → 그 결과에 모든 Multiply 모디파이어 순차 적용
        public static double Evaluate(SimpleCardValue cv, GameState s)
        {
            double power = cv.BasePower;

            foreach (var m in cv.Modifiers.Where(m => m.Type == ModifierType.Add))
                if (m.Condition(s))
                    power += m.Value;

            foreach (var m in cv.Modifiers.Where(m => m.Type == ModifierType.Multiply))
                if (m.Condition(s))
                    power *= m.Value;

            return power;
        }
    }

    #endregion


    #region Tier 0 카드 데이터 선언

    public static class SimpleCardTable
    {
        public static readonly List<SimpleCardValue> All = new List<SimpleCardValue>
        {
            // ---------------- Ryzeal 코어 (Star/Plugin/Cross 제외) ----------------
            new SimpleCardValue
            {
                CardId = CardId.IceRyzeal, BasePower = 80,
                Modifiers = {
                    new ValueModifier(s => s.NormalSummonUsed, ModifierType.Add, -20, "일반소환권 사용됨"),
                }
            },
            new SimpleCardValue
            {
                CardId = CardId.SwordRyzeal, BasePower = 80,
                Modifiers = {
                    new ValueModifier(
                        s => !s.RyzealMonstersInZonesExcluding(CardId.SwordRyzeal, Zone.Field).Any()
                          && !s.RyzealMonstersInZonesExcluding(CardId.SwordRyzeal, Zone.Grave).Any(),
                        ModifierType.Add, -20, "필드+묘지에 Ryzeal 몬스터 없음"),
                }
            },
            new SimpleCardValue { CardId = CardId.ExtRyzeal, BasePower = 100 },
            new SimpleCardValue
            {
                CardId = CardId.NodeRyzeal, BasePower = 30,
                Modifiers = {
                    new ValueModifier(s => !s.HasXyzMonsterInZones(Zone.Field, Zone.Grave), ModifierType.Add, -20, "필드+묘지에 XYZ 없음"),
                    new ValueModifier(s => !s.HasAnyJunkResourceInHand(), ModifierType.Add, -10, "Junk Resource 없음"),
                }
            },
            new SimpleCardValue
            {
                CardId = CardId.RyzealPlasmaHole, BasePower = 20,
                Modifiers = {
                    new ValueModifier(s => s.IsFirstPlayer, ModifierType.Add, 10, "선공"),
                    new ValueModifier(s => !s.IsFirstPlayer, ModifierType.Add, -10, "후공"),
                }
            },

            // ---------------- 핸드트랩 ----------------
            new SimpleCardValue { CardId = CardId.AshBlossomAndJoyousSpring, BasePower = 80 },
            new SimpleCardValue { CardId = CardId.GhostOgreAndSnowRabbit, BasePower = 80 }, // Ash Blossom과 동일값/보정
            new SimpleCardValue { CardId = CardId.MaxxC, BasePower = 120 },
            new SimpleCardValue
            {
                CardId = CardId.MulcharmyFuwalos, BasePower = 100,
                Modifiers = {
                    new ValueModifier(s => s.Turn > 1, ModifierType.Add, -60, "1턴 이후"),
                }
            },
            new SimpleCardValue
            {
                CardId = CardId.MulcharmyPurulia, BasePower = 100,
                Modifiers = {
                    new ValueModifier(s => s.Turn > 1, ModifierType.Add, -60, "1턴 이후"),
                }
            },
            new SimpleCardValue { CardId = CardId.NibiruThePrimalBeing, BasePower = 60 },
            new SimpleCardValue { CardId = CardId.GhostBelle, BasePower = 70 },
            new SimpleCardValue { CardId = CardId.InfiniteImpermanence, BasePower = 60 },
            new SimpleCardValue { CardId = CardId.SkullMeister, BasePower = 60 },

            // ---------------- 튜터 ----------------
            new SimpleCardValue
            {
                CardId = CardId.SeventhTachyon, BasePower = 80,
                Modifiers = {
                    new ValueModifier(s => !s.HasAnyJunkResourceInHand(), ModifierType.Add, -30, "hand에 Junk Resource 없음"),
                    new ValueModifier(s => s.HasRyzealSpellTrapInHand(), ModifierType.Add, 20, "hand에 Ryzeal 스펠/트랩 있음"),
                }
            },
            new SimpleCardValue { CardId = CardId.Bonfire, BasePower = 70 },
            new SimpleCardValue { CardId = CardId.LightningCrash, BasePower = 70 },

            // ---------------- 기타 ----------------
            new SimpleCardValue
            {
                CardId = CardId.Sakitama, BasePower = 60,
                Modifiers = {
                    new ValueModifier(
                        s => !s.HasOtherLevelInZone(4, CardId.Sakitama, Zone.Hand),
                        ModifierType.Add, -40, "패에 다른 level4 없음"),
                }
            },
            new SimpleCardValue
            {
                // "자동 junk resource 보정" = 별도 처리 아님, 전 카드 공통 Junk Resource 로직이
                // 이 카드에도 동일하게 적용된다는 의미로 해석 (EvaluateAll의 전역 후처리 단계에서 자동 반영)
                CardId = CardId.GravediggersTrapHole, BasePower = 10,
                Modifiers = {
                    new ValueModifier(s => !s.IsFirstPlayer, ModifierType.Add, -10, "후공"),
                }
            },
            new SimpleCardValue { CardId = CardId.TripleTacticsTalent, BasePower = 40 },
            new SimpleCardValue { CardId = CardId.CrossoutDesignator, BasePower = 40 },
            new SimpleCardValue { CardId = CardId.PotOfProsperity, BasePower = 80 },
        };

        public static readonly Dictionary<int, SimpleCardValue> ById =
            All.ToDictionary(cv => cv.CardId, cv => cv);
    }

    #endregion


    #region Tier 1 / Tier 2 (다른 카드 파워를 참조하는 카드 - 전용 함수 유지)

    public static class TieredCardEvaluator
    {
        private static double GetOrZero(IDictionary<int, double> dict, int cardId)
        {
            double v;
            return dict.TryGetValue(cardId, out v) ? v : 0;
        }

        // Tier 1: Star - 다른 "몬스터"(Tier 0 결과)를 참조
        public static double EvaluateStar(GameState s, IDictionary<int, double> tier0Powers)
        {
            double power = 40;

            if (s.ExistsInZone(CardId.RyzealPlugin, Zone.Hand)) power -= 10;
            if (s.ExistsInZone(CardId.RyzealPlugin, Zone.Field)) power -= 10;
            if (s.ExistsInZone(CardId.RyzealPlugin, Zone.Grave)) power -= 10;
            if (s.ExistsInZone(CardId.RyzealPlugin, Zone.Banished)) power -= 10;

            if (s.ExistsInZone(CardId.RyzealCross, Zone.Hand)) power -= 20;
            if (s.ExistsInZone(CardId.RyzealCross, Zone.Field)) power -= 20;
            if (s.ExistsInZone(CardId.RyzealCross, Zone.Grave)) power -= 20;

            if (s.ExistsInZone(CardId.RyzealPlasmaHole, Zone.Hand)) power -= 10;
            if (s.ExistsInZone(CardId.RyzealPlasmaHole, Zone.Field)) power -= 10;
            if (s.ExistsInZone(CardId.RyzealPlasmaHole, Zone.Grave)) power -= 10;

            double otherMonsterPowerSum = s.RyzealMonstersInZonesExcluding(CardId.StarRyzeal, Zone.Hand, Zone.Field)
                .Sum(id => GetOrZero(tier0Powers, id));

            if (otherMonsterPowerSum <= 50)
                power -= 30;

            return power;
        }

        // Tier 2: Plugin/Cross - 몬스터 전체(Tier 0 + Tier 1)를 참조
        public static double EvaluatePlugin(GameState s, IDictionary<int, double> allMonsterPowers)
        {
            double power = 50;

            double otherRyzealMonsterSum = s.RyzealMonstersInZonesExcluding(CardId.RyzealPlugin, Zone.Hand, Zone.Field)
                .Sum(id => GetOrZero(allMonsterPowers, id));

            if (otherRyzealMonsterSum <= 60)
                power -= 20;

            return power;
        }

        public static double EvaluateCross(GameState s, IDictionary<int, double> allMonsterPowers)
        {
            double power = 60;

            bool hasStrongInHand = s.Zones[Zone.Hand]
                .Where(id => id != CardId.RyzealCross && RyzealMonsterTag.IsRyzealMonster(id))
                .Any(id => GetOrZero(allMonsterPowers, id) >= 80);

            bool hasStrongInField = s.Zones[Zone.Field]
                .Where(RyzealMonsterTag.IsRyzealMonster)
                .Any(id => GetOrZero(allMonsterPowers, id) >= 80);

            if (!hasStrongInHand && !hasStrongInField)
                power -= 20;

            int graveRyzealCount = s.Zones[Zone.Grave].Count(RyzealMonsterTag.IsRyzealMonster);
            if (graveRyzealCount >= 2)
                power += 10;

            return power;
        }
    }

    #endregion


    #region 전체 오케스트레이션

    public static class CardValueEngine
    {
        public static Dictionary<int, double> EvaluateAll(GameState s)
        {
            // Tier 0: 선언형 테이블 전부 일괄 계산
            var tier0 = new Dictionary<int, double>();
            foreach (var cv in SimpleCardTable.All)
                tier0[cv.CardId] = SimpleCardEvaluator.Evaluate(cv, s);

            // Tier 1: Star (Tier 0 결과 참조)
            var tier1 = new Dictionary<int, double>(tier0);
            tier1[CardId.StarRyzeal] = TieredCardEvaluator.EvaluateStar(s, tier0);

            // Tier 2: Plugin/Cross (Tier 0+1 결과 참조)
            var result = new Dictionary<int, double>(tier1);
            result[CardId.RyzealPlugin] = TieredCardEvaluator.EvaluatePlugin(s, tier1);
            result[CardId.RyzealCross] = TieredCardEvaluator.EvaluateCross(s, tier1);

            // 전역 후처리: Junk Resource(손패 중복) -> 모든 카드에 균일하게 ×0.75
            // (Tier 0/1/2 내부 계산에는 영향 없도록 반드시 마지막에 적용)
            var finalKeys = result.Keys.ToList();
            foreach (var cardId in finalKeys)
                if (s.IsJunkResourceInHand(cardId))
                    result[cardId] *= 0.75;

            return result;
        }
    }

    #endregion


    #region 유닛 테스트

    public static class CardValueEngineTests
    {
        private static int _pass = 0;
        private static int _fail = 0;

        private static void AssertEqual(double expected, double actual, string testName)
        {
            if (Math.Abs(expected - actual) < 0.0001)
            {
                _pass++;
                Console.WriteLine("[PASS] " + testName + " (expected=" + expected + ", actual=" + actual + ")");
            }
            else
            {
                _fail++;
                Console.WriteLine("[FAIL] " + testName + " (expected=" + expected + ", actual=" + actual + ")");
            }
        }

        // ---- 회귀 테스트 (지난 4장 프로토타입에서 그대로 유지돼야 함) ----

        public static void Test_Regression_Cross_StrongHandAndGraveResource()
        {
            var s = new GameState();
            s.Zones[Zone.Hand].AddRange(new[] { CardId.IceRyzeal, CardId.ExtRyzeal });
            s.Zones[Zone.Grave].AddRange(new[] { CardId.SwordRyzeal, CardId.NodeRyzeal, CardId.ExtRyzeal });

            var powers = CardValueEngine.EvaluateAll(s);
            AssertEqual(70, powers[CardId.RyzealCross], "회귀_Cross_강한패+묘지자원");
        }

        public static void Test_Regression_Star_WithNodeOnlyDropsTo10()
        {
            var s = new GameState();
            s.Zones[Zone.Hand].Add(CardId.StarRyzeal);
            s.Zones[Zone.Field].Add(CardId.NodeRyzeal);

            var powers = CardValueEngine.EvaluateAll(s);
            AssertEqual(10, powers[CardId.StarRyzeal], "회귀_Star_Node뿐일때");
        }

        // ---- 버그 수정 검증: 비Ryzeal 몬스터가 Ryzeal 총합에 안 끼는지 ----

        public static void Test_BugFix_NonRyzealMonsterNotCountedInStarSum()
        {
            var s = new GameState();
            s.Zones[Zone.Hand].Add(CardId.StarRyzeal);
            s.Zones[Zone.Hand].Add(CardId.AshBlossomAndJoyousSpring); // 몬스터지만 Ryzeal 아님

            var powers = CardValueEngine.EvaluateAll(s);
            // Ash Blossom(80)이 총합에 안 잡히므로 "Ryzeal 파워 총합 0 <= 50" -> -30 그대로 적용돼야 함
            AssertEqual(10, powers[CardId.StarRyzeal], "버그수정_비Ryzeal몬스터_Star합산제외");
        }

        public static void Test_BugFix_NonRyzealMonsterInGraveNotBlockingSwordCondition()
        {
            var s = new GameState();
            // 묘지에 Ash Blossom만 있고 실제 Ryzeal 몬스터는 필드/묘지 어디에도 없음
            s.Zones[Zone.Grave].Add(CardId.AshBlossomAndJoyousSpring);

            var powers = CardValueEngine.EvaluateAll(s);
            // "필드+묘지에 Ryzeal 몬스터 없음"이 참이어야 하므로 -20 적용 -> 80-20=60
            AssertEqual(60, powers[CardId.SwordRyzeal], "버그수정_비Ryzeal몬스터_Sword조건오염안됨");
        }

        // ---- 신규 카드 카테고리 검증 ----

        public static void Test_SimpleHandtrap_NoModifier()
        {
            var s = new GameState();
            var powers = CardValueEngine.EvaluateAll(s);
            AssertEqual(80, powers[CardId.AshBlossomAndJoyousSpring], "핸드트랩_AshBlossom_기본값");
            AssertEqual(80, powers[CardId.GhostOgreAndSnowRabbit], "핸드트랩_GhostOgre_AshBlossom과동일값");
            AssertEqual(120, powers[CardId.MaxxC], "핸드트랩_MaxxC_기본값");
            AssertEqual(80, powers[CardId.PotOfProsperity], "기타_PotOfProsperity_기본값");
        }

        public static void Test_Mulcharmy_TurnCondition()
        {
            var sTurn1 = new GameState { Turn = 1 };
            var sTurn2 = new GameState { Turn = 2 };

            var p1 = CardValueEngine.EvaluateAll(sTurn1);
            var p2 = CardValueEngine.EvaluateAll(sTurn2);

            AssertEqual(100, p1[CardId.MulcharmyFuwalos], "Mulcharmy_1턴차_보정없음");
            AssertEqual(40, p2[CardId.MulcharmyFuwalos], "Mulcharmy_2턴차_100-60");
        }

        public static void Test_SeventhTachyon_CombinedModifiers()
        {
            var s = new GameState();
            s.Zones[Zone.Hand].Add(CardId.RyzealCross); // Ryzeal 스펠/트랩 보유 -> +20
            // Junk Resource는 없음 (중복카드 없음) -> "없을 경우" 조건 True -> -30
            var powers = CardValueEngine.EvaluateAll(s);
            // 80 - 30(junk없음) + 20(ryzeal s/t 있음) = 70
            AssertEqual(70, powers[CardId.SeventhTachyon], "SeventhTachyon_복합조건_70");
        }

        public static void Test_JunkResourceMultiplier_AppliedGlobally()
        {
            var s = new GameState();
            s.Zones[Zone.Hand].AddRange(new[] { CardId.Bonfire, CardId.Bonfire }); // 중복 -> Junk Resource
            var powers = CardValueEngine.EvaluateAll(s);
            AssertEqual(70 * 0.75, powers[CardId.Bonfire], "JunkResource_중복카드_0.75배");
        }

        public static void Test_TrapHole_SecondPlayerAndJunk()
        {
            var s = new GameState { IsFirstPlayer = false };
            s.Zones[Zone.Hand].AddRange(new[] { CardId.GravediggersTrapHole, CardId.GravediggersTrapHole });
            var powers = CardValueEngine.EvaluateAll(s);
            // (10 - 10후공) * 0.75중복 = 0
            AssertEqual(0, powers[CardId.GravediggersTrapHole], "TrapHole_후공+중복_복합");
        }

        public static void Test_Node_AutoXyzDetectionFromExtraDeckList()
        {
            var sNoXyz = new GameState();
            var sWithXyz = new GameState();
            sWithXyz.Zones[Zone.Grave].Add(CardId.TraptrixRafflesia); // 엑스트라덱 목록에 있는 실제 Xyz 카드

            var pNoXyz = CardValueEngine.EvaluateAll(sNoXyz);
            var pWithXyz = CardValueEngine.EvaluateAll(sWithXyz);

            AssertEqual(0, pNoXyz[CardId.NodeRyzeal], "Node_XYZ없음_Junk없음_30-20-10");
            AssertEqual(20, pWithXyz[CardId.NodeRyzeal], "Node_묘지Xyz자동감지_30-10");
        }

        public static void Test_Sakitama_LevelConditionNowWorks()
        {
            var sAlone = new GameState();
            sAlone.Zones[Zone.Hand].Add(CardId.Sakitama);

            var sWithOtherLevel4 = new GameState();
            sWithOtherLevel4.Zones[Zone.Hand].AddRange(new[] { CardId.Sakitama, CardId.IceRyzeal });

            var pAlone = CardValueEngine.EvaluateAll(sAlone);
            var pWith = CardValueEngine.EvaluateAll(sWithOtherLevel4);

            AssertEqual(20, pAlone[CardId.Sakitama], "Sakitama_혼자있음_60-40");
            AssertEqual(60, pWith[CardId.Sakitama], "Sakitama_다른lv4있음_보정없음");
        }

        public static void RunAll()
        {
            _pass = 0; _fail = 0;

            Test_Regression_Cross_StrongHandAndGraveResource();
            Test_Regression_Star_WithNodeOnlyDropsTo10();
            Test_BugFix_NonRyzealMonsterNotCountedInStarSum();
            Test_BugFix_NonRyzealMonsterInGraveNotBlockingSwordCondition();
            Test_SimpleHandtrap_NoModifier();
            Test_Mulcharmy_TurnCondition();
            Test_SeventhTachyon_CombinedModifiers();
            Test_JunkResourceMultiplier_AppliedGlobally();
            Test_TrapHole_SecondPlayerAndJunk();
            Test_Node_AutoXyzDetectionFromExtraDeckList();
            Test_Sakitama_LevelConditionNowWorks();

            Console.WriteLine("");
            Console.WriteLine("결과: " + _pass + " 통과 / " + _fail + " 실패");
        }
    }

    #endregion



}
