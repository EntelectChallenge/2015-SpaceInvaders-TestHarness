# 2015 Space Invaders Test Harness
The current release of the test harness is version 1.0.0.

For more information about the challenge see the [Challenge website](http://challenge.entelect.co.za/) .

The purpose of this project to allow contestants to test their bots on their local machines. The test harness contains the logic for processing moves and running a match between two bots. This project can also be used to get a better understanding of the rules and to help debug your bot.

Improvements and enhancements may be made to the test harness code over time, but the rules should remain stable.

The test harness has been made available to the community for peer review and bug fixes, so if you find any bugs or have any concerns, please e-mail challenge@entelect.co.za, discuss it with us on the [Challenge forum](http://forum.entelect.co.za/) or submit a pull request on Github.

## Usage
The easiest way to start using the test harness is to download the [binary release zip](https://github.com/EntelectChallenge/2015-SpaceInvaders-TestHarness/releases/download/1.0.0/2015-TestHarness-1.0.0-Windows.zip). You will also need the .NET framework if you don't have it installed already - you can get the offline installer for [.NET Framework 4.5.1 here](http://www.microsoft.com/en-za/download/details.aspx?id=40779).

Once you have installed .NET and downloaded the binary release zip file, extract it and open a new Command Prompt in the test harness folder.

We have bundled the compiled C# sample bot with the harness in the player1 and player2 folders, so at this point you could simply run `SpaceInvadersDuel.exe` to see two random bots play a match.

Once you have written your own bot you can override one of the player folders with your bot or you can use the command line arguments to specify the bots that should be run. You can see the available command line arguments by running `SpaceInvadersDuel.exe --help`:
```powershell
SpaceInvadersDuel 1.0.0.0
Copyright c Microsoft 2015

  -o, --one      (Default: player1) Relative path to the folder containing the
                 player one bot

  -t, --two      (Default: player2) Relative path to the folder containing the
                 player two bot

  -r, --rules    (Default: False) Prints out the rules and saves them in
                 markdown format to rules.md

  --help         Display this help screen.
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
### v1.0.0 - 09/04/2015
Changelog:
* Initial release.

Code coverage:
* Line: 98%
* Branch: 89.8%
