using System;
using System.Diagnostics;
using ChallengeHarnessInterfaces;

namespace ChallengeHarness.Loggers
{
    public class ConsoleLogger : ILogger
    {
		protected const string title = "Entelect 100K Challenge: Space Invaders";

        public ConsoleLogger()
        {
            Console.Clear();
        }

        public void Log(MatchRender rendered)
        {

            LogWithoutScrolling(rendered);
        }

		public void Log(MatchSummary summary)
		{
			var message = String.Format("Match result: Player {0} wins{1}Win reason: {2}", summary.Winner, Environment.NewLine, summary.WinReason);
			Debug.WriteLine(message);
			Console.WriteLine(message);
		}

		protected void LogWithoutScrolling (MatchRender rendered)
		{
			var line = 0;
			
			WriteToConsoleAndDebug (0, line++, title);

			WriteToConsoleAndDebug(0, line, rendered.Map);
			line += CalculateRenderedMapHeight(rendered.Map);

			foreach (var move in rendered.Moves)
			{
				WriteToConsoleAndDebug(0, line++, move);
			}
		}

        protected void WriteToConsoleAndDebug(int x, int y, string message)
        {
            Debug.WriteLine(message);

            Console.SetCursorPosition(x, y);
            Console.WriteLine(new string(' ', Console.WindowWidth));

            Console.SetCursorPosition(x, y);
            Console.WriteLine(message);
        }

        private int CalculateRenderedMapHeight(string mapView)
        {
            return mapView.Split('\n').Length;
        }
    }
}