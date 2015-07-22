# 2015 Space Invaders Test Harness
The current release of the test harness is version 1.0.6.

For more information about the challenge see the [Challenge website](http://challenge.entelect.co.za/) .

The purpose of this project to allow contestants to test their bots on their local machines. The test harness contains the logic for processing moves and running a match between two bots. This project can also be used to get a better understanding of the rules and to help debug your bot.

Improvements and enhancements may be made to the test harness code overs time, but the rules should remain stable.

The test harness has been made available to the community for peer review and bug fixes, so if you find any bugs or have any concerns, please e-mail challenge@entelect.co.za, discuss it with us on the [Challenge forum](http://forum.entelect.co.za/) or submit a pull request on Github.

## Usage
The easiest way to start using the test harness is to download the [binary release zip](https://github.com/EntelectChallenge/2015-SpaceInvaders-TestHarness/releases/download/1.0.6/2015-TestHarness-1.0.6-Windows.zip). You will also need the .NET framework if you don't have it installed already - you can get the offline installer for [.NET Framework 4.5.1 here](http://www.microsoft.com/en-za/download/details.aspx?id=40779).

Once you have installed .NET and downloaded the binary release zip file, extract it and open a new Command Prompt in the test harness folder.

We have bundled the compiled C# sample bot with the harness in the player1 and player2 folders, so at this point you could simply run `SpaceInvadersDuel.exe` to see two random bots play a match.

Once you have written your own bot you can override one of the player folders with your bot or you can use the command line arguments to specify the bots that should be run. You can see the available command line arguments by running `SpaceInvadersDuel.exe --help`:
```powershell
SpaceInvadersDuel 1.0.6.0                                                     
Copyright c Microsoft 2015                                                    
                                                                              
  -o, --one          (Default: player1) Relative path to the folder containing
                     the player one bot                                       
                                                                              
  -t, --two          (Default: player2) Relative path to the folder containing
                     the player two bot                                       
                                                                              
  -r, --rules        (Default: False) Prints out the rules and saves them in  
                     markdown format to rules.md                              
                                                                              
  -q, --quiet        (Default: False) Disables console logging - logs will onl
                     be written to files.                                     
                                                                              
  -s, --scrolling    (Default: False) Forces scrolling console log output,    
                     which shouldn't crash when running the harness from      
                     another application.                                     
                                                                              
  -l, --log          (Default: ) Relative path where you want the match replay
                     log files to be output (instead of the default           
                     Replays/{matchNumber}).                                  
                                                                              
  --help             Display this help screen.                                
```

So for example you can do something like this to run your bot against the bundled example bot: `SpaceInvadersDuel.exe -o ../mybot -t player2`.

## Compiling
The test harness is a C&#35; project, so you will need to download and install [Visual Studio Express 2013](http://www.microsoft.com/en-us/download/details.aspx?id=44914) if you intend to work on it.

Once you have installed Visual Studio, open the `SpaceInvadersDuel.sln` solution file at the root of the project and select Build -> Build Solution from the menus to compile the project. This should automatically fetch the library dependencies using NuGet.

### Tests
We have written a number of automated tests to ensure that the game logic and rules have been implemented correctly - if you make any changes to the test harness you should run the tests to ensure that everything is still working correctly before submitting a pull request. The easiest way to run the tests is to select Test -> Run -> All Tests from the menu in Visual Studio.

If you add a new feature you should add tests to cover it. After compiling the project you can run the coverage report yourself as follows:
* Open a new Command Prompt
* Change to the project directory
* `cd SpaceInvadersTest\bin\Debug`
* `coverage && report`

Provided all the tests pass, you should find the coverage report in `SpaceInvadersTest\bin\debug\coverage\index.html`.

## Release Notes
### v1.0.6 - 22/07/2015
* Bugs fixed:
  * Simultaneous collisions of missile chains now resolve intuitively.
  * SetErrorMode should no longer crash the harness on Linux / Mac.

The following unintuitive missile behaviours have been fixed:
```
BEFORE
...    ...               ...    ...
.i.    ...               .i.    ...
.i.    ...               .i.    ...
.!. -> ...         AND   .!. -> ...
.!.    ...               ...    ...
...    ...               ...    ...

AFTER
...    ...    ...        ...    ...
.i.    ...    ...        .i.    ...
.i.    .i.    ...        .i.    .i.
.!. -> .!. -> ...  AND   .!. -> ...
.!.    ...    ...        ...    ...
...    ...    ...        ...    ...
```

### v1.0.5 - 11/07/2015
* Bugs fixed:
  * Players now start with the correct 3 lives instead of only 2 (thanks [rm2k](https://github.com/rm2k)).
  * Improved layout of kills on basic map output and several fields on advanced map output (thanks [jlaihong](https://github.com/jlaihong)).
  * Simultaneous collisions of missiles and bullets now destroy all involved (thanks [DeanWookey](https://github.com/DeanWookey)).
  * Player ships moving into aliens will now die (thanks [DeanWookey](https://github.com/DeanWookey)).
  * Prevented alien shots fired in the last row from destroying the wall in front of the alien (thanks [rm2k](https://github.com/rm2k)).

### v1.0.4 - 01/06/2015
* Bugs fixed:
  * Suppressed exception dialogs popped up when C&#35; bots had uncaught exceptions.
* Features:
  * Detailed map render containing additional details (thanks [jlaihong](https://github.com/jlaihong)).

### v1.0.3 - 22/05/2015
* Bugs fixed:
  * Fixed alien randomness: both AlienManagers used separate Random classes with the same seed, so randomness was the same for both players (thanks [AttieG](https://github.com/AttieG)).
  * Fixed application still crashing when not running a in a console (ScrollingLogger did a Console.clear() on creation).
* Minor features:
  * Added a command line option to specify a folder that the replay should be put into (-l or --log).

### v1.0.2 - 10/05/2015
* Bugs fixed
  * Fixed some exceptions related to bot timeouts and killing the bot process (thanks Bernard Haeusermann)
  * Ship spawn logic now deducts a life before spawn / collision checking which could have caused the life not to be deducted if spawning on an alien or bullet (thanks Jansen du Plessis).
  * Aliens hitting the back wall now destroy the correct player (thanks Jansen du Plessis).
  * Improved tests for aliens hitting the back wall a bit.
  * Fixed run.sh newline characters and gave hints on installing dependencies (thanks Marius Kruger).
  * Got rid of compiler warnings (thanks Bernhard Haeusermann).
* Minor features:
  * Added a command line option to force scrolling console logging (-s or --scrolling).
  * Added a command line option to disable console logging (-q or --quiet).

### v1.0.1 - 24/04/2015
* Bugs fixed:
  * A life is now deducted on ship spawning instead of player death which should give a consistent 1 ship & 3 lives, regardless of how they are used.
  * Player.CopyAndFlip now also copies the:
    * AlienFactory
    * MissileController (thanks to @leppie)
    * Ship
  * Should no longer crash if the terminal is too small to render the game - instead falls back on scrolling output.
  * Now correctly changes the process name to /bin/bash on Linux.
  * Fixed MoveNotOnMapException that sometimes happened when spawning aliens (thanks to @leppie).
* Minor features:
  * On Linux the TestHarness will try to use run.sh instead of run.bat. Example files have been added to all sample bots and they will also be tagged as version 1.0.1.

### v1.0.0 - 09/04/2015
Changelog:
* Initial release.

Code coverage:
* Line: 98%
* Branch: 89.8%
