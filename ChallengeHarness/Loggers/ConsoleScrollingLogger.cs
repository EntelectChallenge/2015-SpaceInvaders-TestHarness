using System;
using System.Diagnostics;
using ChallengeHarnessInterfaces;

namespace ChallengeHarness.Loggers
{
    public class ConsoleScrollingLogger : ILogger
    {
        protected const string title = "Entelect 100K Challenge: Space Invaders";

        public ConsoleScrollingLogger()
        {
            Console.Clear();
        }

        public void Log(MatchRender rendered)
        {
            WriteToConsoleAndDebug(title);
            WriteToConsoleAndDebug(rendered.Map);

            Console.WriteLine();
            Console.WriteLine();

            foreach (var move in rendered.Moves)
            {
                WriteToConsoleAndDebug(move);
            }
        }

        public void Log(MatchSummary summary)
        {

        }

        protected void WriteToConsoleAndDebug(string message)
        {
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
