namespace ChallengeHarnessInterfaces
{
    public class MatchRender
    {
        public MatchRender()
        {
            Moves = new string[2];
        }

        public int RoundNumber { get; set; }
        public string Round { get; set; }
        public string Map { get; set; }
        public string MapAdvanced { get; set; }
        public string State { get; set; }
        public string[] Moves { get; set; }
    }
}