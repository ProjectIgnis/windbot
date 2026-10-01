using System;

namespace RyzealAI.Prototype
{
    public enum ActionType { NormalSummon, SpecialSummon, Activate }

    // AI policy가 평가할 수 있는, 엔진이 이미 합법으로 제시한 행동 1개.
    public sealed class ActionCandidate
    {
        public ActionType ActionType { get; set; }
        public int CardId { get; set; }
        public int ActionIndex { get; set; }
        public long ActivationDescription { get; set; }
        public double BaselineScore { get; set; }
        public string Label { get; set; }
        public string GoalTag { get; set; }
        // Planned prompt choices, for traceability. This is not an engine legality source.
        public string SelectionPlan { get; set; }
        public Func<bool> Commit { get; set; }
    }
}
