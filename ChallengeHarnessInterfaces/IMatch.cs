namespace ChallengeHarnessInterfaces
{
    public interface IMatch
    {
        int GetRoundNumber();
        void Update();
        bool GameIsOver();
        MatchResult GetResult();
        void SetPlayerName(int player, string playerName);
        void SetPlayerMove(int player, string move);
        string GetPlayerLastMove(int player);
        string GetPlayerLastMoveResult(int player);
        string GetRules();
        IMatch GetFlippedCopyOfMatch();
    }
}