using System;
using System.IO;
using ChallengeHarness.Properties;
using ChallengeHarnessInterfaces;

namespace ChallengeHarness.Loggers
{
    public class MatchLogger : ILogger
    {
        public MatchLogger()
        {
            FileName = Settings.Default.MatchLogFilename;

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            FileWriter = new StreamWriter(FileName);
        }

        public string FileName { get; private set; }
        protected StreamWriter FileWriter { get; private set; }

        public void Log(MatchRender rendered)
        {
            FileWriter.WriteLine();
            FileWriter.WriteLine(rendered.Round);
            FileWriter.WriteLine(rendered.Map);

            foreach (var move in rendered.Moves)
            {
                FileWriter.WriteLine(move);
            }
        }

        public void Log(MatchSummary result)
        {
            Log(String.Format("Match result: Player {0} wins", result.Winner));
            Log(String.Format("Win reason: {0}", result.WinReason));
        }

        public void Log(string message)
        {
            FileWriter.WriteLine(message);
        }

        public void Close()
        {
            FileWriter.Flush();
            FileWriter.Close();
        }
    }
}