using System;
using System.Diagnostics;
using System.IO;
using ChallengeHarness.Runners;
using CommandLine;
using SpaceInvaders.Core;
using SpaceInvaders.Renderers;

namespace SpaceInvadersDuel
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            if (!Parser.Default.ParseArguments(args, options))
            {
                return;
            }

            if (options.ShowRules)
            {
                var rules = Match.GetInstance().GetRules();

                using (var file = new StreamWriter("rules.md"))
                {
                    file.WriteLine(rules);
                }

                Console.WriteLine("Saved these rules in rules.md:");
                Console.WriteLine(rules);
                return;
            }

            try
            {
                var match = Match.GetInstance();
                var runner = new MatchRunner(
                    match,
                    options.PlayerOneBotFolder,
                    options.PlayerTwoBotFolder,
                    new SpaceInvadersRenderer(),
                    options.Quiet,
                    options.Scrolling,
                    options.Log
                    );
                runner.Run();
            }
            catch (FileNotFoundException ex)
            {
                Debug.WriteLine("File not found...");
                Console.WriteLine("File not found...");

                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);

                Debug.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Something went wrong...");
                Console.WriteLine("Something went wrong...");

                Debug.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);

                Debug.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}