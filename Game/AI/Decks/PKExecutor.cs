using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

namespace WindBot.Game.AI.Decks
{
    //WIP
    [Deck("PK", "AI_PK")]
    public class PKExecutor : DefaultExecutor
    {

        public class CardId
        {
            //Decklist made for the October 2021 TCG banlist.
            //Main Deck Monsters
            public const int TourGuide = 10802915;
            public const int Graff = 20758643;
            public const int Cir = 57143342;
            public const int AshBlossom = 14558127;
            public const int Kagemucha = 19353570;
            public const int AncientCloak = 90432163;
            public const int SilentBoots = 36426778;
            public const int StainedGreaves = 88544390;
            public const int RaggedGloves = 63821877;
            public const int TornScales = 25538345;
            public const int Tsuchinoko = 99745551;
            public const int Jackalope = 43694650;
            public const int Wheeleder = 3233859;
            public const int Tracker = 30227494;
            public const int Nibiru = 27204311;
            public const int PrimalBeing = 27204312;
            public const int RedEyesDragon = 74677425;
            public const int DarkMagician = 6172122;
            //Extra Deck
            public const int Cherubini = 58699500;
            public const int IPMasq = 65741786;
            public const int RustyBardiche = 26692769;
            public const int Unicorn = 38342335;
            public const int Apollousa = 4280259; 
            public const int Accesscode = 86066372;
            public const int Borrelsword = 85289965; //Replaced for Accesscode.
            public const int Leviair = 95992081;
            public const int BreakSword = 62709239;
            public const int EvilswarmNightmare = 359563;
            public const int Redoer = 55285840; //Not used. Replaced by Downerd.
            public const int Downerd = 72167543;
            public const int Zeus = 90448279;
            public const int Isolde = 59934749; //Not used. Cut for Dragoon suite.
            public const int Dante = 83531441; //Not used. Also cut for Dragoon suite.
            public const int Anaconda = 70369116; 
            public const int Dragoon = 37818794;
            //Spells
            public const int CalledBy = 24224830;
            public const int ReinforcementOfTheArmy = 32807846;
            public const int DarkRulerNoMore = 54693926; //Not used. Replaced by Forbidden Chalice.
            public const int FoolishBurial = 81439173;
            public const int PotOfProsperity = 84211599; //Not used. Replaced for more extenders.
            public const int HarpiesFeatherDuster = 18144506; //Not used. Replaced by Imperial Order.
            public const int ForbiddenChalice = 25789292; //Not used.
            public const int Twisters = 43898403; //Not used.
            public const int Designator = 65681983; //Not used.
            public const int RedEyesFusion = 6172122;
            public const int EmergencyTeleport = 67723438;
            //Traps
            public const int Impermanence = 10045474; //Not used. Removed due to Dragoon + Fog Blade + consistency.
            public const int ShadeBrigandine = 98827725;
            public const int FogBlade = 25542642;
            public const int Wing = 98431356;
            public const int ImperialOrder = 61740673; 
            public const int Fragrance = 58921041; //Not used. Replaced for Imperial Order.
            //Fodder
            public const int OjamaToken = 29833092; //In case anyone wants to be a comedian.
            public const int VassalToken = 22404676;
            public const int PrimalBeingToken = 27204312;
        }

