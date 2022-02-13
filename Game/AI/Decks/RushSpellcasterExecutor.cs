using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("RushSpellcaster", "AI_RushSpellcaster")]
    public class RushSpellcasterExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int DarkSorceror = 160301005;
            public const int Wolfram = 160301007;
            public const int StrayCat = 160301010;
            public const int FireGolem = 160001019;
            public const int MysticDealer = 160301006;
            public const int WhisperingFairy = 160001018;
            public const int DefensiveDragonMage = 160001031;
            public const int LuminousShaman = 160301009;

            public const int SevensRoad = 160301001;
            public const int WindcasterTorna = 160301002;
            public const int RoadWitch = 160401001;

            public const int RecoveryForce = 160001038;
            public const int HammerCrush = 160001041;
            public const int WindBlessing = 160301011;
            public const int MagicalStream = 160301012;

            public const int DarkLiberation = 160301013;
            public const int CurtainSparks = 160301014;
        }

        public RushSpellcasterExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // summon weenies
            AddExecutor(ExecutorType.Summon, CardId.MysticDealer, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.FireGolem, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.WhisperingFairy, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.DefensiveDragonMage, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.LuminousShaman, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.DarkSorceror, NormalSummon);
            AddExecutor(ExecutorType.Summon, CardId.Wolfram, NormalSummon);
            AddExecutor(ExecutorType.MonsterSet, CardId.StrayCat, NormalSummon);

            AddExecutor(ExecutorType.MonsterSet, CardId.MysticDealer, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.FireGolem, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.WhisperingFairy, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.DefensiveDragonMage, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.LuminousShaman, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.DarkSorceror, NormalSet);
            AddExecutor(ExecutorType.MonsterSet, CardId.Wolfram, NormalSet);

            AddExecutor(ExecutorType.Activate, CardId.MysticDealer, GenericDiscard); // use before being tributed

            // tribute summons
            AddExecutor(ExecutorType.Summon, CardId.RoadWitch, OneTributeSummon);
            AddExecutor(ExecutorType.Summon, CardId.WindcasterTorna, OneTributeSummon);
            AddExecutor(ExecutorType.Activate, CardId.RoadWitch, GenericDiscard);
            AddExecutor(ExecutorType.Summon, CardId.SevensRoad, SevensRoadSummon); // after witch eff

            // good discard effs
            AddExecutor(ExecutorType.Activate, CardId.WindBlessing, WindBlessEff);
            AddExecutor(ExecutorType.Activate, CardId.DefensiveDragonMage, DefDragonEff);

            // other effects
            AddExecutor(ExecutorType.Activate, CardId.MagicalStream, MagStreamEff);
            AddExecutor(ExecutorType.Activate, CardId.RecoveryForce, RecForceEff);

            // set traps, then spells to empty hand
            AddExecutor(ExecutorType.SpellSet, CardId.DarkLiberation, SetBackrow);
            AddExecutor(ExecutorType.SpellSet, CardId.CurtainSparks, SetBackrow);
            AddExecutor(ExecutorType.SpellSet, CardId.WindBlessing, SetBackrow);
            AddExecutor(ExecutorType.SpellSet, CardId.MagicalStream, SetBackrow);
            AddExecutor(ExecutorType.SpellSet, CardId.RecoveryForce, SetBackrow);

            // activate the bad discard effects, otherwise set it
            AddExecutor(ExecutorType.Activate, CardId.HammerCrush, GenericDiscard);
            AddExecutor(ExecutorType.Activate, CardId.WhisperingFairy, WhisFairyEff);
            AddExecutor(ExecutorType.SpellSet, CardId.HammerCrush, SetBackrow);
            AddExecutor(ExecutorType.Activate, CardId.LuminousShaman, LumShamanEff);

            // after all we can is in GY, set up sevensroad
            AddExecutor(ExecutorType.Activate, CardId.SevensRoad, SevensRoadEff);
            AddExecutor(ExecutorType.Activate, CardId.FireGolem, FireGolemEff); // after sroad for atk manip
            AddExecutor(ExecutorType.Activate, CardId.WindcasterTorna, WindTornaEff);

            // trap cards
            AddExecutor(ExecutorType.Activate, CardId.DarkLiberation, DarkLibEff);
            AddExecutor(ExecutorType.Activate, CardId.CurtainSparks, CurSparksEff);
        }

        public override bool OnSelectHand()
        {
            // go first
            return true;
        }

        // helper function to check if a card is an attribute we don't have in GY yet
        private bool IsUniqueAttribute(ClientCard card, bool includeMzone = false)
        {
            CardAttribute att = (CardAttribute)card.Attribute;
            bool uniqueInGY = !Bot.Graveyard.IsExistingMatchingCard(c => c.HasAttribute(att));
            return includeMzone ? (uniqueInGY && !Bot.MonsterZone.IsExistingMatchingCard(c => c.HasAttribute(att))) : uniqueInGY;
        }

        private bool NormalSet()
        {
            // only set if we need to cower from every enemy
            if (Enemy.GetMonsterCount() > 0 && !Enemy.MonsterZone.IsExistingMatchingCard(c => c.Attack < Card.Attack))
            {
                return false;
            }
            // summon monsters with new attributes first
            return IsUniqueAttribute(Card, true) || !Bot.Hand.IsExistingMatchingCard(c => IsUniqueAttribute(c, true));
        }

        private bool NormalSummon()
        {
            // summon monsters with new attributes first
            return IsUniqueAttribute(Card, true) || !Bot.Hand.IsExistingMatchingCard(c => IsUniqueAttribute(c, true));
        }

        // helper function to get cards we don't mind losing
        private IList<ClientCard> GetAllowedDiscards(bool tribute = false)
        {
            int[] dontGY = {
                CardId.WindcasterTorna,
                CardId.RoadWitch,
                CardId.SevensRoad
            };

            if (tribute)
                return Bot.MonsterZone.GetMatchingCards(c => !c.IsCode(dontGY));

            return Bot.Hand.GetMatchingCards(c => !c.IsCode(dontGY));
        }

        private bool GenericDiscard()
        {
            IList<ClientCard> discards = GetAllowedDiscards();
            if (discards.Count < 1)
                return false;

            AI.SelectCard(discards);
            return true;
        }
        

        private bool OneTributeSummon()
        {

            IList<ClientCard> discards = GetAllowedDiscards(true);
            if (discards.Count < 1)
                return false;

            IList<ClientCard> uniques = discards.GetMatchingCards(c => IsUniqueAttribute(c));
            if (uniques.Count > 0)
                AI.SelectCard(uniques);
            else
                AI.SelectCard(discards);

            return true;
        }

        private bool SevensRoadSummon()
        {
            IList<ClientCard> discards = GetAllowedDiscards(true);
            if (discards.Count < 2)
                return false;

            IList<ClientCard> uniques = discards.GetMatchingCards(c => IsUniqueAttribute(c));
            if (uniques.Count > 1)
                AI.SelectCard(uniques);
            else
                AI.SelectCard(discards);

            return true;
        }

        private bool WindBlessEff()
        {
            IList<ClientCard> discards = GetAllowedDiscards();
            if (discards.Count < 1)
                return false;

            AI.SelectCard(discards);
            AI.SelectNextCard(Util.GetWorstBotMonster());
            return true;
        }

        private bool IsSpellcaster(ClientCard card)
        {
            return (card.Race & (int)CardRace.SpellCaster) > 0;
        }

        private bool DefDragonEff()
        {
            IList<ClientCard> discards = GetAllowedDiscards();
            if (discards.Count < 1)
                return false;

            // protect against dark lib
            if (Enemy.GetSpellCount() > 0 && Enemy.Graveyard.GetMatchingCardsCount(IsSpellcaster) > 3)
            {
                AI.SelectCard(discards);
                AI.SelectNextCard(Util.GetBestBotMonster());
                return true;
            }

            return false;
        }

        private bool MagStreamEff()
        {
            // no way to tell backrow, so just blind MST
            return true;
        }

        private bool RecForceEff()
        {
            IList<ClientCard> okToShuffle = Bot.Graveyard.GetMatchingCards(c => !IsUniqueAttribute(c));
            List<ClientCard> selections = new List<ClientCard>();
            while (okToShuffle.Count > 0 && selections.Count < 3)
            {
                ClientCard sel = okToShuffle[0];
                selections.Add(sel);
                okToShuffle.Remove(sel);
                IList<ClientCard> maybeRemove = okToShuffle.GetMatchingCards(c => c.HasAttribute((CardAttribute)sel.Attribute));
                if (maybeRemove.Count == 1)
                    okToShuffle.Remove(maybeRemove[0]); // if exactly 1 left with attribute, no longer OK to shuffle
            }

            if (selections.Count == 3)
            {
                AI.SelectCard(selections);
                return true;
            }

            return false;
        }

        private bool SetBackrow()
        {
            return true;
        }

        private bool WhisFairyEff()
        {
            IList<ClientCard> discards = GetAllowedDiscards();
            if (discards.Count < 1)
                return false;

            // play around dark lib and sroad
            if (Enemy.Graveyard.Count > 1 && (Enemy.Graveyard.IsExistingMatchingCard(IsSpellcaster) || Enemy.MonsterZone.IsExistingMatchingCard(IsSpellcaster)))
            {
                AI.SelectCard(discards);
                return true;
            }

            return false;
        }

        private bool LumShamanEff()
        {
            ClientCard attacker = Bot.MonsterZone.GetMatchingCards(c => c.Level <= 4 && IsSpellcaster(c)).GetHighestAttackMonster();
            ClientCard target = Util.GetWorstEnemyMonster(true);
            return !(attacker == null) && !(target == null) && (attacker.Attack - target.Attack) > Card.Attack;
        }

        private bool SevensRoadEff()
        {
            return true;
        }

        private bool FireGolemEff()
        {
            IList<ClientCard> target = Enemy.MonsterZone.GetMatchingCards(c => c.IsDefense()
                && Bot.MonsterZone.IsExistingMatchingCard(c2 => c.Defense > c2.Attack && c.Defense - 600 < c2.Attack));

            if (target.Count > 0)
            {
                AI.SelectCard(target);
                return true;
            }

            return false;
        }

        private bool WindTornaEff()
        {
            IList<ClientCard> discards = GetAllowedDiscards();
            if (discards.Count < 1)
                return false;

            IList<ClientCard> threatMonsters = Enemy.MonsterZone.GetMatchingCards(c => c.IsAttack()
                && !Bot.MonsterZone.IsExistingMatchingCard(c2 => c2.Attack > c.Attack)
                && Bot.MonsterZone.IsExistingMatchingCard(c2 => c2.Attack > c.Defense));

            if (threatMonsters.Count > 0)
            {
                AI.SelectCard(discards);
                AI.SelectNextCard(threatMonsters);
                return true;
            }

            IList<ClientCard> weakCards = Enemy.MonsterZone.GetMatchingCards(c => c.IsDefense()
                && Bot.MonsterZone.IsExistingMatchingCard(c2 => c2.Attack > c.Attack));

            if (weakCards.Count > 0)
            {
                AI.SelectCard(discards);
                AI.SelectNextCard(weakCards);
                return true;
            }

            return false;
        }

        private bool DarkLibEff()
        {
            IList<ClientCard> okToShuffle = Bot.Graveyard.GetMatchingCards(c => !IsUniqueAttribute(c));
            List<ClientCard> selections = new List<ClientCard>();
            while (okToShuffle.Count > 0 && selections.Count < 4)
            {
                ClientCard sel = okToShuffle[0];
                selections.Add(sel);
                okToShuffle.Remove(sel);
                IList<ClientCard> maybeRemove = okToShuffle.GetMatchingCards(c => c.HasAttribute((CardAttribute)sel.Attribute));
                if (maybeRemove.Count == 1)
                    okToShuffle.Remove(maybeRemove[0]); // if exactly 1 left with attribute, no longer OK to shuffle
            }

            AI.SelectCard(selections);
            return true;
        }

        private bool CurSparksEff()
        {
            return true;
        }
    }
}