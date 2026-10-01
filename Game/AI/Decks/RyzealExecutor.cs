using System;
using System.Linq;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using RyzealAI.Prototype;

namespace WindBot.Game.AI.Decks
{
    [Deck("Ryzeal", "AI_Ryzeal", "Easy")]
    class RyzealExecutor : DefaultExecutor, IMainPhaseDecisionObserver
    {

        #region Card IDs

        public class CardId
        {
            // 덱에 사용되는 카드 ID (중복 카드 하나만 선언)
            public const int NibiruThePrimalBeing = 27204311;
            public const int StarRyzeal = 84433129;
            public const int IceRyzeal = 8633261;
            public const int SwordRyzeal = 35844557;
            public const int NodeRyzeal = 72238166;
            public const int ExtRyzeal = 34022970;
            public const int MulcharmyFuwalos = 42141493;
            public const int MulcharmyPurulia = 84192580;
            public const int AshBlossomAndJoyousSpring = 14558127;
            public const int GhostOgreAndSnowRabbit = 59438930;
            public const int GhostBelleAndHauntedMansion = 73642296;
            public const int MaxxC = 23434538;
            public const int SeventhTachyon = 7477101;
            public const int SeventhAscension = 23153227;
            public const int TripleTacticsTalent = 25311006;
            public const int PotOfProsperity = 84211599;
            public const int Bonfire = 85106525;
            public const int CalledByTheGrave = 24224830;
            public const int RyzealPlugin = 60394026;
            public const int CrossoutDesignator = 65681983;
            public const int RyzealCross = 6798031;
            public const int InfiniteImpermanence = 10045474;
            public const int GravediggersTrapHole = 31548215;
            public const int RyzealPlasmaHole = 33787730;

            // Extra Deck
            public const int DivineArsenalAAZEUSSkyThunder = 90448279;
            public const int MereologicAggregator = 9940036;
            public const int RyzealDetonator = 34909328;
            public const int Number104Masquerade = 2061963;
            public const int RyzealDuoDrive = 7511613;
            public const int EclipseTwins = 45852939;
            public const int FullArmoredUtopicRayLancer = 1269512;
            public const int TornadoDragon = 6983839;
            public const int EvilswarmExcitonKnight = 46772449;
            public const int InfernalFlameBanshee = 29423048;
            public const int TraptrixRafflesia = 6511113;
            public const int NumberF0UtopicFutureZexal = 41522092;
        }

        // 상태 추적 변수들 (필요시 추가)
        private bool NormalSummonUsed = false;

        // HOPT(카드별 "이 효과는 턴에 1번" 제약) 추적.
        // *** 중요: 정정된 설계 ***
        // Ryzeal 몬스터들은 실제로 카드 하나당 "룰 특수소환 효과(체인 안 씀)"와
        // "발동형 효과(서치 등)"라는 서로 독립적인 턴당1번 제약을 두 개씩 갖고 있다.
        // 한 쪽 효과가 이번 턴 이미 쓰였어도 다른 쪽 효과는 여전히 쓸 수 있어야 하므로
        // (예: 2장째 Ice를 룰 특수소환해서 엑시즈 소재로 쓰는 플레이), 카드ID 하나에
        // 플래그 하나로 묶으면 안 되고 효과종류별로 별도 추적해야 함.
        private readonly HashSet<int> _activateEffectUsedThisTurn = new HashSet<int>();
        private readonly HashSet<int> _specialSummonEffectUsedThisTurn = new HashSet<int>();

        // Ice의 발동효과(c8633261.lua e2)는 "일반소환 성공시"에만 터지고 룰특수소환으론 안 터짐.
        // NormalSummonUsed(범용 일반소환권 사용여부)와 별개로, "Ice 본인이 이번 턴에 일반소환됐는가"를
        // 정확히 추적해야 해서 전용 플래그로 둠.
        private bool _iceNormalSummonedThisTurn = false;

        private bool IsActivateEffectUsedThisTurn(int cardId)
        {
            return _activateEffectUsedThisTurn.Contains(cardId);
        }

        private void MarkActivateEffectUsedThisTurn(int cardId)
        {
            _activateEffectUsedThisTurn.Add(cardId);
        }

        private bool IsSpecialSummonEffectUsedThisTurn(int cardId)
        {
            return _specialSummonEffectUsedThisTurn.Contains(cardId);
        }

        private void MarkSpecialSummonEffectUsedThisTurn(int cardId)
        {
            _specialSummonEffectUsedThisTurn.Add(cardId);
        }

        // 메인 Ryzeal 덱 카드 그룹 (기본 5장 + 메인덱 서포트)
        private readonly int[] Ryzeals =
        {
            CardId.StarRyzeal,
            CardId.IceRyzeal,
            CardId.SwordRyzeal,
            CardId.NodeRyzeal,
            CardId.ExtRyzeal,
            CardId.RyzealPlugin,
            CardId.RyzealCross,
            CardId.RyzealPlasmaHole
        };

        // Ryzeal 몬스터 전체 (기본 5장 + 추가 강화 카드)
        private readonly int[] RyzealMonsters =
        {
            CardId.StarRyzeal,
            CardId.IceRyzeal,
            CardId.SwordRyzeal,
            CardId.NodeRyzeal,
            CardId.ExtRyzeal,
            CardId.RyzealDetonator,
            CardId.RyzealDuoDrive
        };

        // 카드의 실제 속성/종족을 런타임에 Deck에서 조회하지 않고,
        // 현재 확정 덱에서 검색 가능한 범위를 정적으로 정의한다.
        // Sword Ryzeal: LIGHT + Pyro
        private readonly int[] PyroMonsters =
        {
            CardId.StarRyzeal,
            CardId.IceRyzeal
        };

        // Ext Ryzeal: FIRE + Thunder
        private readonly int[] ThunderMonsters =
        {
            CardId.SwordRyzeal,
            CardId.NodeRyzeal,
            CardId.ExtRyzeal
        };

        // 현재 덱에서 필드의 Rank 4 Xyz 소재로 활용할 수 있는 Level 4 몬스터.
        // 특히 Ice 룰 특수소환의 코스트를 고를 때 필드의 Level 4를 최대한 보존한다.
        private readonly int[] Level4Monsters =
        {
            CardId.StarRyzeal,
            CardId.IceRyzeal,
            CardId.SwordRyzeal,
            CardId.NodeRyzeal,
            CardId.ExtRyzeal,
            CardId.MulcharmyFuwalos,
            CardId.MulcharmyPurulia
        };

