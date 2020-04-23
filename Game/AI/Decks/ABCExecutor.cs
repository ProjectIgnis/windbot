using System;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using YGOSharp.Network.Enums;
using YGOSharp.OCGWrapper;

namespace WindBot.Game.AI.Decks
{
    [Deck("ABC", "AI_ABC")]
    class ABCExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int UnionDriver = 99249638;
            public const int GalaxySoldier = 46659709;
            public const int PhotonThrasher = 65367484;
            public const int AAssaultCore = 30012506;
            public const int BBusterDrake = 77411244;
            public const int CCrushWyvern = 3405259;
            public const int PhotonOrbital = 89132148;
            public const int PhotonVanisher = 43147039;
            public const int HeavyMechSupportArmor = 39890958;
            public const int AshBlossomJoyousSpring = 14558128;

            public const int ReinaforcmentOfTheArmy = 32807846;
            public const int Terraforming = 73628505;
            public const int MalefactorsCommand = 12524259;
            public const int CalledbyTheGrave = 24224830;
            public const int CrossOutDesignator = 65681983;
            public const int UnionHanger = 66399653;
            public const int InfiniteImpermanence = 10045474;
            public const int SolemnStrike = 40605147;

            public const int ABCDragonBuster = 1561110;
            public const int CyberDragonInfinity = 10443957;
            public const int CyberDragonNova = 58069384;
            public const int CrystronHalqifibrax = 50588353;
            public const int UnionCarrier = 83152482;
            public const int IPMasquerina = 65741786;
            public const int CrusadiaAvramax = 21887175;
            public const int ApollousaBOG = 4280258;
            public const int KnightmareUnicorn = 38342335;
            public const int KnightmarePhoenix = 2857636;
            public const int KnightmareCerberus = 75452921;
        }

        public ABCExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossomJoyousSpring, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.CalledbyTheGrave, DefaultCalledByTheGrave);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImpermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.CrossOutDesignator, CrossOutNegate);
            AddExecutor(ExecutorType.Activate, CardId.ApollousaBOG, ApollousaNegate);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonInfinity, CyberDragonInfinityNegate);
            AddExecutor(ExecutorType.Activate, CardId.CyberDragonInfinity, CyberDragonInfinityAttach);
            AddExecutor(ExecutorType.Activate, CardId.IPMasquerina, IPMasquerinaEffect);
            AddExecutor(ExecutorType.Activate, CardId.ABCDragonBuster, ABCBanish);
            AddExecutor(ExecutorType.Activate, CardId.ABCDragonBuster, ABCUnionSummon);

            AddExecutor(ExecutorType.Activate, CardId.UnionHanger, UnionHangerActivate);
            AddExecutor(ExecutorType.Activate, CardId.UnionHanger, UnionHangerEquip);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonThrasher, PhotonSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.PhotonVanisher, PhotonSummon);
            AddExecutor(ExecutorType.Activate, CardId.PhotonOrbital, PhotonOrbitalEquip);
            AddExecutor(ExecutorType.Activate, CardId.PhotonOrbital, PhotonOrbitalEffect);
            AddExecutor(ExecutorType.Activate, CardId.ReinaforcmentOfTheArmy, ROTAEffect);
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.MalefactorsCommand, MalefactorsCommandEffect);
            AddExecutor(ExecutorType.Activate, CardId.UnionDriver, UnionDriverEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragonNova, CyberDragonNovaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.CyberDragonInfinity, CyberDragonInfinitySummon);

            AddExecutor(ExecutorType.Summon, CardId.AAssaultCore, AAssaultCoreSummon);
            AddExecutor(ExecutorType.Summon, CardId.BBusterDrake, BBusterDrakeSummon);
            AddExecutor(ExecutorType.Summon, CardId.CCrushWyvern, CCrushWyvernSummon);
            AddExecutor(ExecutorType.Activate, CardId.AAssaultCore, UnionSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.BBusterDrake, UnionSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.CCrushWyvern, UnionSpSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareUnicorn, KnightmareUnicornSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmarePhoenix, KnightmarePhoenixSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.KnightmareCerberus, KnightmareCerberusSummon);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareUnicorn, KnightmareUnicornEffect);
            AddExecutor(ExecutorType.Activate, CardId.KnightmarePhoenix, KnightmarePhoenixEffect);
            AddExecutor(ExecutorType.Activate, CardId.KnightmareCerberus, KnightmareCerberusEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.UnionCarrier, UnionCarrierSummon);
            AddExecutor(ExecutorType.Activate, CardId.UnionCarrier, UnionCarrierEffect);
            AddExecutor(ExecutorType.Activate, CardId.GalaxySoldier, GalaxySoldierSpSummon);
            AddExecutor(ExecutorType.Activate, CardId.GalaxySoldier, GalaxySoldierEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.CrusadiaAvramax, CrusadiaAvramaxSummon);
            AddExecutor(ExecutorType.Activate, CardId.CrusadiaAvramax, CrusadiaAvramaxEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.IPMasquerina, IPMasquerinaSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.ABCDragonBuster, ABCDragonBusterSummon);
            AddExecutor(ExecutorType.Activate, CardId.CCrushWyvern, CCrushWyvernEffect);
            AddExecutor(ExecutorType.Activate, CardId.BBusterDrake, BBusterDrakeEffect);
            AddExecutor(ExecutorType.Activate, CardId.AAssaultCore, AAssaultCoreEffect);

            AddExecutor(ExecutorType.SpellSet, CardId.CalledbyTheGrave, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.InfiniteImpermanence, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.CrossOutDesignator, TrapSet);
            AddExecutor(ExecutorType.SpellSet, CardId.SolemnStrike, TrapSet);


        }
        private bool UnionHangerActivated = false;
        private bool UnionHangerEquiped = false;
        private bool UnionDriverUsed = false;
        private bool PhotonOrbitalUsed = false;
        private bool GalaxySoldierUsed = false;
        private bool ABCBanishUsed = false;
        private bool ABCUnionSummonUsed = false;
        private bool UnionCarrierSummonTurn = false;

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        public override void OnNewTurn()
        {
            UnionHangerActivated = false;
            UnionHangerEquiped = false;
            UnionDriverUsed = false;
            PhotonOrbitalUsed = false;
            GalaxySoldierUsed = false;
            ABCBanishUsed = false;
            ABCUnionSummonUsed = false;
            UnionCarrierSummonTurn = false;
        }

        private bool CrossOutNegate()
        {
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard LastChainCard = Util.GetLastChainCard();
                if (LastChainCard.IsCode(CardId.InfiniteImpermanence))
                {
                    AI.SelectAnnounceID(CardId.InfiniteImpermanence);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.AshBlossomJoyousSpring))
                {
                    AI.SelectAnnounceID(CardId.AshBlossomJoyousSpring);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.CalledbyTheGrave))
                {
                    AI.SelectAnnounceID(CardId.CalledbyTheGrave);
                    return true;
                }
            }
            return false;
        }

        private bool ABCBanish()
        {
            if (ActivateDescription == Util.GetStringId(CardId.ABCDragonBuster, 1))
                return false;
            if (Duel.LastChainPlayer == 0 || ABCBanishUsed)
                return false;
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target == null || ((target.HasType(CardType.Spell) || target.HasType(CardType.Trap)) && (!target.HasType(CardType.Equip) && !target.HasType(CardType.Continuous) && !target.HasType(CardType.Field))))
                return false;
            AI.SelectOption(0);
            AI.SelectCard(ABCUnion);
            AI.SelectCard(Card.Location = CardLocation.Hand);
            AI.SelectNextCard(target);
            ABCBanishUsed = true;
            return true;
        }

        private bool ABCUnionSummon()
        {
            if (ActivateDescription == Util.GetStringId(CardId.ABCDragonBuster, 0))
                return false;
            /*ClientCard LastChainCard = Util.GetLastChainCard();
            if (Duel.LastChainPlayer == 0 || !ABCBanishUsed)
            {
                if (LastChainCard != null && LastChainCard.IsCode(CardId.IPMasquerina))
                {
                    AI.SelectOption(0);
                    ABCUnionSummonUsed = true;
                    return true;
                }
                if (LastChainCard != null && !LastChainCard.IsCode(CardId.IPMasquerina))
                    return false;
                return false;
            }*/
            if (Duel.LastChainPlayer == 0)
                return false;
            if (ABCBanishUsed)
            {
                AI.SelectOption(1);
                ABCUnionSummonUsed = true;
                return true;
            }
            return false;
        }

        private bool CyberDragonInfinityNegate()
        {
            if (Duel.LastChainPlayer != 1)
                return false;
            int[] Blacklist =
            {
                70368879,   //Upstart Goblin
                93946239,   //Into the Void
                73628505,   //Terraforming
                55010259,   //Gold Gadget
                29021114,   //Silver Gadget
                34408491,   //Beelze of the Diabolic Dragons
                8763963,    //Beelzeus of the Diabolic Dragons
            };
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard.IsCode(Blacklist))
                return false;
            if (LastChainCard.IsCode(423585) && ActivateDescription == Util.GetStringId(423585, 0))
                return false;
            return true;
        }

        private bool CyberDragonInfinityAttach()
        {
            if (Duel.LastChainPlayer != -1)
                return false;
                if (Enemy.GetMonsterCount() >= 1)
            {
                AI.SelectCard(Util.GetBestEnemyMonster());
                return true;
            }
            return false;
        }

        private bool ApollousaNegate()
        {
            int[] Danger = {
                52350806,
                16209941,
                26302107,
                43316238,
                70711847,
                83518674,
                90807199,
                43694650,
                99745551,
            };
            int[] Blacklist =
            {
                64734921,   //The Agent of Creation - Venus
                34408491,   //Beelze of the Diabolic Dragons
                8763963,    //Beelzeus of the Diabolic Dragons
            };
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard.IsCode(57774843) && ActivateDescription == Util.GetStringId(57774843, 1))
                return false;   //JudgementDragon
            if (LastChainCard.IsCode(423585) && ActivateDescription == Util.GetStringId(423585, 0))
                return false;   //SummonerMonk
            if (LastChainCard.IsCode(43218406) && (LastChainCard.Location == CardLocation.MonsterZone))
                return false;   //WaterGizmek
            if (LastChainCard.IsCode(Danger) && (LastChainCard.Location == CardLocation.Hand))
                return false;
            if (LastChainCard.IsCode(Blacklist))
                return false;
            return true;
        }

        private bool KnightmarePhoenixSummon()
        {
            if (Duel.Turn == 1) return false;
            if (Enemy.SpellZone.GetMatchingCardsCount(card => card.Location == CardLocation.SpellZone) == 0) return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
            };
            if (Enemy.SpellZone.GetMatchingCardsCount(card => card.Location == CardLocation.SpellZone) >= Enemy.MonsterZone.GetMatchingCardsCount(card => card.Location == CardLocation.MonsterZone))
            {
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            return false;
        }

        private bool KnightmareCerberusSummon()
        {
            if (Duel.Turn == 1) return false;
            if (Enemy.MonsterZone.GetMatchingCardsCount(card => card.Location == CardLocation.MonsterZone) == 0) return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
            };
            if (Enemy.MonsterZone.GetMatchingCardsCount(card => card.Location == CardLocation.MonsterZone) >= Enemy.SpellZone.GetMatchingCardsCount(card => card.Location == CardLocation.SpellZone))
            {
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
                {
                    AI.SelectMaterials(materials);
                    return true;
                }
            }
            return false;
        }

        private bool KnightmareUnicornSummon()
        {
            if (Duel.Turn == 1) return false;
            int[] materials = new[] {
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2 && (Bot.HasInMonstersZone(CardId.KnightmareCerberus) || Bot.HasInMonstersZone(CardId.KnightmarePhoenix)))
            {
                List<ClientCard> materials2 = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardLevel);
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!materials2.Contains(card) && card.LinkCount <= 2 && card.IsCode(materials))
                    {
                        materials2.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 3) break;
                    }
                }
                if (link_count >= 3)
                {
                    AI.SelectMaterials(materials2);
                    return true;
                }
            }
            return false;
        }


        private bool KnightmarePhoenixEffect()
        {
            int[] ABCUnion = {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern
            };
            ClientCard target = Util.GetBestEnemySpell();
            if (target == null)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(ABCUnion) || Bot.HasInHand(CardId.PhotonThrasher))
            {
                AI.SelectCard(ABCUnion);
                AI.SelectCard(CardId.PhotonThrasher);
            }
            AI.SelectNextCard(target);
            return true;
        }


        private bool KnightmareCerberusEffect()
        {   
            int[] ABCUnion = {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern
            };
            ClientCard target = Util.GetBestEnemyMonster(canBeTarget: true);
            if (target == null)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(ABCUnion) || Bot.HasInHand(CardId.PhotonThrasher))
            {
                AI.SelectCard(ABCUnion);
                AI.SelectCard(CardId.PhotonThrasher);
            }
            AI.SelectNextCard(target);
            return true;
        }


        private bool KnightmareUnicornEffect()
        {
            int[] ABCUnion = {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern
            };
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target == null)
                return false;
            if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern))
                AI.SelectCard(CardId.AAssaultCore);
            if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                AI.SelectCard(CardId.BBusterDrake);
            if (Bot.HasInHand(ABCUnion) || Bot.HasInHand(CardId.PhotonThrasher))
            {
                AI.SelectCard(ABCUnion);
                AI.SelectCard(CardId.PhotonThrasher);
            }
            AI.SelectNextCard(target);
            return true;
        }

        private int[] ABCUnion = {
            CardId.AAssaultCore,
            CardId.BBusterDrake,
            CardId.CCrushWyvern
        };

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
                if (cardData.HasType(CardType.Union) && Duel.Player == 1)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                return available & ~Bot.GetLinkedZones();
            }
            return 0;
        }


        private bool ROTAEffect()
        {
            if (Bot.HasInHandOrHasInMonstersZone(CardId.PhotonThrasher))
            {
                AI.SelectCard(CardId.PhotonVanisher);
                return true;
            }
            else AI.SelectCard(CardId.PhotonThrasher);
            return true;
        }


        private bool TerraformingEffect()
        {
            if (UnionHangerActivated)
                return false;
            AI.SelectCard(CardId.UnionHanger);
            return true;
        }


        private bool UnionHangerActivate()
        {
            if (UnionHangerActivated)
                return false;
            if (Bot.HasInHand(CardId.BBusterDrake) || Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.CCrushWyvern);
            else
                AI.SelectCard(CardId.BBusterDrake);
            UnionHangerActivated = true;
            return true;
        }

        //Needs to add when to equip other abc instead
        private bool UnionHangerEquip()
        {
            int[] ABCUnion = {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern
            };
            if (UnionHangerEquiped)
                return false;
            if (!ABCUnionSummonUsed && !UnionDriverUsed && Duel.Player == 0)
                AI.SelectCard(CardId.UnionDriver);
            else AI.SelectCard(ABCUnion);
            UnionHangerEquiped = true;
            return true;
        }


        private bool PhotonSummon()
        {
            int[] mats =
            {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern,
                CardId.PhotonThrasher,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) >= 2)
                return false;
            return true;
        }

        private bool PhotonOrbitalEquip()
        {
            if (Card.Location == CardLocation.Hand)
                return true;
            return false;
        }

        private bool CCrushWyvernSummon()
        {
            if (!Bot.HasInHand(CardId.BBusterDrake) && !Bot.HasInHand(CardId.AAssaultCore))
                return false;
            return true;
        }

        private bool BBusterDrakeSummon()
        {
            if (Bot.HasInHand(CardId.CCrushWyvern))
                return false;
            return true;
        }

        private bool AAssaultCoreSummon()
        {
            if (Bot.HasInHand(CardId.CCrushWyvern) || Bot.HasInHand(CardId.BBusterDrake))
                return false;
            return true;
        }



        private bool MalefactorsCommandEffect()
        {
            //Needs to add when to equip other abc instead
            if (Duel.LastChainPlayer == 0)
                return false;
            if (!UnionDriverUsed && !Bot.HasInHandOrInSpellZone(CardId.UnionHanger))
            {
                AI.SelectCard(CardLocation.MonsterZone);
                AI.SelectNextCard(CardId.UnionDriver);
                return true;
            }
            //Needs to add when to equip other abc instead
            if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
            {
                AI.SelectCard(CardId.CyberDragonInfinity);
                AI.SelectNextCard(CardId.HeavyMechSupportArmor);
                return true;
            }
            return false;
        }


        //needs even better logic
        private bool UnionDriverEffect()
        {
            if (UnionDriverUsed)
                return false;
            if (Bot.HasInHand(CardId.AAssaultCore) || Bot.HasInHand(CardId.BBusterDrake))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInMonstersZone(CardId.CCrushWyvern) && Bot.HasInHand(CardId.BBusterDrake) && Bot.HasInHand(CardId.AAssaultCore))
                AI.SelectCard(CardId.CCrushWyvern);
            if (Bot.HasInMonstersZone(CardId.CCrushWyvern))
                AI.SelectCard(CardId.BBusterDrake);
            if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore) && (Bot.HasInMonstersZone(CardId.BBusterDrake) && !Bot.HasInHand(CardId.AAssaultCore)))
                AI.SelectCard(CardId.AAssaultCore);
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.BBusterDrake))
                AI.SelectCard(CardId.BBusterDrake);
            UnionDriverUsed = true;
            return true;
        }

        
        private bool UnionSpSummon()
        {
            if (Card.Location == CardLocation.SpellZone)
                return true;
            return false;
        }

        private bool UnionCarrierSummon()
        {
            if (Bot.HasInMonstersZone(CardId.UnionCarrier, true))
                return false;

            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher
            };
            //(need to allow them if they are equiped by effects other than union driver)
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2 && !Bot.HasInSpellZone(ABCUnion))
            {
                AI.SelectMaterials(materials);
                UnionCarrierSummonTurn = true;
                return true;
            }
            return false;
        }

        private bool CrusadiaAvramaxSummon()
        {
            if (UnionCarrierSummonTurn || Duel.Turn == 1)
                return false;
            int[] materials = new[] {
                CardId.IPMasquerina,
                CardId.UnionCarrier,
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.KnightmareUnicorn,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                List<ClientCard> materials2 = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardLevel);
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!materials2.Contains(card) && card.LinkCount <= 2 && card.IsCode(materials))
                    {
                        materials2.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 4) break;
                    }
                }
                if (link_count >= 4)
                {
                    AI.SelectMaterials(materials2);
                    return true;
                }
            }
            return false;
        }

        private bool CrusadiaAvramaxEffect()
        {
            ClientCard target = Util.GetBestEnemyCard();
            if (target == null)
                return false;
            AI.SelectNextCard(target);
            return true;
        }

        private bool IPMasquerinaSummon()
        {
            if (!UnionCarrierSummonTurn && Duel.Turn == 1)
                return false;
            int[] materials = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.PhotonThrasher,
                CardId.PhotonVanisher,
                CardId.GalaxySoldier,
                CardId.UnionDriver,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool IPMasquerinaEffect()
        {
            int[] MonsterMassRemoval = 
            {
                12580477,   //Raigeki
                53129443,   //Dark Hole
                14532163,   //Lightning Storm
                99330325,   //Interrupted Kaiju Slumber
                69162969,   //Lightning Vortex
                15693423,   //Evenly Matched
                53582587,   //Torrential Tribute
                8251996,    //Ojama Delta Hurricane!!
                44883830,   //Des Croaking
            };
            int[] materials1 = new[] {
                CardId.CCrushWyvern,
                CardId.BBusterDrake,
                CardId.AAssaultCore,
                CardId.IPMasquerina,
            };
            int[] materials2 = new[] {
                CardId.IPMasquerina,
                CardId.UnionCarrier,
                CardId.KnightmareCerberus,
                CardId.KnightmarePhoenix,
                CardId.KnightmareUnicorn,
            };
            if (Duel.LastChainPlayer == 0)
                return false;
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard != null && LastChainCard.IsCode(MonsterMassRemoval))
            {
                if (Bot.HasInMonstersZone(CardId.ABCDragonBuster))
                {
                    AI.SelectCard(CardId.ApollousaBOG);
                    AI.SelectMaterials(materials1);
                    return true;
                }
                if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials1)) >= 2 && !Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !ABCUnionSummonUsed)
                {
                    AI.SelectCard(CardId.CrusadiaAvramax);
                    AI.SelectMaterials(materials2);
                    return true;
                }
                return false;
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials1)) >= 3 && ABCUnionSummonUsed)
            {
                AI.SelectCard(CardId.ApollousaBOG);
                AI.SelectMaterials(materials1);
                return true;
            }
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials1)) >= 2 && !Bot.HasInMonstersZone(CardId.ABCDragonBuster) && !ABCUnionSummonUsed)
            {
                AI.SelectCard(CardId.CrusadiaAvramax);
                AI.SelectMaterials(materials2);
                return true;
            }
            return false;
        }

        private bool CyberDragonNovaSummon()
        {
            int[] materials = new[] {
                CardId.GalaxySoldier,
                CardId.UnionDriver
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                AI.SelectMaterials(materials);
                return true;
            }
            return false;
        }

        private bool CyberDragonInfinitySummon()
        {
            AI.SelectMaterials(CardId.CyberDragonNova);
            return true;
        }

        private bool ABCDragonBusterSummon()
        {
            if (Bot.Graveyard.GetMatchingCardsCount(card => card.IsCode(ABCUnion)) >= 3)
                AI.SelectMaterials(CardLocation.Grave);
            else AI.SelectMaterials(CardLocation.MonsterZone);
            return true;
        }

        //needs to add the ability to not use the effect if there is no way to get the monster back to grave
        private bool AAssaultCoreEffect()
        {
            if (Card.Location != CardLocation.Grave)
                return false;
            if (Bot.HasInMonstersZone(CardId.IPMasquerina) && (!Bot.HasInHand(CardId.GalaxySoldier) || GalaxySoldierUsed))
            {
                if (Bot.Graveyard.GetCardCount(CardId.CCrushWyvern) >= 2)
                {
                    AI.SelectCard(CardId.CCrushWyvern);
                    return true;
                }
                if (Bot.Graveyard.GetCardCount(CardId.BBusterDrake) >= 2)
                {
                    AI.SelectCard(CardId.BBusterDrake);
                    return true;
                }
                if (Bot.Graveyard.GetCardCount(CardId.AAssaultCore) >= 2)
                {
                    AI.SelectCard(CardId.AAssaultCore);
                    return true;
                }
                return false;
            }
            AI.SelectCard(CardId.CCrushWyvern);
            AI.SelectCard(CardId.BBusterDrake);
            return true;
        }

        private bool BBusterDrakeEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.CCrushWyvern))
                    AI.SelectCard(CardId.CCrushWyvern);
                if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.AAssaultCore);
                if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.BBusterDrake);
                return true;
            }
            return false;
        }

        private bool CCrushWyvernEffect()
        {
            if (Card.Location == CardLocation.Grave)
            {
                if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.BBusterDrake) && Bot.HasInHand(CardId.BBusterDrake))
                    AI.SelectCard(CardId.BBusterDrake);
                if (!Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore) && Bot.HasInHand(CardId.AAssaultCore))
                    AI.SelectCard(CardId.AAssaultCore);
                if (Bot.HasInMonstersZoneOrInGraveyard(CardId.BBusterDrake) && Bot.HasInMonstersZoneOrInGraveyard(CardId.AAssaultCore))
                    AI.SelectCard(CardId.CCrushWyvern);
                else AI.SelectCard(ABCUnion);
                return true;
            }
            return false;
        }


        private bool UnionCarrierEffect()
        {
            if (Bot.HasInMonstersZone(CardId.CyberDragonInfinity))
            {
                AI.SelectCard(CardId.CyberDragonInfinity);
                AI.SelectNextCard(CardId.HeavyMechSupportArmor);
                return true;
            }
            if (!UnionDriverUsed || !PhotonOrbitalUsed)
            {
                int[] mats =
                {
                    CardId.AAssaultCore,
                    CardId.BBusterDrake,
                    CardId.CCrushWyvern,
                    CardId.PhotonThrasher,
                };
                AI.SelectCard(CardId.UnionCarrier);
                if (!UnionDriverUsed)
                {
                    AI.SelectNextCard(CardId.UnionDriver);
                    return true;
                }
                if (!PhotonOrbitalUsed)
                {
                    if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(mats)) >= 2 && (Bot.HasInHand(CardId.GalaxySoldier) || GalaxySoldierUsed))
                        return false;
                    AI.SelectNextCard(CardId.PhotonOrbital);
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool PhotonOrbitalEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                if (Bot.HasInHand(CardId.GalaxySoldier))
                    AI.SelectCard(CardId.PhotonVanisher);
                else AI.SelectCard(CardId.GalaxySoldier);
                PhotonOrbitalUsed = true;
                return true;
            }
            return false;
        }

        private bool GalaxySoldierSpSummon()
        {
            int[] ABCUnion = {
                CardId.AAssaultCore,
                CardId.BBusterDrake,
                CardId.CCrushWyvern
            };
            if (GalaxySoldierUsed && !Bot.HasInMonstersZone(CardId.GalaxySoldier))
                return false;
            if (Card.Location == CardLocation.Hand)
            {
                if (Bot.HasInGraveyard(CardId.AAssaultCore) && Bot.HasInGraveyard(CardId.BBusterDrake))
                {
                    AI.SelectCard(CardId.CCrushWyvern);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.BBusterDrake) && Bot.HasInGraveyard(CardId.CCrushWyvern))
                {
                    AI.SelectCard(CardId.AAssaultCore);
                    return true;
                }
                if (Bot.HasInGraveyard(CardId.CCrushWyvern) && Bot.HasInGraveyard(CardId.AAssaultCore))
                {
                    AI.SelectCard(CardId.BBusterDrake);
                    return true;
                }
                if (Bot.HasInHand(ABCUnion) || Bot.HasInHand(CardId.PhotonThrasher))
                {
                    AI.SelectCard(ABCUnion);
                    AI.SelectCard(CardId.PhotonThrasher);
                    return true;
                }
                if (!Bot.HasInHandOrHasInMonstersZone(ABCUnion) && (Bot.Hand.GetMatchingCardsCount(card => card.HasAttribute(CardAttribute.Light)) >=3 || (Bot.Hand.GetMatchingCardsCount(card => card.HasAttribute(CardAttribute.Light)) >= 2 && Bot.HasInMonstersZone(CardId.GalaxySoldier))))
                {
                    return true;
                }
            }
            return false;
        }

        private bool GalaxySoldierEffect()
        {
            if (Card.Location == CardLocation.MonsterZone && !GalaxySoldierUsed)
            {
                AI.SelectCard(CardId.GalaxySoldier);
                GalaxySoldierUsed = true;
                return true;
            }
            return false;
        }

        private bool TrapSet()
        {
            if (Bot.HasInMonstersZone(CardId.ABCDragonBuster) && Bot.GetHandCount() == 1)
                return false;
            if (!Util.IsTurn1OrMain2())
                return false;
            AI.SelectPlace(Zones.z0 + Zones.z1 + Zones.z3 + Zones.z4);
            return true;
        }


        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
    }
}
