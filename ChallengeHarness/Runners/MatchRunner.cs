using ChallengeHarness.Loggers;
using ChallengeHarness.Properties;
using ChallengeHarnessInterfaces;
using System;

namespace ChallengeHarness.Runners
{
    public class MatchRunner
    {
        private readonly ConsoleLogger _consoleLogger;
        private readonly MatchLogger _logger;
        private readonly BotRunner[] _players;
        private readonly ReplayLogger _replayLogger;

        public MatchRunner(IMatch match, string playerOneFolder, string playerTwoFolder, IRenderer renderer)
        {
            Match = match;
            Renderer = renderer;

            _logger = new MatchLogger();
            _consoleLogger = new ConsoleLogger();
            _replayLogger = new ReplayLogger();

			string runFilename = Environment.OSVersion.Platform == PlatformID.Unix ? Settings.Default.BotRunFilenameLinux : Settings.Default.BotRunFilename;
            _players = new BotRunner[2];
            _players[0] = new BotRunner(
                1,
                playerOneFolder,
				runFilename
                );
            _players[1] = new BotRunner(
                2,
                playerTwoFolder,
				runFilename
                );

            match.SetPlayerName(1, _players[0].PlayerName);
            match.SetPlayerName(2, _players[1].PlayerName);
        }

        public IMatch Match { get; private set; }
        public IRenderer Renderer { get; private set; }

        public void Run()
        {
            do
            {
                var renderP1 = Renderer.Render(Match);
                var renderP2 = Renderer.Render(Match.GetFlippedCopyOfMatch());

                LogAll(renderP1);

                GetMove(_players[0], renderP1);
                GetMove(_players[1], renderP2);

                Match.Update();
            } while (!Match.GameIsOver());

            LogAll(Renderer.Render(Match));
            LogAll(Renderer.RenderSummary(Match));

            CopyLogs();
        }

        private void GetMove(BotRunner player, MatchRender rendered)
        {
            var move = player.GetMove(rendered);
            Match.SetPlayerMove(player.PlayerNumber, move);
        }

        private void LogAll(MatchRender renderP1)
        {
            _logger.Log(renderP1);
            _replayLogger.Log(renderP1);
            _consoleLogger.Log(renderP1);
        }

        private void LogAll(MatchSummary summary)
        {
            _logger.Log(summary);
            _replayLogger.Log(summary);
            _consoleLogger.Log(summary);
        }

        private void CopyLogs()
        {
            _logger.Close();

            _replayLogger.CopyMatchLog(_logger.FileName);
            _replayLogger.CopyBotLog(_players[0].BotLogFilename, 1);
            _replayLogger.CopyBotLog(_players[1].BotLogFilename, 2);
        }
    }
}