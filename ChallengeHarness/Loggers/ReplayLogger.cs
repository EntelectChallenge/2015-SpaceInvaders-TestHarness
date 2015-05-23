using System;
using System.IO;
using ChallengeHarness.Properties;
using ChallengeHarnessInterfaces;
using Newtonsoft.Json;

namespace ChallengeHarness.Loggers
{
    public class ReplayLogger : ILogger
    {
        public ReplayLogger(string replayFolder)
        {
            if ((replayFolder != null) && (!replayFolder.Trim().Equals(""))) {
                SetUserDefinedReplayFolder(replayFolder);
            } else {
                SetAutomaticReplayFolder();
            }
        }

        public string ReplayDirectory { get; private set; }
        public int MatchId { get; private set; }

        private void SetUserDefinedReplayFolder(string replayFolder)
        {
            ReplayDirectory = replayFolder;

            if (!Directory.Exists(ReplayDirectory))
            {
                Directory.CreateDirectory(ReplayDirectory);
            }
        }

        private void SetAutomaticReplayFolder()
        {
            if (!Directory.Exists(Settings.Default.ReplaysFolder))
            {
                Directory.CreateDirectory(Settings.Default.ReplaysFolder);
            }

            ReplayDirectory = CalculateNextReplayFolderNumbered();

            // Below would be a better option, but not switching to it now to avoid making a breaking
            // change for people that have automated running their test matches...
            // ReplayDirectory = CalculateNextReplayFolderDate(); 
            
            Directory.CreateDirectory(ReplayDirectory);
        }

        public void Log(MatchRender rendered)
        {
            SaveMap(rendered);
            SaveMapAdvanced(rendered);

            SaveState(rendered);

            SavePlayerOneMove(rendered);
            SavePlayerTwoMove(rendered);
        }

        public void Log(MatchSummary summary)
        {
            var result = JsonConvert.SerializeObject(
                summary,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            );

            using (
                var writer =
                    new StreamWriter(ReplayDirectory + Path.DirectorySeparatorChar +
                                     Settings.Default.MatchResultFilename))
            {
                writer.WriteLine(result);
            }
        }

        public void CopyMatchLog(string fileName)
        {
            File.Copy(fileName, ReplayDirectory + Path.DirectorySeparatorChar + Settings.Default.MatchLogFilename,
                true);
        }

        public void CopyBotLog(string fileName, int playerNumber)
        {
            File.Copy(fileName, ReplayDirectory + Path.DirectorySeparatorChar + "bot" + playerNumber + ".log", true);
        }

        protected string CalculateNextReplayFolderNumbered()
        {
            var replays = Directory.GetDirectories(Settings.Default.ReplaysFolder);
            MatchId = 1;
            if (replays.Length > 0)
            {
                Array.Sort(replays);
                var lastReplayName = replays[replays.Length - 1].Split(Path.DirectorySeparatorChar)[1];
                MatchId = Int16.Parse(lastReplayName) + 1;
            }

            return Settings.Default.ReplaysFolder + Path.DirectorySeparatorChar + MatchId.ToString("D4");
        }

        protected string CalculateNextReplayFolderDate()
        {
            DateTime dt = new DateTime();
            string dateString = dt.Year + "" + dt.Month + "" + dt.Day + "" + dt.Hour + "" + dt.Second + "" + dt.Millisecond;
            return Settings.Default.ReplaysFolder + Path.DirectorySeparatorChar + dateString;
        }

        protected void SaveMap(MatchRender rendered)
        {
            WriteFile(rendered.RoundNumber, Settings.Default.MapFilename, rendered.Map);
        }
        protected void SaveMapAdvanced(MatchRender rendered)
        {
            WriteFile(rendered.RoundNumber, Settings.Default.MapAdvancedFilename, rendered.MapAdvanced);
        }

        protected void SaveState(MatchRender rendered)
        {
            WriteFile(rendered.RoundNumber, Settings.Default.StateFilename, rendered.State);
        }

        protected void SavePlayerOneMove(MatchRender rendered)
        {
            WriteFile(rendered.RoundNumber, Settings.Default.ReplayPlayerOneMoveFilename, rendered.Moves[0]);
        }

        protected void SavePlayerTwoMove(MatchRender rendered)
        {
            WriteFile(rendered.RoundNumber, Settings.Default.ReplayPlayerTwoMoveFilename, rendered.Moves[1]);
        }

        protected void WriteFile(int iteration, string filename, string message)
        {
            var iterationFolder = ReplayDirectory + Path.DirectorySeparatorChar + iteration.ToString("D3");
            if (!Directory.Exists(iterationFolder))
            {
                Directory.CreateDirectory(iterationFolder);
            }

            using (var writer = new StreamWriter(iterationFolder + Path.DirectorySeparatorChar + filename))
            {
                writer.WriteLine(message);
            }
        }
    }
}