        public PKExecutor(GameAI ai, Duel duel)
        : base(ai, duel)
        {
            //Counters
            AddExecutor(ExecutorType.Activate, CardId.FogBlade, FogBladeEffect);
            AddExecutor(ExecutorType.Activate, CardId.Dragoon, DragoonNegate);
            AddExecutor(ExecutorType.Activate, CardId.Apollousa, ApollousaEffect);
            AddExecutor(ExecutorType.Activate, CardId.EvilswarmNightmare);
            AddExecutor(ExecutorType.Activate, CardId.IPMasq, MasqEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Unicorn, UnicornFromIP);
            AddExecutor(ExecutorType.Activate, CardId.Unicorn, UnicornEffect);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.Impermanence, DefaultInfiniteImpermanence);
            AddExecutor(ExecutorType.Activate, CardId.CalledBy, CalledByEffect);
            AddExecutor(ExecutorType.Activate, CardId.ForbiddenChalice, ChaliceEffect);
            AddExecutor(ExecutorType.Activate, CardId.Twisters, TwistersEffect);
            AddExecutor(ExecutorType.Activate, CardId.DarkRulerNoMore, DarkRulerNoMoreEffect);
            AddExecutor(ExecutorType.Activate, CardId.Designator, DesignatorEffect);
            AddExecutor(ExecutorType.Activate, CardId.HarpiesFeatherDuster, DefaultHarpiesFeatherDusterFirst);
            //Burn/Destruction
            AddExecutor(ExecutorType.Activate, CardId.Dragoon, DragoonEffect);
            //Bardiche Setup
            AddExecutor(ExecutorType.Activate, CardId.PotOfProsperity, ProsperityEffect);
            AddExecutor(ExecutorType.Activate, CardId.ReinforcementOfTheArmy, ReinforcementEffect);
            AddExecutor(ExecutorType.Summon, CardId.TourGuide, NormalSummon);
            AddExecutor(ExecutorType.Activate, CardId.Kagemucha, SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.TourGuide, TourGuideEffect);
            AddExecutor(ExecutorType.Summon, CardId.TornScales, NormalSummon);
            AddExecutor(ExecutorType.Activate, CardId.Wheeleder, SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.Tracker, SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.EmergencyTeleport, TeleportEffect);
            AddExecutor(ExecutorType.SpellSet, CardId.ShadeBrigandine);
            AddExecutor(ExecutorType.Activate, CardId.ShadeBrigandine, SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.TornScales, ScalesEffect);
            AddExecutor(ExecutorType.Activate, CardId.AncientCloak, CloakEffect);
            AddExecutor(ExecutorType.Activate, CardId.StainedGreaves, SpecialSummon);
            AddExecutor(ExecutorType.Activate, CardId.SilentBoots, BootsEffect);
            AddExecutor(ExecutorType.Activate, CardId.Graff, GraffEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Cherubini, CherubiniSummon);
            AddExecutor(ExecutorType.Summon, CardId.AncientCloak, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.StainedGreaves, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.Graff, BASummon);
            AddExecutor(ExecutorType.Summon, CardId.Cir, BASummon);
            AddExecutor(ExecutorType.Activate, CardId.Cherubini, CherubiniEffect);
            AddExecutor(ExecutorType.Activate, CardId.Cir, CirEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.RustyBardiche, BardicheSummon);
            //Apollousa Setup
            AddExecutor(ExecutorType.Activate, CardId.Tsuchinoko, DangerSummon);
            AddExecutor(ExecutorType.Activate, CardId.Jackalope, DangerSummon);
            AddExecutor(ExecutorType.Activate, CardId.FoolishBurial, BurialEffect);
            AddExecutor(ExecutorType.Activate, CardId.RaggedGloves, GlovesEffect);
            AddExecutor(ExecutorType.Activate, CardId.RustyBardiche, BardicheEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.SilentBoots, BootsSummon);
            AddExecutor(ExecutorType.Activate, CardId.StainedGreaves, GreavesEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Leviair, LeviairSummon);
            AddExecutor(ExecutorType.Activate, CardId.Leviair, LeviairEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Apollousa, ApollousaSummon);
            //Dragoon Setup
            AddExecutor(ExecutorType.SpSummon, CardId.Anaconda, AnacondaSummon);
            AddExecutor(ExecutorType.Activate, CardId.Anaconda, AnacondaEffect);
            AddExecutor(ExecutorType.Activate, CardId.RedEyesFusion, DragoonSummon);
            //Turn 3+
            AddExecutor(ExecutorType.SpSummon, CardId.IPMasq, IPMasqSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.BreakSword, SwordSummon);
            AddExecutor(ExecutorType.Activate, CardId.BreakSword, SwordEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Isolde, IsoldeSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Unicorn, UnicornSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Accesscode, AccessSummon);
            AddExecutor(ExecutorType.Activate, CardId.Accesscode, AccessEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.EvilswarmNightmare, NightmareSummon);
            AddExecutor(ExecutorType.Activate, CardId.Wing, WingEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Dante, DanteSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Downerd, DownerdSummon);
            AddExecutor(ExecutorType.SpSummon, CardId.Zeus, ZeusSummon);
            AddExecutor(ExecutorType.Activate, CardId.Zeus, ZeusEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.Borrelsword, BorrelSummon);
            AddExecutor(ExecutorType.Activate, CardId.Borrelsword, BorrelEffect);
            AddExecutor(ExecutorType.Summon, CardId.SilentBoots, NormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.Kagemucha, KageSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.Wheeleder, PsychicSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.Tracker, PsychicSummon);
            AddExecutor(ExecutorType.Summon, CardId.AshBlossom, AshSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.DarkMagician, DoNotSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.RedEyesDragon, DoNotSummon);
            AddExecutor(ExecutorType.Activate, CardId.Dante, DanteEffect);
            //Floodgates
            AddExecutor(ExecutorType.Activate, CardId.ImperialOrder, ImperialEffect);
            AddExecutor(ExecutorType.Activate, CardId.Fragrance, FragranceEffect);
            //Failsafes
            AddExecutor(ExecutorType.SpellSet);
            AddExecutor(ExecutorType.Repos, DefaultMonsterRepos);
            /* AddExecutor(ExecutorType.MonsterSet, CardId.TornScales, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.AncientCloak, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.StainedGreaves, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.RaggedGloves, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Kagemucha, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Tracker, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Wheeleder, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.TourGuide, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Cir, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Graff, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Tsuchinoko, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Jackalope, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.AshBlossom, NormalSet);
            Want to add something to check if there's no combo that can be made and if all monsters are better. This is only as a last resort to try to keep LP up to try to last 1 more turn. */
        }

        private bool MaxxCUsed = false;
        private bool BootsUsed = false;
        private bool BorrelswordUsed = false;
        private bool DragoonSummoned = false;

        /*
        public bool MaxxC()
        {
            if ((Util.ChainContainsCard(23434538))) //Maxx "C"
                return MaxxCUsed = true;
            else return MaxxCUsed = false;
            //Need to set up so it only activates when Maxx C is not negated.
        }*/

        public override void OnNewTurn()
        {
           MaxxCUsed = false;
           BootsUsed = false;
            BorrelswordUsed = false;
            DragoonSummoned = false;
        }