        public RyzealExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // 1) 상대 효과 대응 (핸드트랩/무효화)
            AddExecutor(ExecutorType.Activate, CardId.NibiruThePrimalBeing, NibiruEffect);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomAndJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostOgreAndSnowRabbit, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.GhostBelleAndHauntedMansion, DefaultGhostBelleAndHauntedMansion);
            AddExecutor(ExecutorType.Activate, CardId.MaxxC, DefaultMaxxC);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyFuwalos, DefaultMulcharmyFuwalos);
            AddExecutor(ExecutorType.Activate, CardId.MulcharmyPurulia, DefaultMulcharmyPurulia);
            AddExecutor(ExecutorType.Activate, CardId.CalledByTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.CrossoutDesignator, CrossOutNegate);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.GravediggersTrapHole, DefaultUniqueTrap);

            // Detonator optional on-summon attachment trigger. Other effects remain unsupported.
            AddExecutor(ExecutorType.Activate, CardId.RyzealDetonator, DetonatorAttachEffect);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDetonator, DetonatorDestroyEffect);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDuoDrive, DuoDriveAttachEffect);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDuoDrive, DuoDriveSearchEffect);

            // 2) 서치 / 초기 세팅
            AddExecutor(ExecutorType.Activate, CardId.SeventhAscension, SeventhAscensionEffect);
            AddExecutor(ExecutorType.Activate, CardId.SeventhTachyon, SeventhTachyonEffect);
            AddExecutor(ExecutorType.Activate, CardId.Bonfire, BonfireEffect);

            // 2.5) 기본 Ryzeal 순차 전개
            AddExecutor(ExecutorType.Summon, CardId.IceRyzeal, IceRyzealSummon);
            AddExecutor(ExecutorType.Activate, CardId.IceRyzeal, IceRyzealEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.IceRyzeal, IceRyzealSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.SwordRyzeal, SwordRyzealSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.NodeRyzeal, NodeRyzealSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.NodeRyzeal, NodeRyzealEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.ExtRyzeal, ExtRyzealSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.ExtRyzeal, ExtRyzealEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.StarRyzeal, StarRyzealSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.SwordRyzeal, SwordRyzealEffect);

            // 3) 덱/엑스트라덱 활용
            AddExecutor(ExecutorType.SpSummon, CardId.TraptrixRafflesia, TraptrixRafflesiaSummon);

            // 기본 Spell/Trap/Repos 처리 (비-리질/비-아키타입 카드에 대해)
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            AddExecutor(ExecutorType.Activate, CardId.PotOfProsperity, DefaultPotOfDesires);
            AddExecutor(ExecutorType.Activate, CardId.TripleTacticsTalent, DefaultTrap);

            // TODO: 추가 콤보/소환 로직 구현
        }

        public override bool OnSelectHand()
        {
            return true;
        }

        // 우선순위: Seventh Ascension -> Seventh Tachyon -> Bonfire -> 기타는 Default_OnSelectCard
        //          (Then: Ext Ryzeal / Ice Ryzeal search priority)
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable)
        {
            // Detach cost and target occur before this activation is added to CurrentChain.
            // Route using the explicitly accepted activation, not the previous chain card.
            if (_pendingDetonatorDestroySource != null && cards.Count > 0)
            {
                if (cards.All(c => c.Location == CardLocation.Overlay))
                {
                    var chosen = cards.OrderBy(c => IsRyzealMonster(c.Id) ? 0 : 1)
                        .ThenBy(c => c.Id).Take(min).ToList();
                    if (chosen.Count == min)
                    {
                        foreach (var c in chosen)
                            _decisionLogger.WriteSelection("DetonatorDestroyDetach", Duel.Turn, cards.Select(x => x.Id), c.Id);
                        return chosen;
                    }
                }
                if (hint == (long)HintMsg.Destroy && min == 1 && max == 1)
                {
                    var chosen = cards.Where(c => c.Controller == 1)
                        .OrderByDescending(c => c.IsFaceup())
                        .ThenBy(c => c.Id).FirstOrDefault();
                    _pendingDetonatorDestroySource = null;
                    if (chosen != null)
                    {
                        _decisionLogger.WriteSelection("DetonatorDestroyTarget", Duel.Turn, cards.Select(x => x.Id), chosen.Id);
                        return new List<ClientCard> { chosen };
                    }
                    // The actual prompt may differ from the field snapshot; let host handle it.
                }
            }
            // Resolution can concern an earlier chain link, not the last activated card.
            ClientCard current = Duel.SolvingChainIndex > 0 && Duel.SolvingChainIndex <= Duel.CurrentChain.Count
                ? Duel.CurrentChain[Duel.SolvingChainIndex - 1] : Util.GetLastChainCard();
            // Handle engine-offered selections only; do not queue detach as an activation cost.
            if (current != null && current.Id == CardId.RyzealDuoDrive && cards.Count > 0)
            {
                if (cards.All(c => c.Location == CardLocation.Overlay))
                {
                    var chosen = cards.OrderBy(c => IsRyzealMonster(c.Id) ? 0 : 1)
                        .ThenBy(c => c.Id).Take(min).ToList();
                    foreach (var c in chosen)
                        _decisionLogger.WriteSelection("DuoDriveDetach", Duel.Turn, cards.Select(x => x.Id), c.Id);
                    if (chosen.Count == min) return chosen;
                }
                if (cards.All(c => c.Location == CardLocation.Deck))
                {
                    // Verified upstream OnSelectUnselectCard exposes only selectable first-group
                    // cards, calls OnSelectCard(min=1,max=1), and min=0 when finishable.
                    // Select one valid card each time; return empty when engine allows finishing.
                    if (min == 0 && cancelable) return new List<ClientCard>();
                    var chosen = cards.GroupBy(c => c.Id).Select(g => g.First())
                        .OrderBy(c => Bot.Hand.Any(x => x.Id == c.Id) ? 1 : 0)
                        .ThenBy(c => DuoDriveSearchPriority(c.Id)).ThenBy(c => c.Id)
                        .Take(min).ToList();
                    if (chosen.Count == min)
                    {
                        foreach (var c in chosen)
                            _decisionLogger.WriteSelection("DuoDriveSearch", Duel.Turn, cards.Select(x => x.Id), c.Id);
                        return chosen;
                    }
                }
            }

            // Lua selects one material during resolution, not at activation. Use the engine's
            // offered candidates rather than attempting to duplicate attachfilter in C#.
            if (current != null && current.IsCode(CardId.RyzealDetonator, CardId.RyzealDuoDrive) &&
                hint == (long)HintMsg.XyzMaterial && min == 1 && max == 1 &&
                cards.All(c => c.Location == CardLocation.Grave))
            {
                var selected = cards.OrderBy(c => IsRyzealMonster(c.Id) ? 1 : 0)
                    .ThenBy(c => c.Id).FirstOrDefault();
                if (selected != null)
                {
                    _decisionLogger.WriteSelection(current.Id == CardId.RyzealDetonator ? "DetonatorAttach" : "DuoDriveAttach", Duel.Turn,
                        cards.Select(c => c.Id), selected.Id);
                    return new List<ClientCard> { selected };
                }
            }

            if (current == null)
                return base.OnSelectCard(cards, min, max, hint, cancelable);

            // Ryzeal 기반 카드/서치 이외에는 기본 선택 로직으로 처리
            if (!IsRyzealCard(current.Id) &&
                !current.IsCode(CardId.SeventhAscension, CardId.SeventhTachyon, CardId.Bonfire, CardId.MulcharmyFuwalos))
            {
                return base.OnSelectCard(cards, min, max, hint, cancelable);
            }

            if (current.IsCode(CardId.SeventhAscension))
            {
                ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.SeventhTachyon));
                if (selected != null)
                    return new List<ClientCard> { selected };
            }

            if (current.IsCode(CardId.SwordRyzeal) && cards.All(c => c.Location == CardLocation.Deck))
            {
                // Sword Ryzeal 효과로 덱에서 Ext -> Star -> Ice 우선 선택
                bool hasExt = IsCardAnywhere(CardId.ExtRyzeal);
                bool hasStar = IsCardAnywhere(CardId.StarRyzeal);
                bool hasIce = IsCardAnywhere(CardId.IceRyzeal);

                if (!hasExt)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.ExtRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (!hasStar)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.StarRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (!hasIce)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.IceRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                ClientCard fallback = cards.FirstOrDefault(c => c.IsCode(CardId.ExtRyzeal) || c.IsCode(CardId.IceRyzeal));
                if (fallback != null)
                    return new List<ClientCard> { fallback };
            }

            // Seventh Tachyon 발동 후 덱 맨 위로 넣을 카드 선택 (패에서 선택)
            if (current.IsCode(CardId.SeventhTachyon) && cards.All(c => c.Location == CardLocation.Hand))
            {
                // 1. MulcharmyFuwalos 우선
                ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.MulcharmyFuwalos));
                if (selected != null)
                    return new List<ClientCard> { selected };

                // 2. 중복 카드 (손패에서 같은 ID의 카드가 2장 이상인 것)
                var duplicateCards = cards.GroupBy(c => c.Id).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
                if (duplicateCards.Any())
                {
                    return new List<ClientCard> { duplicateCards.First() };
                }

                // 3. RyzealPlugin, RyzealCross, RyzealPlasmaHole 중 패에 있는 것 (그리고 필드/묘지/제외에 존재하는 경우)
                var ryzealCards = new[] { CardId.RyzealPlugin, CardId.RyzealCross, CardId.RyzealPlasmaHole };
                var availableRyzealCards = cards.Where(c => ryzealCards.Contains(c.Id)).ToList();
                if (availableRyzealCards.Any())
                {
                    // 모두 패, 필드, 묘지, 제외 중 하나에 있는지 확인 (즉, 게임에 존재하는지)
                    bool allExist = ryzealCards.All(id =>
                        Bot.Hand.Any(c => c.Id == id) ||
                        Bot.GetMonsters().Any(c => c.Id == id) ||
                        Bot.GetSpells().Any(c => c.Id == id) ||
                        Bot.Graveyard.Any(c => c.Id == id) ||
                        Bot.Banished.Any(c => c.Id == id));
                    if (allExist)
                    {
                        return new List<ClientCard> { availableRyzealCards.First() };
                    }
                }

                // 4. Ryzeal 카드가 아닌 카드 하나
                var nonRyzealCards = cards.Where(c => !IsRyzealCard(c.Id)).ToList();
                if (nonRyzealCards.Any())
                {
                    return new List<ClientCard> { nonRyzealCards.First() };
                }

                // 5. 랜덤
                return new List<ClientCard> { cards.First() };
            }

            if (current.IsCode(CardId.SeventhTachyon) || current.IsCode(CardId.Bonfire))
            {
                // Ext/Ice/Star 모두 1장 이상 있고 Ext 2장 이상이면 추가 서치 필요 없음
                int extCount = CountInHand(CardId.ExtRyzeal);
                int iceCount = CountInHand(CardId.IceRyzeal);
                int starCount = CountInHand(CardId.StarRyzeal);
                if (extCount >= 2 && iceCount >= 1 && starCount >= 1)
                    return base.OnSelectCard(cards, min, max, hint, cancelable);

                // 우선순위: Ext Ryzeal → Ice Ryzeal → Star Ryzeal → (2장째) Ext Ryzeal → (2장째) Star Ryzeal
                if (extCount == 0)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.ExtRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (iceCount == 0)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.IceRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (starCount == 0)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.StarRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (extCount == 1)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.ExtRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }

                if (starCount == 1)
                {
                    ClientCard selected = cards.FirstOrDefault(c => c.IsCode(CardId.StarRyzeal));
                    if (selected != null)
                        return new List<ClientCard> { selected };
                }
            }

            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        // "가지고 있는 카드"는 손패에 있는 카드만을 의미합니다.
        private bool HasCardInHand(int cardId)
        {
            return Bot.HasInHand(cardId);
        }

        private int CountInHand(int cardId)
        {
            return Bot.Hand.Count(card => card != null && card.Id == cardId);
        }

        private bool IsRyzealCard(int cardId)
        {
            if (RyzealMonsters.Contains(cardId))
                return true;

            // 확장/서포트 카드도 Ryzeal 범위로 간주
            return cardId == CardId.RyzealPlugin || cardId == CardId.RyzealCross || cardId == CardId.RyzealPlasmaHole;
        }

        private bool SeventhAscensionEffect()
        {
            // 우선적으로 꺼내야 하는 카드가 있는 경우 발동 (패에 7th Tachyon이 없을 때)
            return !HasCardInHand(CardId.SeventhTachyon);
        }

        private bool SeventhTachyonEffect()
        {
            // Ext/Ice/Star 모두 손패에 있고 Ext가 2장 이상이면 더 이상 사용할 필요 없음
            if (CountInHand(CardId.ExtRyzeal) >= 2 && HasCardInHand(CardId.IceRyzeal) && HasCardInHand(CardId.StarRyzeal))
                return false;

            // Ext/Ice Ryzeal이 손패에 부족할 때만 발동
            return CountInHand(CardId.ExtRyzeal) < 2 || !HasCardInHand(CardId.IceRyzeal) || !HasCardInHand(CardId.StarRyzeal);
        }

        private bool BonfireEffect()
        {
            // Ext/Ice/Star 모두 손패에 있고 Ext가 2장 이상이면 더 이상 사용할 필요 없음
            if (CountInHand(CardId.ExtRyzeal) >= 2 && HasCardInHand(CardId.IceRyzeal) && HasCardInHand(CardId.StarRyzeal))
                return false;

            // Ext/Ice/Star가 손패에 부족할 때 발동
            return CountInHand(CardId.ExtRyzeal) < 2 || !HasCardInHand(CardId.IceRyzeal) || !HasCardInHand(CardId.StarRyzeal);
        }

        private bool IsCardAnywhere(int cardId)
        {
            return Bot.HasInHandOrHasInMonstersZone(cardId)
                || Bot.HasInGraveyard(cardId)
                || Bot.HasInSpellZone(cardId)
                || Bot.HasInBanished(cardId);
        }

        private bool CanSummonIceRyzeal()
        {
            return !NormalSummonUsed && Bot.HasInHand(CardId.IceRyzeal);
        }

        private bool CommitSummonIceRyzeal()
        {
            NormalSummonUsed = true;
            _iceNormalSummonedThisTurn = true;
            return true;
        }

        private bool IceRyzealSummon()
        {
            if (!CanSummonIceRyzeal()) return false;
            return CommitSummonIceRyzeal();
        }

        // Ryzeal 몬스터들 중 "아직 어디에도 없는" 카드를 우선으로, 덱에 남아있는 걸 고르는 공용 로직.
        // (Ice의 덱 특수소환 효과, Node의 묘지 소생 효과 등 여러 곳에서 재사용)
        private int GetRyzealDeckTarget(int excludeCardId)
        {
            int[] candidates = { CardId.StarRyzeal, CardId.IceRyzeal, CardId.SwordRyzeal, CardId.NodeRyzeal, CardId.ExtRyzeal };
            foreach (var id in candidates)
            {
                if (id == excludeCardId) continue;
                if (!IsCardAnywhere(id) && Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }
            foreach (var id in candidates)
            {
                if (id == excludeCardId) continue;
                if (Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }
            return -1;
        }

        // *** c8633261.lua 확인 결과 전면 재작성 ***
        // 실제 효과: "일반소환 성공시"에만 발동(EVENT_SUMMON_SUCCESS - 특수소환으론 안 터짐),
        // 손으로 서치가 아니라 덱의 다른 Ryzeal 몬스터를 필드로 직접 특수소환(CATEGORY_SPECIAL_SUMMON).
        // 대상은 "Sword 고정"이 아니라 자신을 제외한 아무 Ryzeal 몬스터.
        private bool CanActivateIceRyzealEffect()
        {
            if (IsActivateEffectUsedThisTurn(CardId.IceRyzeal)) // HOPT: 발동효과 턴당 1번 (룰특수소환과 별개)
                return false;

            if (!_iceNormalSummonedThisTurn) // 일반소환 성공시에만 - 룰특수소환으론 안 터짐
                return false;

            if (Card == null || Card.Location != CardLocation.MonsterZone)
                return false;

            if (!Bot.HasInMonstersZone(CardId.IceRyzeal))
                return false;

            if (Bot.GetMonsterCount() >= 5) // 특수소환할 빈 몬스터존 필요 (메인 몬스터존 5칸 기준 근사치)
                return false;

            return GetRyzealDeckTarget(CardId.IceRyzeal) != -1;
        }

        private bool CommitActivateIceRyzealEffect()
        {
            int target = GetRyzealDeckTarget(CardId.IceRyzeal);
            if (target == -1) return false; // 안전장치

            MarkActivateEffectUsedThisTurn(CardId.IceRyzeal);
            AI.SelectCard(target); // 손 서치가 아니라 덱 -> 필드 직접 특수소환
            return true;
        }

        private bool IceRyzealEffect()
        {
            if (!CanActivateIceRyzealEffect()) return false;
            return CommitActivateIceRyzealEffect();
        }

        // ============================================================
        // Ryzeal 몬스터들의 "1번 효과" = 룰 특수소환(체인 안 씀), 카드마다 조건/코스트가 다름.
        // 발동형 효과(서치 등, 위쪽 IceRyzealEffect/SwordRyzealEffect)와는 별개의 HOPT.
        // ============================================================

        private bool IsRyzealMonster(int cardId)
        {
            return RyzealMonsters.Contains(cardId);
        }

        private bool HasLevel4OnField()
        {
            return Bot.GetMonsters().Any(c => Level4Monsters.Contains(c.Id));
        }

        // 필드 Level4 카드 수가 홀수일 때, Level4 몬스터를 필드에 하나 더 추가하는 행동은
        // Detonator/Duo Drive(레벨4 2장 필요) 같은 Rank4 엑시즈 소환을 "짝수"로 맞춰줘서
        // 추가 전개로 이어질 수 있게 한다.
        // 원래는 "이 행동 이후 실제로 Rank4 엑시즈를 하면 그 가치가 높다"를 미래 예측(lookahead)으로
        // 반영해야 하지만, 지금 단계에선 그 계산이 어려워서 "짝수를 만드는 행동" 자체에
        // 직접 가산점을 주는 방식으로 근사한다. (다섯 룰 특수소환 전부 "레벨4 자신을 필드에
        // 추가"하는 동일 매커니즘이라 Ice/Sword/Node/Ext/Star 전부에 동일 적용)
        private const double EvenLevel4BonusScore = 50; // TODO: 실전 테스트로 값 조정 필요

        private double GetEvenLevel4Bonus()
        {
            int level4Count = Bot.GetMonsters().Count(c => Level4Monsters.Contains(c.Id));
            return (level4Count % 2 == 1) ? EvenLevel4BonusScore : 0;
        }

        // --- Ice: 패/필드의 카드 1장을 묘지로 보내고 특수소환 ---
        private bool CanSpecialSummonIceRyzeal()
        {
            if (IsSpecialSummonEffectUsedThisTurn(CardId.IceRyzeal)) return false;
            if (!Bot.HasInHand(CardId.IceRyzeal)) return false;

            // 코스트: Ice 자신 말고 묘지로 보낼 카드가 손패 또는 필드에 있어야 함
            bool hasCostTarget = Bot.Hand.Count(c => c.Id != CardId.IceRyzeal) > 0
                || Bot.GetMonsters().Count > 0
                || Bot.GetSpells().Count > 0;
            return hasCostTarget;
        }

        // Ice 룰 특수소환의 코스트로 보낼 카드를 선택한다.
        // 기본 원칙:
        // 1) 손패의 Ice 이외 카드 우선
        // 2) 손패에 없으면 필드의 Level 4가 아닌 몬스터 우선
        // 3) 그래도 없으면 필드의 다른 카드
        // 4) 마지막 수단으로 필드의 Level 4를 허용
        //
        // 필드의 Level 4를 남겨두면 Mulcharmy Fuwalos/Purulia 같은 Level 4
        // 몬스터를 추가 소재로 활용해 Rank 4 Xyz로 이어갈 수 있으므로 보존한다.
        private int GetIceRyzealCostTarget()
        {
            // 1. 손패의 Ice 이외 카드: 필드 Level 4를 건드리지 않는다.
            ClientCard handTarget = Bot.Hand.FirstOrDefault(c => c.Id != CardId.IceRyzeal);
            if (handTarget != null)
                return handTarget.Id;

            // 2. 필드의 Level 4가 아닌 몬스터.
            ClientCard nonLevel4Monster = Bot.GetMonsters()
                .FirstOrDefault(c => !Level4Monsters.Contains(c.Id));
            if (nonLevel4Monster != null)
                return nonLevel4Monster.Id;

            // 3. 필드의 마법/함정.
            ClientCard spellTarget = Bot.GetSpells().FirstOrDefault();
            if (spellTarget != null)
                return spellTarget.Id;

            // 4. 다른 선택지가 없으면 Level 4 몬스터를 코스트로 사용.
            ClientCard level4Monster = Bot.GetMonsters()
                .FirstOrDefault(c => Level4Monsters.Contains(c.Id));
            if (level4Monster != null)
                return level4Monster.Id;

            return -1;
        }

        private bool CommitSpecialSummonIceRyzeal()
        {
            int target = GetIceRyzealCostTarget();
            if (target == -1)
                return false;

            MarkSpecialSummonEffectUsedThisTurn(CardId.IceRyzeal);
            AI.SelectCard(target);
            return true;
        }

        // --- Sword: 필드/묘지에 Ryzeal 몬스터가 존재하면 특수소환 (코스트 없음) ---
        private bool CanSpecialSummonSwordRyzeal()
        {
            if (IsSpecialSummonEffectUsedThisTurn(CardId.SwordRyzeal)) return false;
            if (!Bot.HasInHand(CardId.SwordRyzeal)) return false;

            bool hasRyzealMonster = Bot.GetMonsters().Any(c => IsRyzealMonster(c.Id))
                || Bot.Graveyard.Any(c => IsRyzealMonster(c.Id));
            return hasRyzealMonster;
        }

        private bool CommitSpecialSummonSwordRyzeal()
        {
            MarkSpecialSummonEffectUsedThisTurn(CardId.SwordRyzeal);
            return true; // 코스트 없음 - 추가 선택 불필요
        }

        // --- Node: 필드/묘지에 Xyz 몬스터가 존재하면 특수소환 (코스트 없음) ---
        private bool CanSpecialSummonNodeRyzeal()
        {
            if (IsSpecialSummonEffectUsedThisTurn(CardId.NodeRyzeal)) return false;
            if (!Bot.HasInHand(CardId.NodeRyzeal)) return false;

            bool hasXyz = Bot.GetMonsters().Any(c => c.HasType(CardType.Xyz))
                || Bot.Graveyard.Any(c => c.HasType(CardType.Xyz));
            return hasXyz;
        }

        private bool CommitSpecialSummonNodeRyzeal()
        {
            MarkSpecialSummonEffectUsedThisTurn(CardId.NodeRyzeal);
            return true;
        }

        // Node ignition effect (c72238166.lua e2): send 1 card from hand/field as cost,
        // then target and revive another Ryzeal monster from GY in face-up defense with effects negated.
        // Both selections are queued before the engine asks; no prompt is left to the default selector.
        private ClientCard GetNodeRyzealCostCard()
        {
            // On a full field prefer a monster cost; a hand/spell cost cannot free a zone.
            if (Bot.GetMonsterCount() >= 5)
                return Bot.GetMonsters().FirstOrDefault(c => c.IsFaceup());
            var duplicate = Bot.Hand.FirstOrDefault(c => c.Id != CardId.NodeRyzeal && Bot.Hand.Count(x => x.Id == c.Id) > 1);
            if (duplicate != null) return duplicate;
            var nonRyzeal = Bot.Hand.FirstOrDefault(c => !IsRyzealCard(c.Id));
            if (nonRyzeal != null) return nonRyzeal;
            var spell = Bot.GetSpells().FirstOrDefault();
            if (spell != null) return spell;
            // Remaining Ryzeal hand cards are legal costs as well.
            var hand = Bot.Hand.FirstOrDefault();
            if (hand != null) return hand;
            return Bot.GetMonsters().FirstOrDefault();
        }
        private int GetNodeRyzealCostTarget()
        {
            var card = GetNodeRyzealCostCard();
            return card == null ? -1 : card.Id;
        }

        private int GetNodeRyzealReviveTarget()
        {
            // The revived monster is negated, so choose it as a Rank 4 body rather than for its effect.
            int[] priority = { CardId.ExtRyzeal, CardId.SwordRyzeal, CardId.IceRyzeal, CardId.StarRyzeal };
            foreach (int id in priority)
                if (Bot.Graveyard.Any(c => c.Id == id)) return id;
            return -1;
        }

        private bool CanActivateNodeRyzealEffect()
        {
            if (IsActivateEffectUsedThisTurn(CardId.NodeRyzeal)) return false;
            if (Card == null || Card.Location != CardLocation.MonsterZone) return false;
            if (!Bot.HasInMonstersZone(CardId.NodeRyzeal)) return false;
            if (GetNodeRyzealCostTarget() == -1 || GetNodeRyzealReviveTarget() == -1) return false;

            // Lua checks Duel.GetMZoneCount(tp, costCard). A full field remains legal only when
            // the selected cost itself is a monster and therefore frees a monster zone.
            return Bot.GetMonsterCount() < 5 || Bot.GetMonsters().Count > 0;
        }

        private string GetNodeRyzealSelectionPlan()
        {
            return "cost:" + GetNodeRyzealCostTarget() + ";revive:" + GetNodeRyzealReviveTarget()
                + ";position:FaceUpDefence;revivedEffects:negated";
        }

        private bool CommitActivateNodeRyzealEffect()
        {
            int cost = GetNodeRyzealCostTarget();
            int revive = GetNodeRyzealReviveTarget();
            if (cost == -1 || revive == -1) return false;

            // GameAI queues selectors in prompt order: cost selection, then GY target selection.
            ClientCard costCard = GetNodeRyzealCostCard();
            ClientCard reviveCard = Bot.Graveyard.FirstOrDefault(c => c.Id == revive);
            if (costCard == null || reviveCard == null) return false;
            AI.SelectCard(costCard);
            AI.SelectNextCard(reviveCard);
            // Node's Lua fixes defense position; no position prompt is generated.
            // Do not queue SelectPosition here (it could leak into a later summon).
            MarkActivateEffectUsedThisTurn(CardId.NodeRyzeal);
            return true;
        }

        private bool NodeRyzealEffect()
        {
            if (!CanActivateNodeRyzealEffect()) return false;
            return CommitActivateNodeRyzealEffect();
        }

        // 순수 조건 판정: 지금 ExtRyzeal 효과로 서치할 대상이 있는지만 계산.
        // Ext의 실제 필터는 FIRE + Thunder (c34022970.lua thfilter). 현재 덱에서는
        // ThunderMonsters가 곧 Ext의 합법적인 검색 범위다.
        private int GetExtRyzealSearchTarget()
        {
            foreach (var id in ThunderMonsters)
            {
                if (!IsCardAnywhere(id) && Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }
            foreach (var id in ThunderMonsters)
            {
                if (Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }
            return -1;
        }

        // 추가 조건(c34022970.lua thtg): 필드에 "레벨4도 랭크4도 아닌" 앞면 몬스터가
        // 하나라도 있으면 이 효과 자체가 발동 불가. (thconfilter: IsFaceup and not(Level4 or Rank4))
        private bool HasFieldMonsterBlockingExtSearch()
        {
            return Bot.GetMonsters().Any(c => c.IsFaceup() && c.Level != 4 && c.Rank != 4);
        }

        private bool CanActivateExtRyzealEffect()
        {
            if (IsActivateEffectUsedThisTurn(CardId.ExtRyzeal)) // HOPT: 발동효과 턴당 1번 (룰특수소환과 별개)
                return false;

            if (Card == null || Card.Location != CardLocation.MonsterZone)
                return false;

            if (!Bot.HasInMonstersZone(CardId.ExtRyzeal))
                return false;

            if (HasFieldMonsterBlockingExtSearch())
                return false;

            return GetExtRyzealSearchTarget() != -1;
        }

        private bool CommitActivateExtRyzealEffect()
        {
            int target = GetExtRyzealSearchTarget();
            if (target == -1) return false; // 안전장치

            MarkActivateEffectUsedThisTurn(CardId.ExtRyzeal);
            AI.SelectCard(target);
            return true;
        }

        private bool ExtRyzealEffect()
        {
            if (!CanActivateExtRyzealEffect()) return false;
            return CommitActivateExtRyzealEffect();
        }

        // --- Ext: 엑스트라덱의 Xyz 몬스터 1장을 묘지로 보내고 특수소환 ---
        private bool CanSpecialSummonExtRyzeal()
        {
            if (IsSpecialSummonEffectUsedThisTurn(CardId.ExtRyzeal)) return false;
            if (!Bot.HasInHand(CardId.ExtRyzeal)) return false;

            return Bot.ExtraDeck.Any(c => c.HasType(CardType.Xyz));
        }

        private bool CommitSpecialSummonExtRyzeal()
        {
            MarkSpecialSummonEffectUsedThisTurn(CardId.ExtRyzeal);
            // TODO: 엑스트라덱의 여러 Xyz 중 뭘 묻을지 고르는 우선순위 로직 필요 (지금은 엔진 기본선택에 위임)
            AI.SelectCard(CardLocation.Extra);
            return true;
        }

        // --- Star: 필드 몬스터의 Xyz 소재 1개를 제거하고 특수소환 ---
        private bool CanSpecialSummonStarRyzeal()
        {
            if (IsSpecialSummonEffectUsedThisTurn(CardId.StarRyzeal)) return false;
            if (!Bot.HasInHand(CardId.StarRyzeal)) return false;

            return Bot.GetMonsters().Any(c => c.HasXyzMaterial());
        }

        private bool CommitSpecialSummonStarRyzeal()
        {
            MarkSpecialSummonEffectUsedThisTurn(CardId.StarRyzeal);
            // TODO: 어느 몬스터의 소재를 뗄지 선택하는 로직 미구현.
            // Xyz 소재 제거는 OnSelectXyzMaterial() 오버라이드가 필요할 수 있음 - 확인 필요.
            return true;
        }

        // AddExecutor가 호출하는 진입점들
        private bool IceRyzealSpSummon()
        {
            if (!CanSpecialSummonIceRyzeal()) return false;
            return CommitSpecialSummonIceRyzeal();
        }

        private bool SwordRyzealSpSummon()
        {
            if (!CanSpecialSummonSwordRyzeal()) return false;
            return CommitSpecialSummonSwordRyzeal();
        }

        private bool NodeRyzealSpSummon()
        {
            if (!CanSpecialSummonNodeRyzeal()) return false;
            return CommitSpecialSummonNodeRyzeal();
        }

        private bool ExtRyzealSpSummon()
        {
            if (!CanSpecialSummonExtRyzeal()) return false;
            return CommitSpecialSummonExtRyzeal();
        }

        private bool StarRyzealSpSummon()
        {
            if (!CanSpecialSummonStarRyzeal()) return false;
            return CommitSpecialSummonStarRyzeal();
        }

        // 순수 조건 판정: 지금 SwordRyzeal 효과로 서치할 대상이 있는지만 계산.
        // 어느 카드를 고를지의 "우선순위 로직"이지만 AI.SelectCard 호출(부작용)은 없음
        // -> Can*/Commit* 양쪽에서 이 함수 하나를 공유해서 로직 중복을 피함.
        private int GetSwordRyzealSearchTarget()
        {
            // Sword의 실제 필터는 LIGHT + Pyro.
            // 현재 덱에서는 PyroMonsters가 곧 Sword의 합법적인 검색 범위다.
            //
            // 기존의 Ext 우선 선택은 잘못된 구현이었다.
            // Ext는 FIRE + Thunder이므로 Sword의 LIGHT + Pyro 효과로 검색할 수 없다.

            foreach (var id in PyroMonsters)
            {
                if (!IsCardAnywhere(id) && Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }

            // 이미 다른 영역에 존재하더라도 덱에 남아 있으면 선택 가능.
            // 우선순위는 Star -> Ice로 유지한다.
            foreach (var id in PyroMonsters)
            {
                if (Bot.GetRemainingCount(id, 1) > 0)
                    return id;
            }

            return -1; // 서치할 대상 없음
        }

        private bool CanActivateSwordRyzealEffect()
        {
            if (IsActivateEffectUsedThisTurn(CardId.SwordRyzeal)) // HOPT: 발동효과 턴당 1번 (룰특수소환과 별개)
                return false;

            if (Card == null || Card.Location != CardLocation.MonsterZone)
                return false;

            if (!Bot.HasInMonstersZone(CardId.SwordRyzeal))
                return false;

            return GetSwordRyzealSearchTarget() != -1;
        }

        private bool CommitActivateSwordRyzealEffect()
        {
            int target = GetSwordRyzealSearchTarget();
            if (target == -1) return false; // 안전장치: Can*을 안 거치고 호출돼도 잘못된 선언 방지

            MarkActivateEffectUsedThisTurn(CardId.SwordRyzeal);
            AI.SelectCard(target);
            return true;
        }

        private bool SwordRyzealEffect()
        {
            if (!CanActivateSwordRyzealEffect()) return false;
            return CommitActivateSwordRyzealEffect();
        }

        public override void OnNewTurn()
        {
            base.OnNewTurn();
            NormalSummonUsed = false;
            _activateEffectUsedThisTurn.Clear();
            _specialSummonEffectUsedThisTurn.Clear();
            _iceNormalSummonedThisTurn = false;
        }

        // *** 정정 필요 (c27204311.lua 확인 결과) ***
        // 실제 조건: Duel.GetFlagEffect(1-tp,id)>=5 (상대가 이번 턴 5번 이상 소환했는가)
        // 지금 아래 구현("상대 몬스터 수가 더 많으면")은 완전히 다른(틀린) 조건임.
        // "이번 턴 상대 소환 횟수" 누적 카운터를 얻을 방법이 WindBot Executor 훅 중엔 없어서
        // (OnSummonSuccess류 콜백이 Executor.cs에 없음) 정확히 재현 불가능.
        // 게다가 EFFECT_TYPE_QUICK_O + EVENT_FREE_CHAIN이라 원래 상대 턴 체인 대응 카드라,
        // 이번 개발 범위(메인페이즈 전개, 2차 목표=상대턴 대응은 보류 중) 밖의 카드임.
        // AddExecutor 등록은 레거시 경로용으로 남겨두되, GenerateCandidates에서는 제외함.
        private bool NibiruEffect()
        {
            // 단순히 상대 몬스터가 더 많으면 발동
            return Bot.GetMonsterCount() <= 0 || Bot.GetMonsterCount() < Enemy.GetMonsterCount();
        }

        // 순수 조건 판정: 지금 CrossoutDesignator로 무효화할 만한 대상이 있는지만 계산.
        // 부작용(AI.SelectAnnounceID) 없음 -> 후보 스캔 단계에서 몇 번을 호출해도 안전.
        private int GetCrossOutDesignatorTarget()
        {
            if (Duel.LastChainPlayer != 1) return -1;

            ClientCard lastChainCard = Util.GetLastChainCard();
            if (lastChainCard == null) return -1;

            if (lastChainCard.IsCode(CardId.InfiniteImpermanence)) return CardId.InfiniteImpermanence;
            if (lastChainCard.IsCode(CardId.AshBlossomAndJoyousSpring)) return CardId.AshBlossomAndJoyousSpring;
            // 참고: CalledByTheGrave는 CardId에 정의는 돼 있지만 최신 확정 덱리스트(40장)엔
            // 포함되지 않은 카드라 이 분기는 사실상 항상 미스가 됨. 덱에서 뺀 게 맞다면
            // 이 줄도 같이 정리하는 게 좋음.
            if (lastChainCard.IsCode(CardId.CalledByTheGrave)) return CardId.CalledByTheGrave;

            return -1;
        }

        private bool CanActivateCrossOutDesignator()
        {
            return GetCrossOutDesignatorTarget() != -1;
        }

        private bool CommitActivateCrossOutDesignator()
        {
            int target = GetCrossOutDesignatorTarget();
            if (target == -1) return false; // 안전장치: Can*을 안 거치고 호출돼도 잘못된 선언 방지
            AI.SelectAnnounceID(target);
            return true;
        }

        private bool CrossOutNegate()
        {
            if (!CanActivateCrossOutDesignator()) return false;
            return CommitActivateCrossOutDesignator();
        }

        private bool CanSpecialSummonTraptrixRafflesia()
        {
            return Util.IsTurn1OrMain2() && Bot.GetRemainingCount(CardId.GravediggersTrapHole, 1) > 0;
        }

        private bool CommitSpecialSummonTraptrixRafflesia()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        private bool TraptrixRafflesiaSummon()
        {
            if (!CanSpecialSummonTraptrixRafflesia()) return false;
            return CommitSpecialSummonTraptrixRafflesia();
        }

        /// <summary>
        /// Mulcharmy Purulia는 DefaultExecutor.cs에 참조가 없음(Fuwalos만 있음).
        /// 효과/사용 방식은 Maxx "C"와 거의 동일(상대 턴이면 항상 발동)하지만,
        /// 실제 카드 텍스트상 "자신 필드에 카드가 없을 것"을 요구하므로 그 조건만 추가.
        /// (Fuwalos처럼 턴당 발동 횟수 제한/조건 분기까지는 필요 없음 - 단순화)
        /// </summary>
        private bool DefaultMulcharmyPurulia()
        {
            if (Duel.Player != 1)
                return false;

            if (Bot.GetMonsters().Count > 0 || Bot.GetSpells().Count > 0)
                return false;

            return true;
        }

        private bool DefaultMulcharmyFuwalos()
        {
            return DefaultMulcharmyPurulia();
        }

        #endregion Utils


        #region Candidate Generation / Trace

        // Baseline evaluator only. It is intentionally not a future-state value model.
        private Dictionary<int, double> GetCurrentPowers()
        {
            var state = GameStateAdapter.ToGameState(Bot, Duel, NormalSummonUsed);
            return CardValueEngine.EvaluateAll(state);
        }

        private double GetPowerOrZero(Dictionary<int, double> powers, int cardId)
        {
            double value;
            return powers.TryGetValue(cardId, out value) ? value : 0;
        }

        private readonly DuelDecisionLogger _decisionLogger =
            new DuelDecisionLogger("logs/ryzeal-mainphase.jsonl");

        private bool TryGetActionIndex(ClientCard card, ActionType type, long description, out int actionIndex)
        {
            actionIndex = -1;
            if (card == null) return false;
            if (type == ActionType.NormalSummon)
                actionIndex = GetIdleActionIndex(card, "NormalSummon");
            else if (type == ActionType.SpecialSummon)
                actionIndex = GetIdleActionIndex(card, "SpecialSummon");
            else if (type == ActionType.Activate && card.ActionActivateIndex != null)
            {
                int resolvedIndex;
                if (!card.ActionActivateIndex.TryGetValue(description, out resolvedIndex)) return false;
                actionIndex = resolvedIndex;
            }
            return actionIndex >= 0;
        }

        private static int GetIdleActionIndex(ClientCard card, string kind)
        {
            if (card == null || card.ActionIndex == null) return -1;
            int slot;
            switch (kind)
            {
                case "NormalSummon": slot = (int)MainPhaseAction.MainAction.Summon; break;
                case "SpecialSummon": slot = (int)MainPhaseAction.MainAction.SpSummon; break;
                case "Repos": slot = (int)MainPhaseAction.MainAction.Repos; break;
                case "SetMonster": slot = (int)MainPhaseAction.MainAction.SetMonster; break;
                case "SetSpell": slot = (int)MainPhaseAction.MainAction.SetSpell; break;
                default: return -1;
            }
            return slot < card.ActionIndex.Length ? card.ActionIndex[slot] : -1;
        }

        private bool TryGetActionCard(IList<ClientCard> cards, int cardId, ActionType type, out ClientCard found, out int actionIndex)
        {
            found = null; actionIndex = -1;
            if (cards == null) return false;
            foreach (ClientCard card in cards)
                if (card != null && card.Id == cardId && TryGetActionIndex(card, type, -1, out actionIndex))
                { found = card; return true; }
            return false;
        }

        private bool TryGetActivableCard(MainPhase main, int cardId, out ClientCard found, out long description, out int actionIndex)
        {
            found = null; description = -1; actionIndex = -1;
            if (main == null) return false;
            for (int i = 0; i < main.ActivableCards.Count; i++)
            {
                ClientCard card = main.ActivableCards[i];
                long desc = main.ActivableDescs[i];
                if (card != null && card.Id == cardId && TryGetActionIndex(card, ActionType.Activate, desc, out actionIndex))
                { found = card; description = desc; return true; }
            }
            return false;
        }

        private void AddCandidate(List<ActionCandidate> candidates, ActionType type, int cardId, double score,
            string label, string goalTag, ClientCard source, int actionIndex, long desc, Func<bool> commit, string selectionPlan = "")
        {
            candidates.Add(new ActionCandidate {
                ActionType = type, CardId = cardId, BaselineScore = score, Label = label, GoalTag = goalTag,
                SelectionPlan = selectionPlan, ActionIndex = actionIndex, ActivationDescription = desc,
                // Candidate generation must not leave Executor.Card pointing to an arbitrary last-scanned effect.
                Commit = () => { if (type == ActionType.Activate) SetCard(ExecutorType.Activate, source, desc); return commit(); }
            });
        }

        private List<ActionCandidate> GenerateCandidates(MainPhase main)
        {
            var result = new List<ActionCandidate>();
            var powers = GetCurrentPowers();
            ClientCard card; int index; long desc;

            if (TryGetActionCard(main.SummonableCards, CardId.IceRyzeal, ActionType.NormalSummon, out card, out index) && CanSummonIceRyzeal())
                AddCandidate(result, ActionType.NormalSummon, CardId.IceRyzeal, GetPowerOrZero(powers, CardId.IceRyzeal), "IceRyzeal normal summon", "BuildRank4Board", card, index, -1, CommitSummonIceRyzeal);

            if (TryGetActivableCard(main, CardId.IceRyzeal, out card, out desc, out index))
            { SetCard(ExecutorType.Activate, card, desc); if (CanActivateIceRyzealEffect()) AddCandidate(result, ActionType.Activate, CardId.IceRyzeal, GetPowerOrZero(powers, CardId.IceRyzeal), "IceRyzeal summon-from-deck effect", "BuildRank4Board", card, index, desc, CommitActivateIceRyzealEffect); }
            if (TryGetActivableCard(main, CardId.NodeRyzeal, out card, out desc, out index))
            { SetCard(ExecutorType.Activate, card, desc); if (CanActivateNodeRyzealEffect()) AddCandidate(result, ActionType.Activate, CardId.NodeRyzeal, GetPowerOrZero(powers, CardId.NodeRyzeal), "NodeRyzeal revive effect", "RecoverResource", card, index, desc, CommitActivateNodeRyzealEffect, GetNodeRyzealSelectionPlan()); }
            if (TryGetActivableCard(main, CardId.ExtRyzeal, out card, out desc, out index))
            { SetCard(ExecutorType.Activate, card, desc); if (CanActivateExtRyzealEffect()) AddCandidate(result, ActionType.Activate, CardId.ExtRyzeal, GetPowerOrZero(powers, GetExtRyzealSearchTarget()), "ExtRyzeal search effect", "FindExtender", card, index, desc, CommitActivateExtRyzealEffect); }
            if (TryGetActivableCard(main, CardId.SwordRyzeal, out card, out desc, out index))
            { SetCard(ExecutorType.Activate, card, desc); if (CanActivateSwordRyzealEffect()) AddCandidate(result, ActionType.Activate, CardId.SwordRyzeal, GetPowerOrZero(powers, GetSwordRyzealSearchTarget()), "SwordRyzeal search effect", "FindExtender", card, index, desc, CommitActivateSwordRyzealEffect); }

            if (TryGetActionCard(main.SpecialSummonableCards, CardId.IceRyzeal, ActionType.SpecialSummon, out card, out index) && CanSpecialSummonIceRyzeal()) AddCandidate(result, ActionType.SpecialSummon, CardId.IceRyzeal, GetPowerOrZero(powers, CardId.IceRyzeal), "IceRyzeal rule special summon", "BuildRank4Board", card, index, -1, CommitSpecialSummonIceRyzeal);
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.SwordRyzeal, ActionType.SpecialSummon, out card, out index) && CanSpecialSummonSwordRyzeal()) AddCandidate(result, ActionType.SpecialSummon, CardId.SwordRyzeal, GetPowerOrZero(powers, CardId.SwordRyzeal), "SwordRyzeal rule special summon", "BuildRank4Board", card, index, -1, CommitSpecialSummonSwordRyzeal);
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.NodeRyzeal, ActionType.SpecialSummon, out card, out index) && CanSpecialSummonNodeRyzeal()) AddCandidate(result, ActionType.SpecialSummon, CardId.NodeRyzeal, GetPowerOrZero(powers, CardId.NodeRyzeal), "NodeRyzeal rule special summon", "RecoverResource", card, index, -1, CommitSpecialSummonNodeRyzeal);
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.ExtRyzeal, ActionType.SpecialSummon, out card, out index) && CanSpecialSummonExtRyzeal()) AddCandidate(result, ActionType.SpecialSummon, CardId.ExtRyzeal, GetPowerOrZero(powers, CardId.ExtRyzeal), "ExtRyzeal rule special summon", "SetUpGraveyard", card, index, -1, CommitSpecialSummonExtRyzeal);
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.StarRyzeal, ActionType.SpecialSummon, out card, out index) && CanSpecialSummonStarRyzeal()) AddCandidate(result, ActionType.SpecialSummon, CardId.StarRyzeal, GetPowerOrZero(powers, CardId.StarRyzeal), "StarRyzeal rule special summon", "BuildRank4Board", card, index, -1, CommitSpecialSummonStarRyzeal);
            // Engine availability is required; the local material list is only a selection policy.
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.RyzealDetonator, ActionType.SpecialSummon, out card, out index))
            {
                var materials = GetDetonatorMaterials();
                if (materials.Count == 2)
                    AddCandidate(result, ActionType.SpecialSummon, CardId.RyzealDetonator,
                        GetPowerOrZero(powers, CardId.RyzealDetonator), "Detonator Xyz summon", "BuildDetonator",
                        card, index, -1, CommitDetonatorSummon,
                        "materials:" + String.Join(",", materials.Select(c => c.Id)) + ";count:2");
            }
            if (TryGetActionCard(main.SpecialSummonableCards, CardId.RyzealDuoDrive, ActionType.SpecialSummon, out card, out index))
            {
                var materials = GetDuoDriveMaterials();
                if (materials.Count == 2)
                    AddCandidate(result, ActionType.SpecialSummon, CardId.RyzealDuoDrive,
                        GetPowerOrZero(powers, CardId.RyzealDuoDrive), "Duo Drive Xyz summon", "BuildDuoDrive",
                        card, index, -1, CommitDuoDriveSummon,
                        "materials:" + String.Join(",", materials.Select(c => c.Id)) + ";count:2");
            }
            return result;
        }

        private ClientCard _pendingDetonatorDestroySource;

        private bool DetonatorDestroyEffect()
        {
            if (Card == null || Card.Id != CardId.RyzealDetonator ||
                Card.Location != CardLocation.MonsterZone ||
                ActivateDescription != Util.GetStringId(CardId.RyzealDetonator, 0)) return false;
            // Engine supplies legal opponent-chain activation; do not invent an HOPT here.
            // Avoid activating when only our own field could be destroyed.
            if (Enemy.GetMonsters().Count + Enemy.GetSpells().Count == 0) return false;
            _pendingDetonatorDestroySource = Card;
            return true;
        }

        public override void OnChainEnd()
        {
            _pendingDetonatorDestroySource = null;
            base.OnChainEnd();
        }

        private bool DetonatorAttachEffect()
        {
            if (Card == null || Card.Id != CardId.RyzealDetonator ||
                Card.Location != CardLocation.MonsterZone) return false;
            // aux.Stringid(id,1): attach. Description 0 is the destruction response.
            // Engine handles count limit and attachfilter legality. Do not consume an ignition HOPT.
            return ActivateDescription == Util.GetStringId(CardId.RyzealDetonator, 1);
        }

        private static int DuoDriveSearchPriority(int id)
        {
            int[] priority = { CardId.ExtRyzeal, CardId.SwordRyzeal, CardId.IceRyzeal,
                CardId.NodeRyzeal, CardId.StarRyzeal };
            int index = Array.IndexOf(priority, id);
            return index < 0 ? priority.Length : index;
        }

        private bool DuoDriveSearchEffect()
        {
            // Only called for an engine-offered activation, which checks materials and searchability.
            return Card != null && Card.Id == CardId.RyzealDuoDrive &&
                Card.Location == CardLocation.MonsterZone &&
                ActivateDescription == Util.GetStringId(CardId.RyzealDuoDrive, 1);
        }

        private bool DuoDriveAttachEffect()
        {
            if (Card == null || Card.Id != CardId.RyzealDuoDrive ||
                Card.Location != CardLocation.MonsterZone) return false;
            // c7511613.lua: description 0 = attach, description 1 = search.
            // The engine enforces the separate count limit and eligible graveyard targets.
            return ActivateDescription == Util.GetStringId(CardId.RyzealDuoDrive, 0);
        }

        private List<ClientCard> GetDuoDriveMaterials()
        {
            // Unlike Detonator, Duo Drive allows ANY Level 4 monsters.
            // Preserve an unused Node, then prefer spent Ryzeal effects.
            return Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.Level == 4)
                .OrderBy(c => c.Id == CardId.NodeRyzeal && !IsActivateEffectUsedThisTurn(c.Id) ? 1 : 0)
                .ThenByDescending(c => IsActivateEffectUsedThisTurn(c.Id))
                .ThenBy(c => c.Id).Take(2).ToList();
        }

        private bool CommitDuoDriveSummon()
        {
            var materials = GetDuoDriveMaterials();
            if (materials.Count != 2) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        // Step 03: summon only. Trigger attachment and interruption are separate next steps.
        private List<ClientCard> GetDetonatorMaterials()
        {
            return Bot.GetMonsters()
                .Where(c => c.IsFaceup() && c.Level == 4 && IsRyzealMonster(c.Id))
                // Prefer bodies whose ignition/search effects are already spent.
                .OrderByDescending(c => IsActivateEffectUsedThisTurn(c.Id))
                .ThenBy(c => c.Id == CardId.NodeRyzeal ? 1 : 0)
                .ThenBy(c => c.Id)
                .Take(2).ToList();
        }

        private bool CommitDetonatorSummon()
        {
            var materials = GetDetonatorMaterials();
            if (materials.Count != 2) return false;
            AI.SelectMaterials(materials);
            return true;
        }

        private DecisionTrace BuildTrace(MainPhase main, List<ActionCandidate> candidates)
        {
            var t = new DecisionTrace { Turn = Duel.Turn, Phase = Duel.Phase.ToString(), TurnPlayer = Duel.Player, IsFirst = Duel.IsFirst, NormalSummonUsed = NormalSummonUsed };
            CopyField(Bot, t.Self); CopyField(Enemy, t.Opponent);
            AddLegal(t, main.SummonableCards, "NormalSummon", -1); AddLegal(t, main.SpecialSummonableCards, "SpecialSummon", -1);
            // Logging must never make a legal engine action crash the duel. Some effects can be
            // exposed without an ActionActivateIndex entry, so record -1 when the index is absent.
            for (int i = 0; i < main.ActivableCards.Count; i++)
            {
                ClientCard card = main.ActivableCards[i];
                long desc = main.ActivableDescs[i];
                int actionIndex = -1;
                if (card != null && card.ActionActivateIndex != null)
                {
                    int resolvedIndex;
                    if (card.ActionActivateIndex.TryGetValue(desc, out resolvedIndex))
                        actionIndex = resolvedIndex;
                }
                t.LegalActions.Add(new LegalActionTrace
                {
                    Kind = "Activate",
                    CardId = card == null ? 0 : card.Id,
                    ActivationDescription = desc,
                    ActionIndex = actionIndex
                });
            }
            AddAdditionalLegal(t, main.MonsterSetableCards, "SetMonster");
            AddAdditionalLegal(t, main.SpellSetableCards, "SetSpell");
            AddAdditionalLegal(t, main.ReposableCards, "Repos");
            if (main.CanBattlePhase) t.LegalActions.Add(new LegalActionTrace { Kind = "ToBattlePhase", ActionIndex = -1, ActivationDescription = -1 });
            if (main.CanEndPhase) t.LegalActions.Add(new LegalActionTrace { Kind = "ToEndPhase", ActionIndex = -1, ActivationDescription = -1 });
            foreach (var c in candidates) t.PolicyCandidates.Add(ToTrace(c));
            return t;
        }
        private static void AddAdditionalLegal(DecisionTrace trace, IList<ClientCard> cards, string kind)
        {
            foreach (var card in cards)
                if (card != null) trace.LegalActions.Add(new LegalActionTrace
                    { Kind = kind, CardId = card.Id, ActionIndex = GetIdleActionIndex(card, kind), ActivationDescription = -1 });
        }
        private static void CopyField(ClientField source, FieldTrace target) { target.Hand = source.Hand.Select(c => c.Id).ToList(); target.Monsters = source.GetMonsters().Select(c => c.Id).ToList(); target.Spells = source.GetSpells().Select(c => c.Id).ToList(); target.Graveyard = source.Graveyard.Select(c => c.Id).ToList(); target.Banished = source.Banished.Select(c => c.Id).ToList(); target.Extra = source.ExtraDeck.Select(c => c.Id).ToList(); }
        private static void AddLegal(DecisionTrace t, IList<ClientCard> cards, string kind, long desc) { foreach (var c in cards) t.LegalActions.Add(new LegalActionTrace { Kind = kind, CardId = c.Id, ActivationDescription = desc, ActionIndex = GetIdleActionIndex(c, kind) }); }
        private static CandidateTrace ToTrace(ActionCandidate c) { return new CandidateTrace { Kind = c.ActionType.ToString(), CardId = c.CardId, BaselineScore = c.BaselineScore, Label = c.Label, GoalTag = c.GoalTag, SelectionPlan = c.SelectionPlan }; }

        public void ObserveDuelStart()
        {
            _decisionLogger.BeginDuel();
            _pendingDetonatorDestroySource = null;
        }

        public void ObserveLegacyMainPhaseAction(string kind, int cardId, long description)
        {
            _decisionLogger.WriteLegacyAction(Duel.Turn, Duel.Phase.ToString(), kind, cardId, description);
        }

        public override bool TryGetMainPhaseAction(MainPhase main, out MainPhaseAction action)
        {
            action = null;
            _decisionLogger.BeginDecision();
            var candidates = GenerateCandidates(main);
            var trace = BuildTrace(main, candidates);
            foreach (var c in candidates.OrderByDescending(x => x.BaselineScore).ThenBy(x => x.Label, StringComparer.Ordinal))
            {
                if (!c.Commit()) continue;
                trace.Selected = ToTrace(c); trace.SelectionResult = "committed"; _decisionLogger.Write(trace);
                MainPhaseAction.MainAction kind = c.ActionType == ActionType.NormalSummon ? MainPhaseAction.MainAction.Summon : c.ActionType == ActionType.SpecialSummon ? MainPhaseAction.MainAction.SpSummon : MainPhaseAction.MainAction.Activate;
                action = new MainPhaseAction(kind, c.ActionIndex); return true;
            }
            trace.SelectionResult = candidates.Count == 0 ? "no-policy-candidate; legacy-executor-fallback" : "all-commits-failed; legacy-executor-fallback";
            _decisionLogger.Write(trace); return false;
        }
        #endregion Candidate Generation / Trace
    }
}
