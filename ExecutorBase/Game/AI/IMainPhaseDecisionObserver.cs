namespace WindBot.Game.AI
{
    // Optional observer; other decks need not implement it.
    public interface IMainPhaseDecisionObserver
    {
        void ObserveDuelStart();
        void ObserveLegacyMainPhaseAction(string kind, int cardId, long description);
    }
}
