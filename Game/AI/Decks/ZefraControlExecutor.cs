using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YGOSharp.OCGWrapper.Enums;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Net;

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
        private readonly int[] SelfZefraWarTarget = {
            CardId.Zefraniu,
            CardId.Zefrawendi,
            CardId.Zefraxciton,
            CardId.Zefrathuban,
            CardId.OracleZefra,
            CardId.Zefraath,
        };
        private readonly int[] Level3Tuners = {
            CardId.GhostOgre,
            CardId.AshBlossom,
        };
        private bool OracleActivated = false;
        private bool NormalSummonUsed = false;
        private bool ProvidenceActivated = false;
        private bool PendulumSummoned = false;
        private bool UsedGamma = false;

        public override void OnNewTurn() {
            OracleActivated = false;
            NormalSummonUsed = false;
            ProvidenceActivated = false;
            PendulumSummoned = false;
            UsedGamma = false;
        }
        public ZefraControlExecutor(GameAI ai, Duel duel) : base(ai, duel)
        {
            
            // counter
            AddExecutor(ExecutorType.Activate, CardId.PsyFrameGamma, GammaEffect);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultEffectVeiler);
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.GhostOgre, DefaultGhostOgreAndSnowRabbit);
            AddExecutor(ExecutorType.Activate, CardId.ZefraDivineStrike, ActivateZefraDivineStrike);
            AddExecutor(ExecutorType.Activate, CardId.ZefraWar, ActivateZefraWar);

            AddExecutor(ExecutorType.Activate, CardId.TrueKingVFD, TrueKingStun);

            //search field spell
            AddExecutor(ExecutorType.Activate, CardId.Terraforming, TerraformingEffect);
            AddExecutor(ExecutorType.Activate, CardId.ZefraProvidence, ZefraProvidence);

            //set trap except war
            AddExecutor(ExecutorType.SpellSet, CardId.ZefraDivineStrike, SetDivineStrike);
            AddExecutor(ExecutorType.SpellSet, CardId.NinePilalrs, SetNinePIllar);

            //set zefra war if only 1 zefra card in pendulum scale
            AddExecutor(ExecutorType.SpellSet, CardId.ZefraWar, SetZefraWar);

            // try to complete our scale
            AddExecutor(ExecutorType.Activate, CardId.OracleZefra, OracleActivate);
            AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathActivate);
            AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathDump);
            AddExecutor(ExecutorType.Activate, CardId.Zefraniu, ZefraniuSearch);
            //we put synchro summon here so we can stack our card then draw it, it also chain block ash
            AddExecutor(ExecutorType.Activate, CardId.Crocodragon,CrocoDragonTrigger);
            AddExecutor(ExecutorType.Activate, CardId.OracleZefra,OracleTrigger);


            //normal summon our tuner to either access crocodragon or stardust charge warrior
            for (int i1 = 0; i1 < Level3Tuners.Length; i1++)
            {
                int i = Level3Tuners[i1];
                AddExecutor(ExecutorType.Summon, i, ShouldNormalSummonHandtrap);
            }
            AddExecutor(ExecutorType.Activate, CardId.FairyTailLuna, FairyTailLunaSearch);
            AddExecutor(ExecutorType.Activate, CardId.FairyTailLuna, FairyTailLunaBounce);

            //special summon kaku
            AddExecutor(ExecutorType.Activate, CardId.GizmekKaku,GizmekKakuSummon);

            // putting low scale prioritize non zefraxi card
            AddExecutor(ExecutorType.Activate, CardId.Zefrathuban, ScaleZefrathuban);
            AddExecutor(ExecutorType.Activate, CardId.Zefraxi,ZefraxiToTuner);
            AddExecutor(ExecutorType.SpSummon, CardId.Crocodragon, SynchroCrocodragon);

            //if there are no lowscale we must make do with high scale, add non zefraniu card first
            AddExecutor(ExecutorType.Activate, CardId.Zefraxciton,ScaleZefraxciton);


            // some special summoning move 
            AddExecutor(ExecutorType.SpSummon, CardId.Enterblathnir, SummonEnteblethnir);
            AddExecutor(ExecutorType.SpSummon, CardId.PsyframelordLambda, LinkSummonLambda);
            AddExecutor(ExecutorType.SpSummon, CardId.TrueKingVFD, VFDSummon);
            AddExecutor(ExecutorType.Summon, CardId.FairyTailLuna);
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpSummon, CheckPendulumSummon);
            
            //AddExecutor(ExecutorType.SpSummon,CardId.Crocodragon,SynchroCrocodragon);

            //AddExecutor(ExecutorType.Activate, CardId.Zefraath, ZefraathActivate);

        }

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            return DefaultMonsterRepos();
        }
        private bool ScaleZefraxciton() {
            if (Card.Location == CardLocation.Hand)
            {
                return true;
            }
            return false;
        }
        private bool ScaleZefrathuban() {
            if (Card.Location == CardLocation.Hand) {
                return true;
            }
            return false;
        }

        private bool LinkSummonLambda() {
            if (Bot.HasInMonstersZone(CardId.PsyFrameGamma) && Bot.HasInMonstersZone(CardId.PsyFrameDriver)) {
                return true;
            }
            return false;
        }
        private bool HasLv6NonTunerMonster(List<ClientCard> monsterOnField) {
            List<ClientCard> lv6Monster = new List<ClientCard>();
            for (int i = 0; i < monsterOnField.Count; i++) {
                if (!monsterOnField[i].IsTuner() && monsterOnField[i].Level == 6) {
                    return true;
                }
            }
            return false;
        }
        
        private bool ActivateZefraWar() {
            if (!Bot.HasInGraveyard(CardId.ZefraProvidence)) {
                return false;
            }
            ClientCard target = Util.GetBestEnemyCard(canBeTarget: true);
            if (target.IsSpell() || target.IsTrap()) {
                if (!target.HasType(CardType.Continuous) &&
                    !target.HasType(CardType.Field) &&
                    !target.HasType(CardType.Pendulum))
                {
                    return false;
                }
            }
            if (target == null) {
                return false;
            }
            AI.SelectCard(target);

            AI.SelectCard(SelfZefraWarTarget);
            return true;
        }
        private bool HasLv3NonTunerMonster(List<ClientCard> monsterOnField)
        {
            List<ClientCard> lv6Monster = new List<ClientCard>();
            for (int i = 0; i < monsterOnField.Count; i++)
            {
                if (!monsterOnField[i].IsTuner() && monsterOnField[i].Level == 3)
                {
                    return true;
                }
            }
            return false;
        }
        private bool HasTunerOnField(List<ClientCard> monsterOnField) {
            List<ClientCard> lv6Monster = new List<ClientCard>();
            for (int i = 0; i < monsterOnField.Count; i++)
            {
                if (monsterOnField[i].IsTuner() )
                {
                    return true;
                }
            }
            return false;
        }

        private bool FairyTailLunaSearch() {
            const long hack_code = 62205969853120512;
            //if (ActivateDescription == Util.GetStringId(CardId.FairyTailLuna, 0))
            if (ActivateDescription == hack_code)
            {
                return true;
            }
            return false;
        }

        private bool SetNinePIllar() {
            return true;
        }
        private bool ShouldNormalSummonHandtrap() {
            List<ClientCard> monsterOnField = Bot.GetMonsters();
            if (!HasTunerOnField(monsterOnField) && (
                    (HasLv6NonTunerMonster(monsterOnField) && Bot.HasInExtra(CardId.Crocodragon))
                    || (HasLv3NonTunerMonster(monsterOnField) && Bot.HasInExtra(CardId.StardustChargeWarrior)))) {
                return true;
            }
            return false;
        }

        private bool CheckPendulumSummon() {
            if (!PendulumSummoned) {
                //PendulumSummoned = true;
                return true;
            }
            return false;
        }
        private bool VFDSummon() {
            if (Duel.Turn == 1)
            {
                return true;
            }
            else {
                if (Duel.Turn == 2 && Duel.Phase == DuelPhase.Main2) {
                    return true;
                }
            }
            return false;
        }
        private bool GizmekKakuSummon() {
            if (Bot.HasInHand(CardId.GizmekKaku)) {
                return true;
            }
            return false;
        }
        private static ClientCard GetExtraDisruptibleDeckMonster(List<ClientCard> monsterlist) {
            ClientCard result = null;
            for (int i = 0; i < monsterlist.Count; i++) {
                ClientCard temp = monsterlist[i];
                if (temp.IsShouldNotBeTarget()) {
                    continue;
                }
                if (temp.HasType(CardType.Xyz)|| temp.HasType(CardType.Synchro)||temp.HasType(CardType.Fusion)) {
                    return temp;
                }
                if (temp.HasType(CardType.Link) && temp.LinkCount >= 2) {
                    return temp;
                }
            }
            return result;
        }
        private bool FairyTailLunaBounce() {
            if (ActivateDescription == Util.GetStringId(CardId.FairyTailLuna, 1))
            {
                if (Duel.Phase == DuelPhase.BattleStart)
                {
                    ClientCard result = Util.GetProblematicEnemyMonster(canBeTarget : true);
                    if (result == null)
                    {
                        return false;
                    }
                    AI.SelectCard(result.Alias);
                    return true;
                }
                else {
                    ClientCard result = GetExtraDisruptibleDeckMonster(Util.Enemy.GetMonsters());
                    if (result == null)
                    {
                        return false;
                    }
                    AI.SelectCard(result.Alias);
                    return true;
                }
                
            }
            return false;
        }
        private bool SummonEnteblethnir() {
            if (Duel.Turn==1) {
                return false;
            }
            return false;
        }
        public override int OnSelectOption(IList<long> options)
        {
            if (Card.Alias == CardId.TrueKingVFD) {
                for (int i = 0; i < options.Count; i++) {
                }
            }
            return base.OnSelectOption(options);
        }
        private bool SetZefraWar() {
            List<ClientCard> spellOnZone = Bot.GetSpells();
            int zefraCount = 0;
            List<int> zefraCards = new List<int>{
                CardId.Zefraath,
                CardId.Zefraniu,
                CardId.Zefraxi,
                CardId.Zefrathuban,
                CardId.Zefraxciton,
                CardId.ZefraPath,
                CardId.Zefrawendi,
            };
            for (int i = 0; i < spellOnZone.Count; i++) {
                if (zefraCards.Contains(spellOnZone[i].Alias)) {
                    zefraCount++;
                }
            }
            return zefraCount<2;
        }
        // blind negate
        private bool ActivateZefraDivineStrike() {
            if (Duel.LastChainPlayer == 1) {
                return true;
            }
            return false;
        }
        
        private bool TrueKingStun()  {
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.Standby) {
                AI.SelectAttribute(CardAttribute.Dark);// Select DARK as default
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
                if (!UsedGamma)
                {
                    PendulumSummoned = true;
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
                //Bot.MonsterZone.GetMonsters();
                return true;
            }
            if (Bot.HasInMonstersZone(CardId.Zefraniu) && Bot.HasInMonstersZone(Level3Tuners))
            {
                //Bot.MonsterZone.GetMonsters();
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
            if (ActivateDescription == Util.GetStringId(CardId.OracleZefra, 1)) {

            }
            if (Bot.HasInMonstersZone(CardId.Crocodragon))
            {
                if (Bot.GetRemainingCount(CardId.GizmekKaku, 1) > 0)
                {
                    AI.SelectCard(CardId.GizmekKaku);
                    return true;
                }
                AI.SelectCard(Level3Tuners);
                return true;
            }
            else if (Bot.HasInMonstersZone(CardId.StardustChargeWarrior))
            {
                AI.SelectCard(Level3Tuners);
                return true;
            }
            else if (Bot.HasInMonstersZone(CardId.MetaphysHorus)) {
                AI.SelectCard(CardId.ZefraProvidence);
                return true;
            }
            return true;
        }
        private bool ZefraniuSearch() {
            if (Card.Location == CardLocation.Hand) {
                return false;
            }
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
                UsedGamma = true;
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
            if (Bot.HasInSpellZone(LowScale) || Bot.HasInSpellZone(HighScale)) {
                return true;
            }
            if (!Bot.HasInHand(LowScale) && !Bot.HasInHand(HighScale))
            {
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
                if (!Bot.HasInHand(LowScale) && !Bot.HasInHand(HighScale) && !Bot.HasInSpellZone(HighScale) && !Bot.HasInSpellZone(LowScale)) {                    
                    return false;
                }
                if (!Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHandOrInSpellZone(LowScale)) {
                    AI.SelectCard(CardId.Zefraniu);
                }else if (Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHandOrInSpellZone(LowScale)) {
                    AI.SelectCard(HighScaleNoZeraniu);
                }
                if (Bot.HasInHand(CardId.Zefraniu) && !Bot.HasInHandOrInSpellZone(LowScale))
                {
                    AI.SelectCard(CardId.Zefraxi);
                }
                else if (!Bot.HasInHand(CardId.Zefraxi) && Bot.HasInHandOrInSpellZone(LowScale)) {
                    AI.SelectCard(CardId.Zefraniu);                        
                }else if (Bot.HasInHand(CardId.Zefraniu) && Bot.HasInHandOrInSpellZone(HighScale))
                {
                    if (Bot.HasInSpellZone(HighScale))
                    {
                        AI.SelectCard(LowScale);
                    }
                    else {
                        AI.SelectCard(HighScale);
                    }
                    
                }else if (Bot.HasInHand(CardId.Zefraxi) && Bot.HasInHand(CardId.Zefraniu)) {
                    if (Bot.HasInHand(LowScaleNoZeraxi)) {
                        AI.SelectCard(HighScaleNoZeraniu);
                    }else if (Bot.HasInHand(HighScaleNoZeraniu))
                    {
                        AI.SelectCard(LowScaleNoZeraxi);
                    }
                }
                return true;
            }
            return false;
            
        }
        private bool ZefraProvidence() {
            //out first turn pendulum completition
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

            if (OpponentsHasSetCard()) {
                AI.SelectCard(CardId.Zefraxciton);
            }
            AI.SelectCard(BackRowSearchPriority);
            return true;
        }
        private bool CardIsFaceDown(ClientCard c) {
            return c.HasPosition(CardPosition.FaceDown);
        }
        private bool OpponentsHasSetCard() {
            ClientCard result = Enemy.GetMonstersInMainZone().GetFirstMatchingCard(CardIsFaceDown);
            if (result != null) {
                return true;
            }
            result = Enemy.GetSpells().GetFirstMatchingCard(CardIsFaceDown);
            return result != null;

        }
        private bool OracleActivate()
        {
            if (OracleActivated)
                return false;
            OracleActivated = true;
            if (!Bot.HasInHand(CardId.Zefraath) && !Bot.HasInSpellZone(CardId.Zefraath)) {
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
                AI.SelectCard(HighScaleNoZeraniu);
                return true;
            }
            if (Bot.HasInSpellZone(CardId.Zefraath) && !Bot.HasInHand(LowScale)) {
                AI.SelectCard(LowScale);
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
