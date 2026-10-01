using System.Collections.Generic;
using System.Linq;
using WindBot.Game;
using RyzealAI.Prototype;

namespace WindBot.Game.AI.Decks
{
    // ============================================================
    // ClientField/Duel(실제 게임 상태) -> GameState(CardValueEngine용 목업) 변환기
    //
    // 사용법 (RyzealExecutor 안에서):
    //   var state = GameStateAdapter.ToGameState(Bot, Duel, NormalSummonUsed);
    //   var powers = CardValueEngine.EvaluateAll(state);
    //
    // 주의:
    // - "Zone.Field"는 몬스터존+스펠존을 합친 것. Star/Plugin/Cross 조건문에서
    //   "필드에 존재"가 몬스터/스펠 구분 없이 쓰이므로 이렇게 합쳐야 정확함.
    // - NormalSummonUsed / EffectUsed(HOPT)류는 엔진이 안 주므로 Executor가
    //   직접 추적해온 값을 파라미터로 반드시 넘겨야 함 (지난 대화에서 확인된 내용).
    // ============================================================
    public static class GameStateAdapter
    {
        public static GameState ToGameState(ClientField bot, Duel duel, bool normalSummonUsed)
        {
            var state = new GameState();

            state.Zones[Zone.Hand] = bot.Hand.Select(c => c.Id).ToList();

            var fieldIds = new List<int>();
            fieldIds.AddRange(bot.GetMonsters().Select(c => c.Id));
            fieldIds.AddRange(bot.GetSpells().Select(c => c.Id));
            state.Zones[Zone.Field] = fieldIds;

            state.Zones[Zone.Grave] = bot.Graveyard.Select(c => c.Id).ToList();
            state.Zones[Zone.Banished] = bot.Banished.Select(c => c.Id).ToList();

            state.NormalSummonUsed = normalSummonUsed;
            state.IsFirstPlayer = duel.IsFirst;
            state.Turn = duel.Turn;

            return state;
        }
    }
}
