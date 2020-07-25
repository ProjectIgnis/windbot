using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;


namespace WindBot.Game.AI.Decks
{
    [Deck("ZefraControl", "AI_ZefraControl")]
    class ZefraControlExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int PsyFrameDriver = 49036338;
            public const int Zefraath = 29432356;
            public const int GizmekKaku = 63633694;
            public const int Zefraniu = 58990362;
            public const int Zefraxciton = 22617205;
            public const int FairyTailLuna = 86937530;
            public const int Zefrathuban = 96223501;
            public const int Zefrawendi = 23166823;
            public const int Zefraxi = 21495657;
            public const int Zeframpilica = 57777714;
            public const int AshBlossom = 14558127;
            public const int GhostOgre = 59438930;
            public const int PsyFrameGamma = 38814750;
            public const int EffectVeiler = 97268402;
            public const int Terraforming = 73628505;
            public const int ZefraProvidence = 74580251;
            public const int ZefraPath = 5255013;
            public const int OracleZefra = 32354768;
            public const int ZefraWar = 96073342;
            public const int ZefraDivineStrike = 35561352;
            public const int NinePilalrs = 57831349;

            public const int Crocodragon = 87188910;
            public const int IgnisterProminence = 18239909;
            public const int QuantumDragon = 63533837;
            public const int WindPegasus = 98506199;
            public const int MetaphysHorus = 36898537;
            public const int StardustChargeWarrior = 64880894;
            public const int EarthSlicer = 97584719;
            public const int Enterblathnir = 95113856;
            public const int PtolemyM7 = 38495396;
            public const int PhotonStrike = 92661479;
            public const int Sheridan = 32302078;
            public const int VampiricDragon = 93713837;
            public const int TrueKingVFD = 88581108;
            public const int DracoMasterOfTenyi = 23935886;
            public const int PsyframelordLambda = 8802510;
        }
        private readonly int[] LowScaleNoZeraxi =
        {
            //CardId.Zefraxi,
            CardId.Zefrathuban,
            CardId.Zeframpilica
        };
        private readonly int[] HighScaleNoZeraniu =
        {
            //CardId.Zefraxi,
            CardId.Zefraxciton,
            CardId.Zefrawendi,
        };
        private readonly int[] LowScale =
        {
            CardId.Zefraxi,
            CardId.Zefrathuban,
            CardId.Zeframpilica
        };
        private readonly int[] HighScale=
        {
            CardId.Zefraniu,
            CardId.Zefraxciton,
            CardId.Zefrawendi,
        };
        private readonly int[] BackRowSearchPriority = {
            CardId.ZefraProvidence,
            CardId.ZefraDivineStrike,
            CardId.ZefraWar,
        };
        private bool OracleActivated = false;
        private bool NormalSummonUsed = false;
        private bool ProvidenceActivated = false;
        private bool PendulumSummoned = false;

        public override void OnNewTurn() {
            OracleActivated = false;
            NormalSummonUsed = false;
            ProvidenceActivated = false;
            PendulumSummoned = false;
        }
        public ZefraControlExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            
            // counter
            AddExecutor(ExecutorType.Activate, CardId.PsyFrameGamma, GammaEffect);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.ZefraDivineStrike, ActivateZefraDivineStrike);

            AddExecutor(ExecutorType.Activate, CardId.TrueKingVFD, TrueKingStun);

            //search field spell
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZefraProvidence, ZefraProvidence);

            //set trap except war
            AddExecutor(ExecutorType.SpellSet, CardId.ZefraDivineStrike, SetDivineStrike);

            // try to complete our scale
            AddExecutor(ExecutorType.Activate, CardId.OracleZefra, OracleActivate);
            AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathActivate);
            AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathDump);
            AddExecutor(ExecutorType.Activate, CardId.Zefraniu, ZefraniuSearch);
            AddExecutor(ExecutorType.Activate, CardId.Crocodragon,CrocoDragonTrigger);
            AddExecutor(ExecutorType.Activate, CardId.OracleZefra,OracleTrigger);

            //special summon kaku
            AddExecutor(ExecutorType.Activate, CardId.GizmekKaku,GizmekKakuSummon);

            // putting low scale prioritize non zefraxi card
            AddExecutor(ExecutorType.Activate, CardId.Zefrathuban);
            AddExecutor(ExecutorType.Activate, CardId.Zefraxi,ZefraxiToTuner);
            AddExecutor(ExecutorType.SpSummon, CardId.Crocodragon, SynchroCrocodragon);
            AddExecutor(ExecutorType.SpSummon, CardId.Enterblathnir, SummonEnteblethnir);
            AddExecutor(ExecutorType.SpSummon, CardId.PsyframelordLambda, LinkSummonLambda);
            AddExecutor(ExecutorType.SpSummon, CardId.TrueKingVFD, VFDSummon);
            AddExecutor(ExecutorType.SpSummon, CheckPendulumSummon);
            //AddExecutor(ExecutorType.SpSummon,CardId.Crocodragon,SynchroCrocodragon);

            //AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathActivate);

        }

        private bool LinkSummonLambda() {
            if (Bot.HasInMonstersZone(CardId.PsyFrameGamma) && Bot.HasInMonstersZone(CardId.PsyFrameDriver)) {
                return true;
            }
            return false;
        }

        private bool CheckPendulumSummon() {
            if (!PendulumSummoned) {
                PendulumSummoned = true;
                return true;
            }
            return false;
        }
        private bool VFDSummon() {
            if (Duel.Turn == 1) {
                return true;
            }
            return false;
        }
        private bool GizmekKakuSummon() {
            if (Bot.HasInHand(CardId.GizmekKaku)) {
                return true;
            }
            return false;
        }
        private bool SummonEnteblethnir() {
            if (Duel.Turn==1) {
                return false;
            }
            return false;
        }
        // blind negate
        private bool ActivateZefraDivineStrike() {
            if (Duel.LastChainPlayer == 1) {
                return true;
            }
            return false;
        }
        private bool TrueKingStun() {
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.Standby) {
                return true;
            }
            return false;
        }
        private bool SetDivineStrike() {
            if (Duel.Phase == DuelPhase.Main2 || Duel.Turn==1) {
                return true;
            }
            return false;
        }
        public override int OnSelectPlace(long cardId, int player, CardLocation location, int available) {
            if (cardId == CardId.Crocodragon)
            {
                if ((Zones.z5 & available) > 0) return Zones.z5;
                if ((Zones.z6 & available) > 0) return Zones.z6;
            }
            return base.OnSelectPlace(cardId, player, location, available);
        }
        // this is a hack so we can pendulum summon
        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, long hint, bool cancelable)
        {
            const long HINTMSG_FMATERIAL = 511;
            const long HINTMSG_SMATERIAL = 512;
            const long HINTMSG_XMATERIAL = 513;
            const long HINTMSG_LMATERIAL = 533;
            const long HINTMSG_SPSUMMON = 509;
            if (hint == HINTMSG_SPSUMMON)//this also TRUE if we activate gamma
            {
                IList<ClientCard> selected = new List<ClientCard>();
                if (cards.Count > 0 && cards[0].Location == CardLocation.Extra) { //pendulum summon from extra deck
                    for (int i = 1; i <= max; ++i) {
                        ClientCard card = cards[cards.Count - i];
                        if (card.IsCode(CardId.Zefraniu) || card.IsCode(CardId.Zefraxi))
                        {
                            selected.Add(card);
                            continue;
                        }
                    }
                }
                if (selected.Count == 0) {
                    selected.Add(cards[cards.Count - 1]);
                }
                return selected;
            } else if (hint == HINTMSG_LMATERIAL) {

            }
            return null;
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == CardId.Zefraniu || cardId==CardId.Zefrathuban) {
                return CardPosition.FaceUpDefence;
            }
            if (cardId == CardId.TrueKingVFD) {
                if (Duel.Turn == 1)
                {
                    // play around lightning storm
                    return CardPosition.FaceUpDefence;
                }
                else {
                    return CardPosition.FaceUpAttack;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }
        public override IList<ClientCard> OnSelectPendulumSummon(IList<ClientCard> cards, int max)
        {
            Logger.DebugWriteLine("OnSelectPendulumSummon");
            IList<ClientCard> selected = new List<ClientCard>();
            for (int i = 1; i <= max; ++i)
            {
                
                ClientCard card = cards[cards.Count - i];
                if (card.IsCode(CardId.Zefraniu)||card.IsCode(CardId.Zefraxi)) {
                    selected.Add(card);
                    continue; 
                }
            }
            if (selected.Count == 0)
            selected.Add(cards[cards.Count - 1]);
            return selected;
        }
        private bool ZefraxiToTuner() {
            if (Bot.HasInMonstersZone(CardId.Zefraniu)) {
                AI.SelectCard(CardId.Zefraniu);
            }
            return true;
        }
        private bool SynchroCrocodragon() {
            if (Bot.HasInMonstersZone(CardId.Zefraniu) && Bot.HasInMonstersZone(CardId.Zefraxi)) {
                Bot.MonsterZone.GetMonsters();
                return true;
            }
            //AI.SelectMaterials();
            return false;
        }
        private bool CrocoDragonTrigger() {
            if (ActivateDescription == Util.GetStringId(CardId.Crocodragon, 0)) {
                return true;
            }
            return false;
        }
        private bool OracleTrigger() {
            if (Bot.HasInMonstersZone(CardId.Crocodragon)) {
                if (Bot.GetRemainingCount(CardId.GizmekKaku, 1)>0) {
                    AI.SelectCard(CardId.GizmekKaku);
                    return true;
                }
                if (Bot.GetRemainingCount(CardId.AshBlossom, 1) > 0) {
                    AI.SelectCard(CardId.AshBlossom);
                    return true;
                }
                
            }
            return true;
        }
        private bool ZefraniuSearch() {
            if (!ProvidenceActivated)
            {
                AI.SelectCard(CardId.ZefraProvidence);
                return true;
            } else {
                if (Bot.HasInHand(CardId.ZefraDivineStrike))
                {
                    AI.SelectCard(CardId.NinePilalrs);
                }
                else {
                    AI.SelectCard(CardId.ZefraDivineStrike);
                }
                return true;
            }
        }
        // always use gamma if able
        private bool GammaEffect()
        {
            if (Duel.LastChainPlayer == 1)
            {
                return true;
            }
            return false;
        }
        private bool PutLowScale() {
            return true;
        }
        private bool TerraformingEffect()
        {
            if (OracleActivated)
                return false;
            AI.SelectCard(CardId.OracleZefra);
            return true;
        }

        private bool ZefraathActivate() {
            if (Bot.HasInSpellZone(CardId.Zefraath)){
                return false;
            }
            return true;
        }
        private bool ZefraathDump() {
            if (Bot.HasInSpellZone(CardId.Zefraath))
            {
                if (Card.Location == CardLocation.Hand) {
                    return false;
                }
                if (!Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHand(LowScale)) {
                    AI.SelectCard(CardId.Zefraniu);
                }
                if (Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHand(LowScale)) {
                    AI.SelectCard(HighScaleNoZeraniu);
                }
                if (Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHand(LowScale))
                {
                    AI.SelectCard(HighScaleNoZeraniu);
                }
                if (Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHand(HighScale))
                {
                    AI.SelectCard(HighScaleNoZeraniu);
                }
                return true;
            }
            return false;
            
        }
        private bool ZefraProvidence() {
            if (Bot.HasInMonstersZone(CardId.Zefraniu)) {
                if (Bot.HasInMonstersZone(CardId.Zefraxi))
                {
                    AI.SelectCard(CardId.ZefraDivineStrike);
                }
                else {
                    AI.SelectCard(CardId.ZefraWar);
                }
                return true;
            }
            if (!Bot.HasInHand(CardId.OracleZefra)) {
                AI.SelectCard(CardId.OracleZefra);
                ProvidenceActivated = true;
                return true;
            }

            if (!Bot.HasInHand(CardId.Zefraath)) {
                AI.SelectCard(CardId.Zefraath);
                ProvidenceActivated = true;
                return true;
            }

            if (Bot.HasInHand(LowScale) && !Bot.HasInHand(CardId.Zefraxi)) {
                AI.SelectCard(CardId.Zefraxi);
                ProvidenceActivated = true;
                return true;
            }

            if (Bot.HasInHand(HighScale) && !Bot.HasInHand(CardId.Zefraniu)) {
                AI.SelectCard(CardId.Zefraniu);
                ProvidenceActivated = true;
                return true;
            }
            if (Bot.HasInHand(HighScale) && !Bot.HasInHand(CardId.Zefrathuban))
            {
                AI.SelectCard(CardId.Zefrathuban);
                ProvidenceActivated = true;
                return true;
            }
            AI.SelectCard(BackRowSearchPriority);
            return true;
        }
        private bool OracleActivate()
        {
            if (OracleActivated)
                return false;
            OracleActivated = true;
            if (!Bot.HasInHand(CardId.Zefraath)) {
                AI.SelectCard(CardId.Zefraath);
                return true;
            }

            if (Bot.HasInHand(LowScale) && !Bot.HasInHand(CardId.Zefraxi)) {
                AI.SelectCard(CardId.Zefraxi);
                return true;
            }
            if (Bot.HasInHand(HighScale) && !Bot.HasInHand(CardId.Zefraniu)) {
                AI.SelectCard(CardId.Zefraniu);
                return true;
            }
            if (!Bot.HasInHand(LowScaleNoZeraxi) && Bot.HasInHand(CardId.Zefraxi)) {
                AI.SelectCard(LowScaleNoZeraxi);
                return true;
            }
            if (!Bot.HasInHand(HighScaleNoZeraniu) && Bot.HasInHand(CardId.Zefraniu))
            {
                AI.SelectCard(HighScale);
                return true;
            }
            AI.SelectCard(LowScaleNoZeraxi);

            return true;
        }
        public override bool OnSelectHand()
        {
            return true;
        }
    }
}