        private bool NormalSummon()
        {
            return true;
        }
        private bool NormalSet()
        {
            return !(NormalSummon());
        }
        private bool BootsSummon()
        {
            BootsUsed = true;
            return true;
        }
        public override bool OnSelectHand()
        {
            // Going first deck.
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable)
        {
            // select cards
            return null;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (cardData.Attack <= 1000)
                    return CardPosition.FaceUpDefence;
            }
            return 0;
        }

        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available)
        {
            if (location == CardLocation.MonsterZone)
            {
                if (Card.HasAttribute(CardAttribute.Dark) && Card.HasType(CardType.Xyz))
                {
                    ClientCard l = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.RustyBardiche);
                    int zones = (l?.GetLinkedZones() ?? 0) & available;
                    if ((zones & Zones.z4) > 0)
                        return Zones.z4;
                    if ((zones & Zones.z3) > 0)
                        return Zones.z3;
                    if ((zones & Zones.z2) > 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) > 0)
                        return Zones.z1;
                    if ((zones & Zones.z0) > 0)
                        return Zones.z0;
                }
                if (!Card.HasAttribute(CardAttribute.Dark) || !Card.HasType(CardType.Link))
                {
                    ClientCard l = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.RustyBardiche);
                    int zones = (l?.GetLinkedZones() ?? 0) & available;
                    if ((zones & Zones.z4) < 0)
                        return Zones.z4;
                    if ((zones & Zones.z3) < 0)
                        return Zones.z3;
                    if ((zones & Zones.z2) < 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) < 0)
                        return Zones.z1;
                    if ((zones & Zones.z0) < 0)
                        return Zones.z0;
                }
                if (Card.IsCode(CardId.Cir) || Card.IsCode(CardId.Graff))
                {
                    ClientCard l = Bot.MonsterZone.GetFirstMatchingCard(card => card.Id == CardId.Cherubini);
                    int zones = (l?.GetLinkedZones() ?? 0) & available;
                    if ((zones & Zones.z4) > 0)
                        return Zones.z4;
                    if ((zones & Zones.z3) > 0)
                        return Zones.z3;
                    if ((zones & Zones.z2) > 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) > 0)
                        return Zones.z1;
                    if ((zones & Zones.z0) > 0)
                        return Zones.z0;
                }
                if (Enemy.HasInMonstersZone(5821478)) //Topologic Bomber Dragon
                {
                    ClientCard l = Enemy.MonsterZone.GetFirstMatchingCard(card => card.Id == 5821478);
                    int zones = (l?.GetLinkedZones() ?? 0) & available;
                    if ((zones & Zones.z4) < 0)
                        return Zones.z4;
                    if ((zones & Zones.z3) < 0)
                        return Zones.z3;
                    if ((zones & Zones.z2) < 0)
                        return Zones.z2;
                    if ((zones & Zones.z1) < 0)
                        return Zones.z1;
                    if ((zones & Zones.z0) < 0)
                        return Zones.z0;
                }
            }
            return 0;
        }

        public override bool OnPreBattleBetween(ClientCard attacker, ClientCard defender)
        {
            int[] DoNotAttack =
            {
                63845230, //Eater of Millions
                98434877, //Suijin
                62340868, //Kazejin
                25955164, //Sanga of the Thunder
                72427512, //Mimicking Man Eater Bug
                7572887, //D.D. Warrior Lady
            };
            int[] Blacklist =
{
                72283691, //Golden Castle Stromberg
                39537362, //Ordeal of a Traveler
            };
            if (Enemy.HasInSpellZone(Blacklist))
            {
                return false;
            }
            else if ((Enemy.HasInMonstersZone(29552709)) && (defender.HasSetcode(0x10))) //Daigusto Sphreez
            {
                return false;
            }
            else if (defender.IsCode(DoNotAttack))
            {
                return false;
            }
            else if (defender.HasSetcode(0xd1)) //Graydles
            {
                return false;
            };
            if (attacker.IsCode(CardId.Borrelsword) && !attacker.IsDisabled() && BorrelswordUsed == false)
            {
                attacker.RealPower = attacker.RealPower + defender.GetDefensePower() / 2;
                defender.RealPower = defender.RealPower - defender.GetDefensePower() / 2;
            }
            return base.OnPreBattleBetween(attacker, defender);
        } //Yes, I'm aware this is a very cumbersome and inefficient way to code this. I intend to fix it later.

        private bool TourGuideEffect()
        {
            if (MaxxCUsed == true) return false;
            AI.SelectCard(new[] {
                CardId.Graff,
                CardId.Cir,
                CardId.TourGuide
        });
            return true;
        }
        private bool DoNotSummon()
        {
            return false;
        }
        private bool BASummon()
        {
            if (Bot.GetHandCount() == 1 && Bot.GetMonsterCount() == 0) return true;
            if (Bot.HasInMonstersZone(CardId.Cherubini)) return true;
            return false;
        }
        private bool DangerSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Bot.GetHandCount() == 1) return false;
            return true;
        }
        private bool KageSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Bot.HasInHand(CardId.Wheeleder) || Bot.HasInHand(CardId.Tracker)) return true;
            else return false;
        }
        private bool AshSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Bot.HasInHand(CardId.Wheeleder) || Bot.HasInHand(CardId.Tracker) || Bot.HasInHand(CardId.Kagemucha)) return true;
            else return false;
        }
        private bool PsychicSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Bot.HasInHand(CardId.Kagemucha)) return true;
            else return false;
        }
        private bool GraffEffect()
        {
            if (MaxxCUsed == true) return false;
            if (Card.Location == CardLocation.Hand) return false;
            AI.SelectCard(CardId.Cir);
            return true;
        }
        private bool CirEffect()
        {
            if (MaxxCUsed == true) return false;
            if (Card.Location == CardLocation.Hand) return false;
            AI.SelectCard(new[] {
                CardId.Cherubini,
                CardId.Graff
            });
            return true;
        }
        private bool CherubiniEffect()
        {
            AI.SelectCard(new[] {
                CardId.Graff,
                CardId.AncientCloak,
                CardId.RaggedGloves,
                CardId.TornScales,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Cir,
            });
            return true;
        }
        private bool TeleportEffect()
        {
            AI.SelectCard(new[] {
                CardId.Wheeleder,
                CardId.Tracker,
            });
            return true;
        }
        private bool SpecialSummon()
        {
            if (MaxxCUsed == true) return false;
            return true;
        }
        private bool GreavesEffect()
        {
            return true;
        }
        private bool LeviairEffect()
        {
            if (MaxxCUsed == true) return false;
            AI.SelectCard(new[] {
                CardId.TornScales,
                CardId.RaggedGloves,
                CardId.AncientCloak,
                CardId.SilentBoots,
                CardId.StainedGreaves,
            });
            return true;
        }
        private bool DanteEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Dante, 0))
                return false;
            else
                AI.SelectCard(new[] {
                CardId.Cir,
                CardId.Graff,
                CardId.Cherubini,
            });
            return true;
        }
        private bool BardicheSummon()
        {
            if (MaxxCUsed == true) return false;
            int[] materials = new[] {
                CardId.Cir,
                CardId.Graff,
                CardId.Cherubini,
                CardId.IPMasq,
                CardId.TornScales,
                CardId.Wheeleder,
                CardId.Tracker,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.Isolde,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.ShadeBrigandine,
                CardId.Unicorn,
                CardId.Leviair
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

        private bool BardicheEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RustyBardiche, 0))
            {
                if (Bot.HasInGraveyard(CardId.TornScales))
                    AI.SelectCard(new[] {
                CardId.RaggedGloves,
                CardId.AncientCloak,
                CardId.SilentBoots,
                CardId.StainedGreaves,
            });
                else AI.SelectCard(CardId.TornScales);
                ;
                AI.SelectNextCard(new[] {
                CardId.ShadeBrigandine,
                CardId.FogBlade,
                CardId.Wing,
            });
            }
            else
            {
                ClientCard target = null;
                target = Util.GetBestEnemyCard(false, true);
                if (target == null)
                    return false;
                AI.SelectCard(target);
                return true;
            }
            return true;
        }
        private bool ApollousaSummon()
        {
            if (MaxxCUsed == true) return false;
            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.IPMasq,
                CardId.Isolde,
                CardId.Cherubini,
                CardId.Anaconda,
                CardId.Wheeleder,
                CardId.ShadeBrigandine,
                CardId.Dante,
                CardId.BreakSword,
                CardId.TornScales,
                CardId.SilentBoots,
                CardId.RaggedGloves,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Kagemucha,
                CardId.TourGuide,
                CardId.Graff,
                CardId.Cir,
                CardId.Leviair,
                CardId.AshBlossom,
                CardId.EvilswarmNightmare,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.Unicorn,
            };
            if (Enemy.HasInSpellZone(82732705) || Enemy.HasInSpellZone(76375976)) return false; //Skill Drain & Mystic Mine
            if (Bot.GetMonsterCount() == 2 || ((Bot.GetMonsterCount() == 3) && (Bot.HasInMonstersZone(CardId.RustyBardiche)))) return false;
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 2)
            {
                List<ClientCard> materials2 = new List<ClientCard>();
                List<ClientCard> bot_monster = Bot.GetMonsters();
                bot_monster.Sort(CardContainer.CompareCardLevel);
                int link_count = 0;
                foreach (ClientCard card in bot_monster)
                {
                    if (card.IsFacedown()) continue;
                    if (!materials2.Contains(card) && card.LinkCount <= 3 && card.IsCode(materials))
                    {
                        materials2.Add(card);
                        link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                        if (link_count >= 4) break;
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

        private bool SwordSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                if (Enemy.GetFieldCount() == 0) return false;
                int[] materials = new[] {
                    CardId.Tsuchinoko,
                    CardId.Jackalope,
                    CardId.Kagemucha,
                    CardId.TourGuide,
                    CardId.RaggedGloves,
                    CardId.Cir,
                    CardId.AncientCloak,
                    CardId.Graff,
                    CardId.SilentBoots,
                    CardId.Wheeleder,
                    CardId.TornScales,
                    CardId.StainedGreaves,
                    CardId.Tracker,
                };
                AI.SelectMaterials(materials);
                return true;
            }
        }
        private bool UnicornSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Duel.Turn == 1) return false;
            if (Enemy.GetFieldCount() == 0) return false;
            if (Bot.GetHandCount() == 0) return false;
            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.IPMasq,
                CardId.Cherubini,
                CardId.Isolde,
                CardId.Anaconda,
                CardId.TornScales,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Wheeleder,
                CardId.ShadeBrigandine,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tracker,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.Leviair,
                CardId.Dragoon,
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
                        if (link_count >= 3) break;
                    }
                }
                if (link_count < 3)
                {
                    return false;
                }
                AI.SelectMaterials(materials2);
                return true;
            }
            return false;

        }


            private bool UnicornEffect()
        {
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target == null)
                return false;
            else AI.SelectCard(DiscardCard());
            AI.SelectNextCard(target);
            if (Enemy.HasInSpellZone(82732705)) return false; //Skill Drain
            return true;
        }
        private bool SwordEffect()
        {
            if (Card.Location == CardLocation.MonsterZone)
            {
                if (Enemy.GetFieldCount() == 0) return false;
                ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
                if (target == null)
                    return false;
                if (Enemy.HasInSpellZone(82732705)) return false; //Skill Drain
                AI.SelectCard(CardId.BreakSword);
                AI.SelectNextCard(target);
                return true;
            }
            if (Card.Location == CardLocation.Grave)
            {
                {
                    AI.SelectCard(new[] {
                CardId.SilentBoots,
                CardId.AncientCloak,
                CardId.TornScales,
                CardId.StainedGreaves,
                CardId.RaggedGloves,
            });
                }; return true;
            }
            else return false;
        }
        private bool FogBladeEffect()
        {
            if (Card.Location == CardLocation.SpellZone)
            {
                return !Util.HasChainedTrap(0) && DefaultDisableMonster();
            }
            else if (Bot.HasInGraveyard(CardId.RustyBardiche) || Bot.GetMonsterCount() < 2)
            {
                AI.SelectCard(new[] {
                CardId.RustyBardiche,
                CardId.BreakSword,
            });
            }
            return false;
        }
        private bool WingEffect()
        {
            if ((Card.Location == CardLocation.Grave) && (Bot.HasInGraveyard(CardId.RustyBardiche) || Bot.GetMonsterCount() < 2))
            {
                AI.SelectCard(new[] {
                CardId.RustyBardiche,
                CardId.BreakSword,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.AncientCloak,
                CardId.RaggedGloves,
                CardId.TornScales,
            });
            if (Card.Location != CardLocation.Grave)
                {
                    if (Bot.GetFieldCount() == 0) return false;
                    ClientCard target = Util.GetBestBotMonster();
                    if (target == null)
                        return false;
                    AI.SelectCard(target);
                    return true;
                }
            }
            return true;
        }
        private bool ReinforcementEffect()
        {
            AI.SelectCard(new[]{
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.TornScales,
                CardId.AncientCloak,
                CardId.RaggedGloves,
                CardId.Kagemucha,
            });
            return true;
        }
        private bool IPMasqSummon()
        {
            if (MaxxCUsed == true) return false;
            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.TornScales,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) < 2)
            {
                return false;
            }
            if ((Bot.GetFieldHandCount()-Bot.GetSpellCount()) == 2) return false;
            if ((Bot.GetFieldHandCount() - Bot.GetSpellCount()) == 3 && Bot.HasInMonstersZone(CardId.RustyBardiche)) return false;
            AI.SelectMaterials(materials);
            return true;
        }
        private bool MasqEffect()
        {
            int[] materials = new[] {
                CardId.IPMasq,
                CardId.Isolde,
                CardId.Anaconda,
                CardId.TornScales,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
                CardId.Unicorn,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool ChaliceEffect()
        {
            if (Duel.LastChainPlayer == 1)
            {
                var target = Util.GetProblematicEnemyMonster(0, true);
                if (target != null && !target.IsShouldNotBeSpellTrapTarget() && Duel.CurrentChain.Contains(target))
                {
                    AI.SelectCard(target);
                    return true;
                }
            }
            return false;
        }
        private bool DarkRulerNoMoreEffect()
        {
            if (Duel.LastChainPlayer == 1)
            {
                if (Util.GetLastChainCard().Location != CardLocation.MonsterZone)
                    return false;
            }
            return UniqueFaceupSpell();
        }
        private bool DesignatorEffect()
        {
            if (Duel.LastChainPlayer == 1)
            {
                ClientCard LastChainCard = Util.GetLastChainCard();
                if (LastChainCard.IsCode(CardId.Nibiru))
                {
                    AI.SelectAnnounceID(CardId.Nibiru);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.AshBlossom))
                {
                    AI.SelectAnnounceID(CardId.AshBlossom);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.Impermanence))
                {
                    AI.SelectAnnounceID(CardId.Impermanence);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.CalledBy))
                {
                    AI.SelectAnnounceID(CardId.CalledBy);
                    return true;
                }
                if (LastChainCard.IsCode(CardId.DarkRulerNoMore))
                {
                    AI.SelectAnnounceID(CardId.DarkRulerNoMore);
                    return true;
                }
            }
            return false;
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
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return true;
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

        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = true, List<int> avoid_list = null)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence) continue;
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
                    AI.SelectPlace(zone);
                    return;
                };
            }
            AI.SelectPlace(0);
        }
        private bool ProsperityEffect()
        {
            {
                if (SpellNegatable()) return false;
                SelectSTPlace(Card, true);
                AI.SelectNumber(3);
            }
            AI.SelectCard(new[] {
                CardId.Isolde,
                CardId.Dante,
                CardId.Accesscode,
                CardId.Downerd,
                CardId.EvilswarmNightmare,
                CardId.Unicorn,
                CardId.Leviair,
                CardId.IPMasq,
                CardId.Borrelsword,
                CardId.Zeus,
                CardId.Anaconda,
                CardId.Dragoon,
                CardId.Cherubini,
                CardId.BreakSword,
                CardId.Apollousa,
                CardId.RustyBardiche,
            });
            if (Enemy.HasInSpellZone(82732705) || Enemy.HasInSpellZone(76375976)) // Skill Drain | Mystic Mine
                AI.SelectNextCard(new[] {
                CardId.HarpiesFeatherDuster,
                CardId.ImperialOrder,
                CardId.TourGuide,
                CardId.ReinforcementOfTheArmy,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.ShadeBrigandine,
                CardId.Wing,
                CardId.FogBlade,
                CardId.ForbiddenChalice,
                CardId.FoolishBurial,
                CardId.TornScales,
                CardId.AshBlossom,
                CardId.Impermanence,
                CardId.CalledBy,
                CardId.Wheeleder,
                CardId.Jackalope,
                CardId.DarkRulerNoMore,
                CardId.Kagemucha,
                CardId.Graff,
                CardId.Cir,
                CardId.RedEyesFusion,
                CardId.PotOfProsperity,
            });
            else
            AI.SelectNextCard(new[] {
                CardId.TourGuide,
                CardId.ReinforcementOfTheArmy,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.ShadeBrigandine,
                CardId.Wing,
                CardId.FogBlade,
                CardId.ImperialOrder,
                CardId.HarpiesFeatherDuster,
                CardId.ForbiddenChalice,
                CardId.FoolishBurial,
                CardId.TornScales,
                CardId.AshBlossom,
                CardId.Impermanence,
                CardId.CalledBy,
                CardId.Wheeleder,
                CardId.Jackalope,
                CardId.DarkRulerNoMore,
                CardId.Kagemucha,
                CardId.Tracker,
                CardId.Graff,
                CardId.Cir,
                CardId.RedEyesFusion,
                CardId.PotOfProsperity,
            });
            return true;
        }
        private bool ImperialEffect()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().IsCode(CardId.PotOfProsperity))
                return false;
            return DefaultOnBecomeTarget() && Util.GetLastChainCard().HasType(CardType.Spell);
        }

        private bool LeviairSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                int[] materials = new[] {
                    CardId.Tsuchinoko,
                    CardId.Jackalope,
                    CardId.Kagemucha,
                    CardId.TourGuide,
                    CardId.RaggedGloves,
                    CardId.Cir,
                    CardId.AncientCloak,
                    CardId.Graff,
                    CardId.SilentBoots,
                    CardId.Wheeleder,
                    CardId.TornScales,
                    CardId.StainedGreaves,
                    CardId.Tracker,
                };
                AI.SelectMaterials(materials);
                return true;
            }
        }
        private bool BootsEffect()
        {
            if (!Bot.HasInGraveyard(CardId.Impermanence) && !Bot.HasInGraveyard(CardId.Wing) && !Bot.HasInGraveyard(CardId.FogBlade) && !Bot.HasInGraveyard(CardId.ImperialOrder) && !Bot.HasInGraveyard(CardId.Fragrance))
                AI.SelectCard(CardId.ShadeBrigandine);
            else
                AI.SelectCard(CardId.FogBlade);
            return true;
        }
        private bool CloakEffect()
        {
            if (!Bot.HasInGraveyard(CardId.Impermanence) && !Bot.HasInGraveyard(CardId.Wing) && !Bot.HasInGraveyard(CardId.FogBlade) && !Bot.HasInGraveyard(CardId.ImperialOrder) && !Bot.HasInGraveyard(CardId.Fragrance))
                AI.SelectCard(CardId.ShadeBrigandine);
            if (BootsUsed == true)
                AI.SelectCard(CardId.StainedGreaves);
            if (BootsUsed == false)
                AI.SelectCard(CardId.SilentBoots);
            return true;
        }
        public bool BorrelSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Duel.Turn == 1) return false;

            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.IPMasq,
                CardId.Unicorn,
                CardId.Isolde,
                CardId.Anaconda,
                CardId.TornScales,
                CardId.Cherubini,
                CardId.Cir,
                CardId.Graff,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
                CardId.Dante,
            };

            List<ClientCard> material_list = new List<ClientCard>();
            List<ClientCard> bot_monster = Bot.GetMonsters();
            bot_monster.Sort(CardContainer.CompareCardAttack);
            //bot_monster.Reverse();
            int link_count = 0;
            foreach (ClientCard card in bot_monster)
            {
                if (card.IsFacedown()) continue;
                if (!material_list.Contains(card) && card.LinkCount < 3)
                {
                    material_list.Add(card);
                    link_count += (card.HasType(CardType.Link)) ? card.LinkCount : 1;
                }
            }
            AI.SelectMaterials(materials);
            return true;
        }

        public bool BorrelEffect()
        {
            if (ActivateDescription == -1) return true;
            else if ((Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2) || Util.IsChainTarget(Card))
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard card in enemyMonsters)
                {
                    if (!card.HasPosition(CardPosition.Attack) || card.HasType(CardType.Link))
                    {
                        return false;
                    }
                    else AI.SelectCard(card);
                }
                List<ClientCard> bot_list = Bot.GetMonsters();
                bot_list.Sort(CardContainer.CompareCardAttack);
                //bot_list.Reverse();
                foreach (ClientCard card in bot_list)
                {
                    if (!card.HasPosition(CardPosition.Attack) || card.HasType(CardType.Link))
                    {
                        return false;
                    }
                    else AI.SelectCard(card);
                }
            }
            return true;
        }
        private bool DanteSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                if (!Bot.HasInExtra(CardId.Zeus)) return false;
                int[] materials = new[] {
                    CardId.Tsuchinoko,
                    CardId.Jackalope,
                    CardId.Kagemucha,
                    CardId.TourGuide,
                    CardId.RaggedGloves,
                    CardId.Cir,
                    CardId.AncientCloak,
                    CardId.Graff,
                    CardId.SilentBoots,
                    CardId.Wheeleder,
                    CardId.TornScales,
                    CardId.StainedGreaves,
                    CardId.Tracker,
                };
                AI.SelectMaterials(materials);
                return true;
            }
        }
        private bool DownerdSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                if (!Bot.HasInExtra(CardId.Zeus)) return false;
                int[] materials = new[] {
                    CardId.Dante,
                    CardId.Leviair,
                    CardId.BreakSword,
                };
                AI.SelectMaterials(materials);
                return true;
            }
        }
        private bool ZeusSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                int[] materials = new[] {
                    CardId.Downerd,
                    CardId.EvilswarmNightmare,
                };
                AI.SelectMaterials(materials);
                return true;
            }
        }
        private bool ZeusEffect()
        {
            return ((Util.IsOneEnemyBetter()) || (Bot.GetFieldCount() <= Enemy.GetFieldCount()));
        }

        private bool BurialEffect()
        {
            if (!Bot.HasInGraveyard(CardId.TornScales))
                AI.SelectCard(CardId.TornScales);
            else
                AI.SelectCard(new[] {
                CardId.RaggedGloves,
                CardId.AncientCloak,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.TornScales,
                CardId.Graff,
                CardId.Cir,
                CardId.Wheeleder,
                CardId.Tracker,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.DarkMagician,
                CardId.RedEyesDragon,
                CardId.AshBlossom,
            });
            return true;
        }
        private bool GlovesEffect()
        {
            if (Bot.HasInGraveyard(CardId.TornScales))
                AI.SelectCard(new[] {
                CardId.Wing,
                CardId.FogBlade,
                CardId.AncientCloak,
                CardId.SilentBoots,
                CardId.TornScales,
                CardId.StainedGreaves,
            });
            else AI.SelectCard(CardId.TornScales);
            ;
            return true;
        }
        private bool ScalesEffect()
        {
            if (Card.Location == CardLocation.Grave)
                if (MaxxCUsed == true) return false;
                else return true;
            else
                if (Enemy.HasInSpellZone(82732705) || Enemy.HasInSpellZone(76375976)) // Mystic Mine & Skill Drain
                AI.SelectCard(DiscardCard());
            AI.SelectNextCard(CardId.AncientCloak);
            return true;
        }
        public bool AccessSummon()
        {
            if (MaxxCUsed == true) return false;
            if (Duel.Phase != DuelPhase.Main1) return false;
            if (Duel.Turn == 1) return false;

            int[] materials = new[] {
                CardId.Apollousa,
                CardId.Unicorn,
                CardId.IPMasq,
                CardId.Anaconda,
                CardId.Isolde,
                CardId.Cherubini,
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.ShadeBrigandine,
                CardId.TornScales,
                CardId.Cherubini,
                CardId.Cir,
                CardId.Graff,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
            };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) >= 3)
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
        public bool AccessEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Accesscode, 2))
            {
                ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
                if (target == null)
                    return false;
                else AI.SelectCard(Bot.GetGraveyardMonsters());
                AI.SelectNextCard(target);
                return true;
            }
            else
            {
                AI.SelectCard(new[]{
                CardId.Borrelsword,
                CardId.Apollousa,
                CardId.Unicorn,
                CardId.Isolde,
                CardId.Cherubini,
                CardId.Anaconda,
                CardId.IPMasq,
            });
                return true;
            }
            // return false; //Banishing cards from the GY first now, but still banishing itself.
        }
        private bool IsoldeSummon()
        {
            if (MaxxCUsed == true) return false;
            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.TornScales,
                CardId.Cherubini,
                CardId.Cir,
                CardId.Graff,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
                CardId.IPMasq,
        };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }
        private bool FusionRequirements()
        {
            if (!Bot.HasInExtra(CardId.Dragoon)) return false;
            if (Bot.HasInGraveyard(CardId.RedEyesFusion)) return false;
            if (Bot.HasInGraveyard(CardId.DarkMagician)) return false;
            if (Bot.HasInGraveyard(CardId.RedEyesDragon)) return false;
            if (Bot.HasInBanished(CardId.RedEyesFusion)) return false;
            if (Bot.HasInBanished(CardId.DarkMagician)) return false;
            if (Bot.HasInBanished(CardId.RedEyesDragon)) return false;
            return true;
        }
        private bool AnacondaSummon()
        {
            if (MaxxCUsed == true) return false;
            if (FusionRequirements() == false) return false;
            if (Bot.HasInSpellZone(CardId.RedEyesFusion)) return false;
            int[] materials = new[] {
                CardId.OjamaToken,
                CardId.VassalToken,
                CardId.PrimalBeingToken,
                CardId.TornScales,
                CardId.Cir,
                CardId.Graff,
                CardId.AshBlossom,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.AncientCloak,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tracker,
                CardId.Leviair,
                CardId.Isolde,
                CardId.IPMasq,
        };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }

        private bool AnacondaEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.Anaconda, 0))
                return false;
            AI.SelectCard(CardId.RedEyesFusion);
            AI.SelectMaterials(CardLocation.Deck);
            DragoonSummoned = true;
            return true;
        }

        private bool DragoonSummon()
        {
            AI.SelectMaterials(CardLocation.Deck);
            DragoonSummoned = true;
            return true;
        }

        private bool ApollousaEffect()
        {
            int[] Blacklist =
            {
                64734921,   //The Agent of Creation - Venus
                34408491,   //Beelze of the Diabolic Dragons
                8763963,    //Beelzeus of the Diabolic Dragons
                74586817,   //PSY-Framelord Omega
                29353756,   //ZW - Eagle Claw
            };
            int[] HandMZone =
            {
                93969023,   //Black Metal Dragon
                81471108,   //ZW - Tornado Bringer
                45082499,   //ZW - Lightning Blade
                2648201,    //ZW - Sleipnir Mail
                40941889,   //ZW - Asura Strike
                6330307,    //DZW - Chimera Clad
                14235211,   //Rider of the Storm Winds
                38210374,   //Explossum
                38601126,   //Robot Buster Destruction Sword
                2602411,    //Wizard Buster Destruction Sword
                76218313,   //Dragon Buster Destruction Sword
            };
            int[] Hand =
            {
                94573223,   //Inzektor Giga-Mantis
                21977828,   //Inzektor Giga-Weevil
                89132148,   //Photon Orbital
                76080032,   //ZW - Unicorn Spear
                87008374,   //ZW - Phoenix Bow
                12927849,   //SZW - Fenrir Sword
            };
            ClientCard LastChainCard = Util.GetLastChainCard();
            if (LastChainCard.IsCode(57774843) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return false;   //Judgment Dragon
            if (LastChainCard.IsCode(423585) && ActivateDescription == Util.GetStringId(423585, 0))
                return false;   //Summoner Monk
            if (LastChainCard.IsCode(43218406) && (LastChainCard.Location == CardLocation.MonsterZone))
                return false;   //Water Gizmek
            if (LastChainCard.HasSetcode(0x11e) && (LastChainCard.Location == CardLocation.Hand))
                return false;   //Danger!
            if (LastChainCard.HasSetcode(0x109a) && (LastChainCard.Location == CardLocation.Hand || LastChainCard.Location == CardLocation.MonsterZone) && (Duel.Phase == DuelPhase.Main1 || Duel.Phase == DuelPhase.Main2))
                return false;   //Superheavy Samurai Soul
            if (LastChainCard.IsCode(HandMZone) && (LastChainCard.Location == CardLocation.Hand || LastChainCard.Location == CardLocation.MonsterZone))
                return false;   //Equip effects from hand or monster zone
            if (LastChainCard.IsCode(Hand) && LastChainCard.Location == CardLocation.Hand)
                return false;   //Equip effects from hand
            if (LastChainCard.IsCode(Blacklist))
                return false;
            return true;
        }
        private bool NightmareSummon()
        {
            {
                if (MaxxCUsed == true) return false;
                AI.SelectPosition(CardPosition.FaceUpDefence);
                AI.SelectCard(new[] {
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.TornScales,
                CardId.AncientCloak,
                CardId.RaggedGloves,
                CardId.ShadeBrigandine,
                });
                return true;
            }
        }
        private bool FragranceEffect()
        {

            int spell_count = 0;
            foreach (ClientCard check in Bot.Hand)
            {
                if (check.HasType(CardType.Spell))
                    spell_count++;
            }
            if (spell_count >= 2) return false;
            return Duel.Player == 1 && UniqueFaceupSpell();
        }

        private bool TwistersEffect()
        {
            if (Util.ChainContainsCard(CardId.Twisters))
                return false;
            IList<ClientCard> targets = new List<ClientCard>();
            foreach (ClientCard target in Enemy.GetSpells())
            {
                if (target.IsFloodgate())
                    targets.Add(target);
                if (targets.Count >= 2)
                    break;
            }
            if (targets.Count < 2)
            {
                foreach (ClientCard target in Enemy.GetSpells())
                {
                    if (target.IsFacedown() || target.HasType(CardType.Continuous) || target.HasType(CardType.Pendulum))
                        targets.Add(target);
                    if (targets.Count >= 2)
                        break;
                }
            }
            if (targets.Count > 0)
            {
                AI.SelectCard(DiscardCard());
                AI.SelectNextCard(targets);
                return true;
            }
            return false;
        }
        private bool DragoonEffect()
        {
            if (ActivateDescription == -1 || ActivateDescription == Util.GetStringId(CardId.Dragoon, 1))
                return false;
            AI.SelectCard(Util.GetBestEnemyMonster());
            return true;
        }
        private bool DragoonNegate()
        {
            if (ActivateDescription != -1 && ActivateDescription != Util.GetStringId(CardId.Dragoon, 1))
                return false;
            if (Duel.LastChainPlayer != 1)
                return false;
            AI.SelectCard(DiscardCard());
            return true;
        }
        private int DiscardCard()
        {
            if (Bot.HasInHand(CardId.Cir) && Bot.HasInGraveyard(CardId.Cherubini) && (DragoonSummoned = false))
                return CardId.Cir;
            if (Bot.HasInHand(CardId.EmergencyTeleport) && Bot.HasInGraveyard(CardId.Tracker) && Bot.HasInGraveyard(CardId.Wheeleder))
                return CardId.EmergencyTeleport;
            if (Bot.HasInHand(CardId.RedEyesDragon) && (FusionRequirements() == false))
                return CardId.RedEyesDragon;
            if (Bot.HasInGraveyard(CardId.Wheeleder) && Bot.HasInGraveyard(CardId.Tracker))
                return CardId.EmergencyTeleport;
            if (Bot.HasInHand(CardId.DarkMagician) && (FusionRequirements() == false))
                return CardId.DarkMagician;
            if (Bot.HasInHand(CardId.RedEyesFusion) && (FusionRequirements() == false))
                return CardId.RedEyesFusion;
            if (Bot.HasInHand(CardId.RaggedGloves))
                return CardId.RaggedGloves;
            if (Bot.HasInHand(CardId.TornScales) && !Bot.HasInGraveyard(CardId.TornScales))
                return CardId.TornScales;
            if (Bot.HasInHand(CardId.AncientCloak))
                return CardId.AncientCloak;
            if (Bot.HasInHand(CardId.Tsuchinoko))
                return CardId.Tsuchinoko;
            if (Bot.HasInHand(CardId.Jackalope))
                return CardId.Jackalope;
            if (Bot.HasInHand(CardId.Wing))
                return CardId.Wing;
            if (Bot.HasInHand(CardId.DarkMagician))
                return CardId.DarkMagician;
            if (Bot.HasInHand(CardId.RedEyesDragon))
                return CardId.RedEyesDragon;
            if (Bot.HasInHand(CardId.Wheeleder))
                return CardId.Wheeleder;
            if (Bot.HasInHand(CardId.Tracker))
                return CardId.Tracker;
            if (Bot.HasInHand(CardId.Kagemucha))
                return CardId.Kagemucha;
            if (Bot.HasInHand(CardId.StainedGreaves))
                return CardId.StainedGreaves;
            if (Bot.HasInHand(CardId.SilentBoots))
                return CardId.SilentBoots;
            if (Bot.HasInHand(CardId.FogBlade))
                return CardId.FogBlade;
            return 0;
        }
        private bool UnicornFromIP()
        {
            if (Duel.Player == 0) return false;
            if (Enemy.GetFieldCount() == 0) return false;
            if (Bot.GetHandCount() == 0) return false;
            else return UnicornSummon();
        }
        private bool CherubiniSummon()
        {
            if (MaxxCUsed == true) return false;
            int[] materials = new[] {
                CardId.Cir,
                CardId.Graff,
                CardId.TornScales,
                CardId.AncientCloak,
                CardId.RaggedGloves,
                CardId.SilentBoots,
                CardId.StainedGreaves,
                CardId.Tsuchinoko,
                CardId.Jackalope,
                CardId.AshBlossom,
                CardId.Wheeleder,
                CardId.TourGuide,
                CardId.Kagemucha,
                CardId.Tracker,
        };
            if (Bot.MonsterZone.GetMatchingCardsCount(card => card.IsCode(materials)) < 2)
            {
                return false;
            }
            AI.SelectMaterials(materials);
            return true;
        }
        public bool CalledByEffect()
        {
            if (!DefaultUniqueTrap())
                return false;

            if (Duel.Player == 1)
            {
                ClientCard target = Enemy.MonsterZone.GetShouldBeDisabledBeforeItUseEffectMonster();
                if (target != null && Enemy.HasInGraveyard(target.Id))
                {
                    AI.SelectCard(target.Id);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            if (LastChainCard != null
                && LastChainCard.Controller == 1
                && (LastChainCard.Location == CardLocation.Grave
                || LastChainCard.Location == CardLocation.Hand
                || LastChainCard.Location == CardLocation.MonsterZone
                || LastChainCard.Location == CardLocation.Removed)
                && !LastChainCard.IsDisabled() && !LastChainCard.IsShouldNotBeTarget()
                && !LastChainCard.IsShouldNotBeSpellTrapTarget()
                && Enemy.HasInGraveyard(LastChainCard.Id))
            {
                AI.SelectCard(LastChainCard.Id);
                return true;
            }

            if (Bot.BattlingMonster != null && Enemy.BattlingMonster != null)
            {
                if (!Enemy.BattlingMonster.IsDisabled() && Enemy.BattlingMonster.IsCode(_CardId.EaterOfMillions) && Enemy.HasInGraveyard(_CardId.EaterOfMillions))
                {
                    AI.SelectCard(Enemy.BattlingMonster.Id);
                    return true;
                }
            }

            if (Duel.Phase == DuelPhase.BattleStart && Duel.Player == 1 &&
                Enemy.HasInMonstersZone(_CardId.NumberS39UtopiaTheLightning, true) && Enemy.HasInGraveyard(_CardId.NumberS39UtopiaTheLightning))
            {
                AI.SelectCard(_CardId.NumberS39UtopiaTheLightning);
                return true;
            }

            return false;
        }
    }

}
