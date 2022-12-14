using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using System.Linq;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("Benkei", "AI_Benkei")]
    public class BenkeiExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int WanderingGryphonRider = 2563463;
            public const int Fateful = 39568067;
            public const int Enchantress = 30680659;
            public const int Aramesir = 3285551;
            public const int AdventurerToken = 3285552;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int Benkei = 84430950;
            public const int PotofExtravagance = 49238328;
            public const int Mataza = 22609617;
            public const int Hayabusa = 21015833;
            public const int LightningStorm = 14532163;
            public const int Dracoback = 38745520;
            public const int AxeDespair = 40619825;
            public const int MaskBrutality = 82432018;
            public const int MagePower = 83746708;
            public const int BashingShield = 88610708;
            public const int Gallatin = 14745409;
            public const int Cowboy = 12014404;
        }
        public BenkeiExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            //Cowboy Burn
            AddExecutor(ExecutorType.SpSummon, CardId.Cowboy, CowboySummon);
            AddExecutor(ExecutorType.Activate, CardId.Cowboy);
            //Counters
            AddExecutor(ExecutorType.Activate, CardId.PotofExtravagance, PotActivate);
            AddExecutor(ExecutorType.Activate, CardId.WanderingGryphonRider, GryphonEffect);
            AddExecutor(ExecutorType.Activate, CardId.LightningStorm, LightningStormActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CosmicCyclone, DefaultCosmicCyclone);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, _CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, _CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            AddExecutor(ExecutorType.Activate, _CardId.Raigeki, DefaultRaigeki);
            AddExecutor(ExecutorType.SpSummon, _CardId.GamecieltheSeaTurtleKaiju, DefaultKaijuSpsummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);
            AddExecutor(ExecutorType.Activate, CardId.Dracoback, DracobackEffect);
            AddExecutor(ExecutorType.Activate, CardId.Dracoback, DracobackEquip);
            //OTK Setup
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, RotaSearch);
            AddExecutor(ExecutorType.Summon, CardId.Benkei, WarriorSummon);
            AddExecutor(ExecutorType.Summon, CardId.Mataza, WarriorSummon);
            AddExecutor(ExecutorType.Summon, CardId.Hayabusa, WarriorSummon);
            AddExecutor(ExecutorType.Activate, CardId.MagePower, GenericEquip);
            AddExecutor(ExecutorType.Activate, CardId.AxeDespair, GenericEquip);
            AddExecutor(ExecutorType.Activate, CardId.MaskBrutality, GenericEquip);
            AddExecutor(ExecutorType.Activate, CardId.BashingShield, WarriorEquip);
            AddExecutor(ExecutorType.Activate, CardId.Gallatin, WarriorEquip);
            //Adventurer Setup
            AddExecutor(ExecutorType.Activate, CardId.Aramesir, AdventurerSummon);
            AddExecutor(ExecutorType.Activate, CardId.Enchantress, EnchantressEffect);
            AddExecutor(ExecutorType.Activate, CardId.Fateful, FatefulEffect);
            AddExecutor(ExecutorType.Activate, CardId.WanderingGryphonRider, GryphonSummon);
            //End of Main
            AddExecutor(ExecutorType.SpellSet, DefaultSpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
        }
        public bool EnchantressUsed = false;

        public override void OnNewTurn()
        {
            EnchantressUsed = false;
        }

        public override int OnSelectOption(IList<long> options)
        {
            return Program.Rand.Next(options.Count);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack < 0)
                    return CardPosition.FaceUpAttack;
                if (cardData.Attack <= 499)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }
        private bool WarriorSummon()
        {
            if (Duel.Turn == 1)
                return false;
            else return true;
        }

        private bool GryphonEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            return Duel.LastChainPlayer == 1;
        }
        private bool GryphonSummon()
        {
            if (Card.Location != CardLocation.Hand)
                return false;
            if (Duel.Turn == 1) AI.SelectPosition(CardPosition.FaceUpDefence);
            return Bot.HasInMonstersZone(CardId.AdventurerToken) || (Duel.Player == 0 && (Duel.LastChainPlayer == -1 || Bot.HasInSpellZone(CardId.Fateful)));
        }
        public bool AdventurerSummon()
        {
            if (Enemy.HasInGraveyard(43534808)) return false; //Token Collector
            AI.SelectYesNo(true);
            if (Duel.Turn == 1) AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool EnchantressEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                AI.SelectCard(CardLocation.Deck);
                EnchantressUsed = true;
                return true;
            }
            if (ActivateDescription == Util.GetStringId(CardId.Enchantress, 0))
            {
                // summon
                return false;
            }
            else
            {
                // search
                return !Bot.HasInHand(CardId.Aramesir) && !Bot.HasInMonstersZone(CardId.AdventurerToken);
            }
        }
        private bool FatefulEffect()
        {
            if (Card.Location == CardLocation.Hand)
                return false;
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.Fateful, 1))
            {
                // search equip to hand
                AI.SelectOption(0);
                return true;
            }
            else
            {
                // search rider or aquamancer
                if (Bot.GetRemainingCount(CardId.WanderingGryphonRider, 1) == 0 || Bot.GetHandCount() == 0 || !Bot.HasInMonstersZone(CardId.AdventurerToken))
                {
                    AI.SelectCard(CardId.Enchantress);
                    if (Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.Enchantress))
                        AI.SelectNextCard(CardId.Enchantress);
                }
                return true;
            }
        }
        public bool PotActivate()
        {
            if (SpellNegatable()) return false;
            AI.SelectOption(1);
            return true;
        }
        public bool SpellNegatable(bool isCounter = false, ClientCard target = null)
        {
            // target default set
            if (target == null) target = Card;
            // won't negate if not on field
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            // negate judge
            if (Enemy.HasInMonstersZone(99916754, true) && !isCounter) return true; //Naturia Exterio
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(33198837, true)) return true; //Naturia Beast
                if (Enemy.HasInSpellZone(61740673, true) || Enemy.HasInSpellZone(511002996, true)) return true; //Imperial Order
                if (Enemy.HasInMonstersZone(37267041, true) || Bot.HasInMonstersZone(37267041, true)) return true; //Silent Swordsman
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(51452091, true) || Bot.HasInSpellZone(51452091, true)) return true; //Royal Decree
                if (Enemy.HasInMonstersZone(73551138, true)) return true; //Gishki Emilia
                if (Enemy.HasInSpellZone(37209439, true) || Bot.HasInSpellZone(37209439, true)) return true; //Dark Contract
            }
            // how to get here?
            return false;
        }
        public bool RotaSearch()
        {
            AI.SelectCard(CardId.Benkei, CardId.Mataza, CardId.Hayabusa);
            return true;
        }
        public bool LightningStormActivate()
        {
            int bestPower = 0;
            foreach (ClientCard hand in Bot.Hand)
            {
                if (hand.IsMonster() && hand.Level <= 4 && hand.Attack > bestPower) bestPower = hand.Attack;
            }

            int opt = -1;
            // destroy monster
            if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsFloodgate() && card.IsAttack()) != null
                || Enemy.MonsterZone.GetMatchingCardsCount(card => card.IsAttack() && card.Attack >= bestPower) >= 2) opt = 0;
            // destroy spell/trap
            else if (Enemy.GetSpellCount() >= 2 || Util.GetProblematicEnemySpell() != null) opt = 1;

            if (opt == -1) return false;

            // only one selection
            if (Enemy.MonsterZone.GetFirstMatchingCard(card => card.IsAttack()) == null
                || Enemy.GetSpellCount() == 0)
            {
                AI.SelectOption(0);
                return true;
            }
            AI.SelectOption(opt);
            return true;
        }
        private bool DracobackEffect()
        {
            if (Card.Location != CardLocation.SpellZone)
                return false;
            ClientCard target = Util.GetProblematicEnemyCard();
            AI.SelectCard(target);
            return true;
        }

        private bool DracobackEquip()
        {
            if (Card.Location == CardLocation.SpellZone)
                return false;
            if (Card.Location == CardLocation.Grave)
                return true;
            if (Bot.HasInMonstersZone(CardId.AdventurerToken, faceUp: true))
            {
                AI.SelectCard(CardId.AdventurerToken);
                return true;
            }
            return false;
        }
        private bool GenericEquip()
        {
            if (Card.Location == CardLocation.SpellZone)
                return false;
            if (Duel.Turn == 1)
                return false;
            if (Bot.GetMonsterCount() > 0)
            {
                AI.SelectCard(CardId.Benkei, CardId.Mataza, CardId.Hayabusa, CardId.AdventurerToken, CardId.Enchantress);
                return true;
            }
            return false;
        }
        private bool WarriorEquip()
        {
            if (Card.Location == CardLocation.SpellZone)
                return false;
            if (Duel.Turn == 1)
                return false;
            if (Bot.GetMonsterCount() > 0)
            {
                AI.SelectCard(CardId.Benkei, CardId.Mataza, CardId.Hayabusa);
                return true;
            }
            return false;
        }
        public override bool OnSelectHand()
        {
            // Going second deck.
            return false;
        }
        private bool CowboySummon()
        {
            if (Enemy.LifePoints <= 800 || (Bot.GetMonsterCount() >= 4 && Enemy.LifePoints <= 1600))
            {
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }
    }
}
