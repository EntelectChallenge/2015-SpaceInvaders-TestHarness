using System;
using System.Collections.Generic;
using System.Text;
using ChallengeHarnessInterfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpaceInvaders.Command;
using SpaceInvaders.Factories;
using SpaceInvaders.Properties;

namespace SpaceInvaders.Core
{
    public class Match : IMatch
    {
        private static Match _instance;

        [JsonConstructor]
        private Match(List<BuildingInfo> buildingsAvailable, Map map, List<Player> players, int roundNumber,
            int roundLimit)
        {
            BuildingsAvailable = buildingsAvailable;
            Map = map;
            Players = players;
            RoundNumber = roundNumber;
            RoundLimit = roundLimit;
        }

        private Match()
        {
            BuildingsAvailable = new List<BuildingInfo>
            {
                new BuildingInfo(ShipCommand.BuildAlienFactory,
                    EntityType.AlienFactory,
                    Settings.Default.AlienFactoryCost),
                new BuildingInfo(ShipCommand.BuildMissileController,
                    EntityType.MissileController,
                    Settings.Default.MissileControllerCost),
                new BuildingInfo(ShipCommand.BuildShield,
                    EntityType.Shield,
                    Settings.Default.ShieldCost)
            };

            RoundLimit = Settings.Default.RoundLimit;
            RoundNumber = 0;
            Players = new List<Player>();
        }

        public List<BuildingInfo> BuildingsAvailable { get; private set; }
        public Map Map { get; set; }
        public List<Player> Players { get; set; }
        public int RoundNumber { get; private set; }
        public int RoundLimit { get; private set; }

        public void SetPlayerName(int playerNumber, string playerName)
        {
            GetPlayer(playerNumber).PlayerName = playerName;
        }

        public void SetPlayerMove(int playerNumber, string move)
        {
            var player = GetPlayer(playerNumber);
            var ship = player.Ship;

            if (move == null) return;

            ShipCommand command;
            if (Enum.TryParse(move, true, out command) && (ship != null))
            {
                ship.LastCommand = ship.Command = command;
            }
        }

        public string GetPlayerLastMove(int playerNumber)
        {
            var player = GetPlayer(playerNumber);
            var ship = player.Ship;

            return ship == null ? "Nothing" : ship.LastCommand.ToString();
        }

        public string GetPlayerLastMoveResult(int playerNumber)
        {
            var player = GetPlayer(playerNumber);
            var ship = player.Ship;

            return ship == null ? "Ship is respawning." : ship.CommandFeedback;
        }

        public int GetRoundNumber()
        {
            return RoundNumber;
        }

        public void Update()
        {
            Map = Map ?? new Map(23, 29);

            RoundNumber++;

            UpdateAlienManagers();
            Map.UpdateEntities();
            UpdateRespawnPlayersIfNecessary();
        }

        public bool GameIsOver()
        {
            var gameOver = false;
            foreach (var player in Players)
            {
                gameOver = gameOver || (player.Lives <= 0 && player.Ship == null) ||
                           RoundNumber >= RoundLimit;
            }

            return gameOver;
        }

        public MatchResult GetResult()
        {
            if ((Players[0].Lives <= 0) && (Players[0].Ship == null) &&
                ((Players[1].Lives > 0) || (Players[1].Ship != null)))
            {
                return MatchResult.PlayerTwoWins;
            }

            if ((Players[1].Lives <= 0) && (Players[1].Ship == null) &&
                ((Players[0].Lives > 0) || (Players[0].Ship != null)))
            {
                return MatchResult.PlayerOneWins;
            }

            if (Players[0].Kills > Players[1].Kills)
            {
                return MatchResult.PlayerOneWins;
            }

            if (Players[1].Kills > Players[0].Kills)
            {
                return MatchResult.PlayerTwoWins;
            }

            return MatchResult.Tie;
        }

