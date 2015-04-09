using System.Collections.Generic;

namespace ChallengeHarnessInterfaces
{
    public class MatchSummary
    {
        public List<Dictionary<string, object>> Players;
        public int Rounds;
        public int Winner;
        public string WinReason;

        public MatchSummary()
        {
            Players = new List<Dictionary<string, object>>(2);
        }
    }
}