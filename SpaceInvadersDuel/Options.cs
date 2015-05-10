using CommandLine;
using CommandLine.Text;

namespace SpaceInvadersDuel
{
    public class Options
    {
        [Option('o', "one", DefaultValue = "player1",
            HelpText = "Relative path to the folder containing the player one bot")]
        public string PlayerOneBotFolder { get; set; }

        [Option('t', "two", DefaultValue = "player2",
            HelpText = "Relative path to the folder containing the player two bot")]
        public string PlayerTwoBotFolder { get; set; }

        [Option('r', "rules", DefaultValue = false,
            HelpText = "Prints out the rules and saves them in markdown format to rules.md")]
        public bool ShowRules { get; set; }
        
        [Option('q', "quiet", DefaultValue = false,
            HelpText = "Disables console logging - logs will only be written to files.")]
        public bool Quiet { get; set; }

        [Option('s', "scrolling", DefaultValue = false,
            HelpText = "Forces scrolling console log output, which shouldn't crash when running the harness from another application.")]
        public bool Scrolling { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}