        public string GetRules()
        {
            var rules = new StringBuilder();

            rules.AppendLine(String.Format("# Space Invaders Rules"));

            rules.AppendLine(String.Format("## Introduction"));
            rules.Append(
                "Each player plays as a hover ship trying to defend their world from alien invaders sent by the other player.");
            rules.Append(" Your primary objective is to survive longer than your opponent.");
            rules.Append(" Your secondary objective is to kill as many of the alien scum as possible!");
            rules.Append(" If you are not too busy surviving you may even be able to sneak a missile through to your ");
            rules.AppendLine("opponent's base...");
            rules.AppendLine();
            rules.AppendLine("The rules are:");

            rules.AppendFormat("1. Bots take turns running and output a move for their ship.{0}",
                Environment.NewLine);
            rules.AppendFormat("2. After both bots have output their moves the board state is advanced by 1 round.{0}",
                Environment.NewLine);
            rules.AppendFormat("3. Each player starts with **{0} lives**.{1}",
                Settings.Default.LivesInitial,
                Environment.NewLine);
            rules.AppendFormat(
                "4. If a player's ship is destroyed it will respawn after **{0} rounds** at a cost of **1 life**.{1}",
                Settings.Default.RespawnDelay,
                Environment.NewLine);
            rules.AppendLine(
                "5. Aliens repeatedly spawn below the middle of the map and move predictably across the map.");
            rules.AppendLine("6. Aliens will periodically shoot bullets at the player.");
            rules.AppendFormat("7. Players can sacrifice lives to construct buildings or shields.{0}",
                Environment.NewLine);
            rules.AppendFormat("8. If the player has no ship and no lives left the player loses immediately.{0}",
                Environment.NewLine);
            rules.AppendFormat("9. If both players lose their last lives at the same time the player with the most kills wins.{0}",
                Environment.NewLine);
            rules.AppendLine("10. A player gets a kill when one of their missiles destroys:");
            rules.AppendLine("\t1. An alien on their side of the map.");
            rules.AppendLine("\t2. The opponent's ship.");
            rules.AppendLine("\t3. One of the opponent's buildings.");
            rules.AppendFormat(
                "11. If both players survive until **round {0}** the player with the most kills wins.{1}",
                Settings.Default.RoundLimit,
                Environment.NewLine);
            rules.AppendLine("12. In the event that there is still a tie, player 1 wins.");
            rules.AppendLine();

            rules.AppendLine("## Aliens");
            rules.Append("A wave of aliens spawns whenever the 2 rows below the middle row of the map is clear.");
            rules.AppendFormat(
                "The initial size of a wave is {0} aliens and increases by {1} alien(s) after {2} rounds.{3}",
                Settings.Default.AlienWaveSizeInitial, Settings.Default.AlienWaveSizeBump,
                Settings.Default.AlienWaveSizeBumpRound, Environment.NewLine);
            rules.AppendLine();

            rules.AppendLine("### Movement");
            rules.Append("Aliens will initially move to the left. Once an alien reaches a wall all the waves attacking ");
            rules.Append(
                "that player will advance one row. After advancing the aliens will switch direction, so the first ");
            rules.AppendLine("wave will start moving to the right after hitting the wall and moving down.");
            rules.AppendLine("If an alien moves into a shield it will explode destroying anything in the 8 squares around it.");
            rules.AppendLine();

            rules.AppendLine("### Shooting");
            rules.AppendFormat("An alien shoots every {0} rounds using the following algorithm:{1}",
                Settings.Default.AlienShotCost, Environment.NewLine);
            rules.AppendLine(
                "1. 33.3% chance that an alien in the front wave shoots towards the player's current location.");
            rules.AppendLine("2. 66.6% chance that another random alien from the front 2 waves shoots.");
            rules.AppendLine();

            rules.AppendLine("## Moves");
            rules.AppendLine("Each round a player may make one of the following moves:");
            rules.AppendLine(" * Nothing");
            rules.AppendLine(" * MoveLeft");
            rules.AppendLine(" * MoveRight");
            rules.AppendLine(" * Shoot");
            rules.AppendLine(" * BuildAlienFactory");
            rules.AppendLine(" * BuildMissileController");
            rules.AppendLine(" * BuildShield");
            rules.AppendLine();
            rules.AppendLine("The commands are explained in more detail below.");
            rules.AppendLine();

            rules.AppendLine("### MoveLeft");
            rules.Append("Moves your ship 1 space to the left. ");
            rules.Append(" Moving into bullets, aliens or missiles will destroy your ship. ");
            rules.AppendLine("Moving into a wall will do nothing.");
            rules.AppendLine();

            rules.AppendLine("### MoveRight");
            rules.AppendLine("Moves your ship 1 space to the right.");
            rules.Append(" Moving into bullets, aliens or missiles will destroy your ship. ");
            rules.AppendLine("Moving into a wall will do nothing.");
            rules.AppendLine();

            rules.AppendLine("### Shoot");
            rules.Append(
                "Creates a new missile in front of your ship, provided you current have less than your **MissileLimit** in flight. ");
            rules.AppendFormat(
                "Your initial MissileLimit is {0}, but can be raised by building a **MissileController**.{1}",
                Settings.Default.MissileLimitInitial, Environment.NewLine);
            rules.AppendLine();

            rules.AppendLine("### Build");
            rules.Append("Each of the build commands costs 1 of your lives to execute and will fail if ");
            rules.Append("you are at 0 lives. The command will fail without cost if another building is in the way. ");
            rules.AppendLine("Any benefit that was provided by a building is nullified if the building is destroyed.");
            rules.AppendLine();

            rules.AppendLine("#### BuildAlienFactory");
            rules.Append("Builds an Alien Factory behind your ship. ");
            rules.AppendFormat(
                "The Alien Factory increases the size of alien waves spawning on the opponent's side by {0}.",
                Settings.Default.AlienFactoryWaveSizeBoost);
            rules.AppendLine();
            rules.AppendLine();

            rules.AppendLine("#### MissileController");
            rules.Append("Builds a Missile Controller behind your ship. ");
            rules.AppendFormat("Increases your **MissileLimit** by {0}.", Settings.Default.MissileLimitBoost);
            rules.AppendLine();
            rules.AppendLine();

            rules.AppendLine("#### BuildShield");
            rules.Append("Creates 9 shield blocks in the 3x3 area in front of your ship (if possible). ");
            rules.AppendLine("Spawning shield blocks will destroy any aliens, bullets and missiles in the 3x3 area.");
//            rules.AppendLine();
//            rules.AppendLine("**Example 1 - no shields:**");
//            rules.AppendLine("```javascript");
//            rules.AppendLine("...");
//            rules.AppendLine("...");
//            rules.AppendLine("...");
//            rules.AppendLine("AAA");
//            rules.AppendLine();
//            rules.AppendLine("Command: BuildShield");
//            rules.AppendLine();
//            rules.AppendLine("---");
//            rules.AppendLine("---");
//            rules.AppendLine("---");
//            rules.AppendLine("AAA");
//            rules.AppendLine("```");
//            rules.AppendLine();
//
//            rules.AppendLine("**Example 2 - shields and bullets:**");
//            rules.AppendLine("```javascript");
//            rules.AppendLine("..|");
//            rules.AppendLine("-..");
//            rules.AppendLine("--.");
//            rules.AppendLine("AAA");
//            rules.AppendLine();
//            rules.AppendLine("Command: BuildShield");
//            rules.AppendLine();
//            rules.AppendLine("---");
//            rules.AppendLine("-- ");
//            rules.AppendLine("---");
//            rules.AppendLine("AAA");
//            rules.AppendLine("```");
//            rules.AppendLine();
//            rules.AppendLine(
//                "**Note:** In example 2 the bullet moves before the ship, resulting in a hole in the middle row of shields instead of the top row.");
            rules.AppendLine();

            rules.AppendLine("## Round Structure");
            rules.AppendLine(
                "The match runner (SpaceInvadersDuel.exe) controls the game and executes each round. The runner " +
                "will output match and state files that are flipped so that your bot always appears to be player 1" +
                "even when it is actually player 2. Each round runs as follows:");
            rules.AppendLine("1. Clear bot **output** folders (except for bot.log).");
            rules.AppendLine("2. Write **output/map.txt** containing a visual summary of the game state for player 1.");
            rules.AppendLine("3. Write **output/state.json** containing the complete game state for player 1.");
            rules.AppendLine("4. Run the player 1 bot and read its move from **output/move.txt**.");
            rules.AppendLine("5. Write a flipped **output/map.txt** containing a visual summary of the game state for player 2.");
            rules.AppendLine("6. Write a flipped **output/state.json** containing the complete game state for player 2.");
            rules.AppendLine("7. Run the player 2 bot and read its move from **output/move.txt**.");
            rules.AppendFormat(
                "8. A bot that doesn't finish in **{0} seconds** will be killed and its move set to *Nothing* for the round.{1}",
                Settings.Default.TurnDurationSeconds,
                Environment.NewLine);
            rules.AppendLine("9. Advance the game state by 1 round:");
            rules.AppendLine("    1. Update the alien commander to spawn new aliens and give aliens orders;");
            rules.AppendLine("    2. Update missiles, moving them forward;");
            rules.AppendLine("    3. Update alien bullets, moving them forward;");
            rules.AppendLine("    4. Update aliens, executing their move & shoot orders;");
            rules.AppendLine("    5. Update ships, executing their orders;");
            rules.AppendLine("    6. Advance respawn timer and respawn ships if necessary.");
            rules.AppendLine("10. If not game end, repeat.");
            rules.AppendLine("");
            rules.Append("A detailed match log including the state and map files for every round ");
            rules.AppendLine("is written in the **Replays** folder for use in debugging and visualizations.");
            rules.AppendLine();

            rules.AppendLine("## Map");
            rules.AppendFormat(
                "The game is played on a {0}x{1} tiled map. The edges of the map are filled with indestructible walls.{2}",
                Settings.Default.MapWidth,
                Settings.Default.MapHeight,
                Environment.NewLine);
            rules.AppendLine("The initial state of the map will be as follows:");
            rules.AppendLine("```javascript");
            rules.AppendLine("###################");
            rules.AppendLine("# {OpponentName}  #");
            rules.AppendLine("# Round:   0      #");
            rules.AppendLine("# Kills: 0        #");
            rules.AppendLine("# Lives: 3        #");
            rules.AppendLine("# Missiles: 0/1   #");
            rules.AppendLine("################### <-- If an alien reaches this row your opponent loses");
            rules.AppendLine("#                 # <-- Opponent's building row");
            rules.AppendLine("#       VVV       #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#          x  x  x# <-- Your aliens spawn here and move up");
            rules.AppendLine(
                "#                 # <-- Middle row");
            rules.AppendLine("#          x  x  x# <-- Your opponent's aliens spawn here and move down");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("#                 #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("# ---         --- #");
            rules.AppendLine("#       AAA       #");
            rules.AppendLine("#                 # <-- Your building row");
            rules.AppendLine("################### <-- If an alien reaches this row you lose");
            rules.AppendLine("# Missiles: 0/1   #");
            rules.AppendLine("# Lives: 3        #");
            rules.AppendLine("# Kills: 0        #");
            rules.AppendLine("# Round:   0      #");
            rules.AppendLine("# {YourName}      #");
            rules.AppendLine("###################");
            rules.AppendLine("```");
            rules.AppendLine();
            rules.AppendLine("You can download an example of the map file [here](map.txt).");
            rules.AppendLine();

            rules.AppendLine("The map legend is as follows:");
            rules.AppendLine("```javascript");
            rules.AppendLine("AAA: Your Ship");
            rules.AppendLine("VVV: Opponent's Ship");
            rules.AppendLine("XXX: Alien Factory");
            rules.AppendLine("MMM: Missile Controller");
            rules.AppendLine("#: Wall");
            rules.AppendLine("-: Shield");
            rules.AppendLine("x: Alien");
            rules.AppendLine("|: Alien Bullet");
            rules.AppendLine("!: Your Missiles");
            rules.AppendLine("i: Opponent's Missiles");
            rules.AppendLine("```");
            rules.AppendLine();

            rules.AppendLine("## State");
            rules.Append("In addition to the map output, the whole game state is also output as a ");
            rules.AppendLine("JSON file every round. You can get an example of the JSON file [here](state.json).");
            rules.AppendLine();

            rules.AppendLine("## Technical");
            rules.AppendLine("The technical requirements for your entry are as follows:");
            rules.AppendLine(
                "1. Your program must be written for one of the following environments (sample bots are available on [Github](https://github.com/EntelectChallenge)):");
            rules.AppendLine("\t1. C# (.NET 4.5)");
            rules.AppendLine("\t2. Java 7 or 8");
            rules.AppendLine("\t3. Node.js 0.12.x");
            rules.AppendLine("\t4. Python 2.7.x");

            rules.AppendLine("2. The operating system that will be used for the challenge is Windows Server 2012.");

            rules.AppendLine("3. Your program must include a compile.bat file in its root that can build the program.");

            rules.AppendLine("4. Your program must include a run.bat file in its root that can run the program.");

            rules.Append("5. The bot is required to accept a single string parameter: the parameter is the ");
            rules.Append("relative path where the match runner will output the game state files and where ");
            rules.AppendLine("the bot must write its move file:");
            rules.AppendLine("\t1. map.txt");
            rules.AppendLine("\t2. state.json");
            rules.AppendLine("\t3. move.txt (written by bot)");

            rules.AppendFormat("6. The bot is required to produce an output file in **{0} seconds**.",
                Settings.Default.TurnDurationSeconds);
            rules.AppendFormat("\t1. If the bot takes longer than **{0} seconds** it will be killed, ",
                Settings.Default.TurnDurationSeconds);
            rules.AppendFormat("forfeiting the opportunity to make a move.{0}", Environment.NewLine);
            rules.AppendLine(
                "\t2. The bot is allowed to store persistent state across rounds. Any persistent state or data that you wish to keep across rounds will be allowed to be stored in temporary disk files.");

            rules.Append("7. The output of your program must be a single text file (move.txt) containing ");
            rules.AppendLine(" one line: the command you wish your ship to execute this round. Valid options are:");
            rules.AppendLine("\t* Nothing");
            rules.AppendLine("\t* MoveLeft");
            rules.AppendLine("\t* MoveRight");
            rules.AppendLine("\t* Shoot");
            rules.AppendLine("\t* BuildAlienFactory");
            rules.AppendLine("\t* BuildMissileController");
            rules.AppendLine("\t* BuildShield");

            rules.AppendLine("8. Your entry must be composed of the following:");
            rules.AppendLine("\t1. Your entry must contain a source folder.");
            rules.AppendLine(
                "\t2. Your entry must contain a compile.bat file that we can use to automatically compile your program.");
            rules.AppendLine("\t3. Your entry must contain a run.bat file for executing the bot.");
            rules.Append(
                "\t4. Your entry must contain a readme.txt file which includes instructions for building your project ");
            rules.AppendLine("and a brief description of your project structure and strategy.");
            rules.AppendLine("\t5. Any executable binaries needed for the Windows platform (Windows Server 2012).");
            rules.AppendLine("\t6. Ensure that your submissions are less than 5MB in size.");
            rules.AppendLine("\t7. Do not hard-code any paths to files or dependencies.");
            rules.AppendLine("\t8. The compile.bat file should fetch any dependencies (eg. Maven, NPM or NuGet).");

            return rules.ToString();
        }

        public IMatch GetFlippedCopyOfMatch()
        {
            var flippedEntities = new Dictionary<int, Entity>();
            var flipper = new CoordinateFlipper(Map.Width, Map.Height);
            var flipped = new Match
            {
                BuildingsAvailable = BuildingsAvailable,
                RoundLimit = RoundLimit,
                RoundNumber = RoundNumber
            };

            flipped.Players.Clear();
            flipped.Map = Map.CopyAndFlip(Map, flipper, flippedEntities);
            flipped.Players.Add(Player.CopyAndFlip(GetPlayer(2), flipper, flippedEntities));
            flipped.Players.Add(Player.CopyAndFlip(GetPlayer(1), flipper, flippedEntities));

            return flipped;
        }

        public static Match GetInstance()
        {
            if (_instance != null) return _instance;

            _instance = new Match();
            _instance.StartNewGame();
            return _instance;
        }

        public static void SetInstance(Match instance)
        {
            _instance = instance;
        }

        public void StartNewGame(bool aliensDisabled = false)
        {
            Players.Clear();
            Players.Add(new Player(1));
            Players.Add(new Player(2));

            if (aliensDisabled)
            {
                Players[0].AlienManager.Disabled = true;
                Players[1].AlienManager.Disabled = true;
            }

            Map = new Map(Settings.Default.MapWidth, Settings.Default.MapHeight);

            UpdateRespawnPlayersIfNecessary();
            Map.UpdateEntities();

            ShieldFactory.BuildInitial(1);
            ShieldFactory.BuildInitial(2);
        }

        private void UpdateAlienManagers()
        {
            foreach (var player in Players)
            {
                if (RoundNumber == Settings.Default.AlienWaveSizeBumpRound)
                {
                    player.AlienWaveSize += Settings.Default.AlienWaveSizeBump;
                }

                player.UpdateAlienManager();
            }
        }

        private void UpdateRespawnPlayersIfNecessary()
        {
            foreach (var player in Players)
            {
                player.RespawnPlayerShipIfNecessary();
            }
        }

        public Player GetPlayer(int playerNumber)
        {
            return Players[playerNumber - 1];
        }
    }

    public class BuildingInfo
    {
        public BuildingInfo(ShipCommand command, EntityType type, int cost)
        {
            Command = command;
            Type = type;
            Cost = cost;
        }

        [JsonConverter(typeof (StringEnumConverter))]
        public ShipCommand Command { get; private set; }

        [JsonConverter(typeof (StringEnumConverter))]
        public EntityType Type { get; private set; }

        public int Cost { get; private set; }
    }
}