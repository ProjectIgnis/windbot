using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RyzealAI.Prototype
{
    // 외부 JSON 라이브러리 없이 JSONL을 남긴다. 한 줄은 한 번의 Main Phase 의사결정이다.
    public sealed class DuelDecisionLogger
    {
        private string _path;
        private readonly string _directory;
        private readonly object _gate = new object();
        private string _duelId = "unstarted-" + Guid.NewGuid().ToString("N");
        private int _decisionNumber;
        private string _decisionId;

        public void BeginDuel()
        {
            lock (_gate)
            {
                _duelId = Guid.NewGuid().ToString("N");
                _decisionNumber = 0;
                _decisionId = null;
                SelectNewLogPath();
            }
        }
        public void BeginDecision()
        {
            _decisionNumber++;
            _decisionId = _duelId + ":" + _decisionNumber.ToString(CultureInfo.InvariantCulture);
        }
        private string CorrelationJson(bool associateDecision)
        {
            return Field("duelId", _duelId) + "," +
                "\"decisionId\":" + (associateDecision && _decisionId != null ? "\"" + Escape(_decisionId) + "\"" : "null") + "," +
                Field("timestampUtc", DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture));
        }


        public DuelDecisionLogger(string path)
        {
            // Keep the existing constructor call; only its directory is used.
            _directory = Path.GetDirectoryName(path);
            if (String.IsNullOrEmpty(_directory)) _directory = ".";
            SelectNewLogPath();
            string directory = _directory;
            if (!String.IsNullOrEmpty(directory))
            {
                try { Directory.CreateDirectory(directory); }
                catch (IOException ex) { Console.Error.WriteLine("Ryzeal log directory failed: " + ex.Message); }
                catch (UnauthorizedAccessException ex) { Console.Error.WriteLine("Ryzeal log directory access failed: " + ex.Message); }
            }
        }

        public string LogPath { get { return _path; } }

        private void SelectNewLogPath()
        {
            // Local machine time; UTC timestamps inside events stay unchanged.
            // Full GUID prevents concurrent duel files sharing the same timestamp.
            string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture)
                + "_" + _duelId + ".jsonl";
            _path = Path.Combine(_directory, fileName);
        }

        public void Write(DecisionTrace trace)
        {
            lock (_gate)
            {
                try { File.AppendAllText(_path, ToJson(trace) + Environment.NewLine, Encoding.UTF8); }
                catch (IOException ex) { Console.Error.WriteLine("Ryzeal log write failed: " + ex.Message); }
                catch (UnauthorizedAccessException ex) { Console.Error.WriteLine("Ryzeal log access failed: " + ex.Message); }
            }
        }

        public void WriteLegacyAction(int turn, string phase, string kind, int cardId, long description)
        {
            string json = "{" + Field("eventType", "legacyMainPhaseSelection") + "," + CorrelationJson(true) + "," +
                Field("turn", turn) + "," + Field("phase", phase) + "," +
                Field("kind", kind) + "," + Field("cardId", cardId) + "," +
                Field("activationDescription", description.ToString(CultureInfo.InvariantCulture)) + "}";
            lock (_gate)
            {
                try { File.AppendAllText(_path, json + Environment.NewLine, Encoding.UTF8); }
                catch (IOException ex) { Console.Error.WriteLine("Ryzeal legacy log failed: " + ex.Message); }
                catch (UnauthorizedAccessException ex) { Console.Error.WriteLine("Ryzeal legacy log access failed: " + ex.Message); }
            }
        }

        public void WriteSelection(string context, int turn, IEnumerable<int> offered, int selected)
        {
            string json = "{" + Field("eventType", "cardSelection") + "," + CorrelationJson(false) + "," +
                Field("context", context) + "," + Field("turn", turn) + "," +
                IntArray("offeredCardIds", offered) + "," + Field("selectedCardId", selected) + "}";
            lock (_gate)
            {
                try { File.AppendAllText(_path, json + Environment.NewLine, Encoding.UTF8); }
                catch (IOException ex) { Console.Error.WriteLine("Ryzeal selection log failed: " + ex.Message); }
                catch (UnauthorizedAccessException ex) { Console.Error.WriteLine("Ryzeal selection log access failed: " + ex.Message); }
            }
        }

        private string ToJson(DecisionTrace t)
        {
            return "{" +
                Field("eventType", "mainPhaseDecision") + "," + CorrelationJson(true) + "," +
                Field("turn", t.Turn) + "," +
                Field("phase", t.Phase) + "," +
                Field("turnPlayer", t.TurnPlayer) + "," +
                Field("isFirst", t.IsFirst) + "," +
                Field("normalSummonUsed", t.NormalSummonUsed) + "," +
                "\"self\":" + ZoneJson(t.Self) + "," +
                "\"opponent\":" + ZoneJson(t.Opponent) + "," +
                "\"legalActions\":" + LegalJson(t.LegalActions) + "," +
                "\"policyCandidates\":" + CandidateJson(t.PolicyCandidates) + "," +
                "\"selected\":" + SelectedJson(t.Selected) + "," +
                Field("selectionResult", t.SelectionResult) +
                "}";
        }

        private static string ZoneJson(FieldTrace f)
        {
            return "{" + IntArray("hand", f.Hand) + "," + IntArray("monsters", f.Monsters) + "," +
                   IntArray("spells", f.Spells) + "," + IntArray("graveyard", f.Graveyard) + "," +
                   IntArray("banished", f.Banished) + "," + IntArray("extra", f.Extra) + "}";
        }
        private static string LegalJson(IEnumerable<LegalActionTrace> xs) =>
            "[" + String.Join(",", xs.Select(x => "{" + Field("kind", x.Kind) + "," + Field("cardId", x.CardId) + "," + Field("activationDescription", x.ActivationDescription) + "," + Field("actionIndex", x.ActionIndex) + "}")) + "]";
        private static string CandidateJson(IEnumerable<CandidateTrace> xs) =>
            "[" + String.Join(",", xs.Select(x => "{" + Field("kind", x.Kind) + "," + Field("cardId", x.CardId) + "," + Field("baselineScore", x.BaselineScore) + "," + Field("label", x.Label) + "," + Field("goalTag", x.GoalTag) + "," + Field("selectionPlan", x.SelectionPlan) + "}")) + "]";
        private static string SelectedJson(CandidateTrace x) => x == null ? "null" :
            "{" + Field("kind", x.Kind) + "," + Field("cardId", x.CardId) + "," + Field("baselineScore", x.BaselineScore) + "," + Field("label", x.Label) + "," + Field("goalTag", x.GoalTag) + "," + Field("selectionPlan", x.SelectionPlan) + "}";
        private static string IntArray(string key, IEnumerable<int> xs) => "\"" + key + "\":[" + String.Join(",", xs) + "]";
        private static string Field(string key, string value) => "\"" + key + "\":\"" + Escape(value ?? "") + "\"";
        private static string Field(string key, int value) => "\"" + key + "\":" + value.ToString(CultureInfo.InvariantCulture);
        private static string Field(string key, long value) => "\"" + key + "\":" + value.ToString(CultureInfo.InvariantCulture);
        private static string Field(string key, double value) => "\"" + key + "\":" + value.ToString("R", CultureInfo.InvariantCulture);
        private static string Field(string key, bool value) => "\"" + key + "\":" + (value ? "true" : "false");
        private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
    }

    public sealed class DecisionTrace
    {
        public int Turn; public string Phase; public int TurnPlayer; public bool IsFirst; public bool NormalSummonUsed;
        public FieldTrace Self = new FieldTrace(); public FieldTrace Opponent = new FieldTrace();
        public List<LegalActionTrace> LegalActions = new List<LegalActionTrace>();
        public List<CandidateTrace> PolicyCandidates = new List<CandidateTrace>();
        public CandidateTrace Selected; public string SelectionResult;
    }
    public sealed class FieldTrace { public List<int> Hand = new List<int>(); public List<int> Monsters = new List<int>(); public List<int> Spells = new List<int>(); public List<int> Graveyard = new List<int>(); public List<int> Banished = new List<int>(); public List<int> Extra = new List<int>(); }
    public sealed class LegalActionTrace { public string Kind; public int CardId; public long ActivationDescription; public int ActionIndex; }
    public sealed class CandidateTrace { public string Kind; public int CardId; public double BaselineScore; public string Label; public string GoalTag; public string SelectionPlan; }
}
