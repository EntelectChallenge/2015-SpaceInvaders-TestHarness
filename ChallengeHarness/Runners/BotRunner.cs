using System;
using System.Diagnostics;
using System.IO;
using ChallengeHarness.Properties;
using ChallengeHarnessInterfaces;
using Newtonsoft.Json;

namespace ChallengeHarness.Runners
{
    public class BotRunner
    {
        private readonly Stopwatch _botTimer;
        private readonly MemoryStream _inMemoryLog;
        private readonly StreamWriter _inMemoryLogWriter;
        private readonly string _mapFilename;
        private readonly string _moveFilename;
        private readonly string _stateFilename;
        private readonly string _workingPath;
        private string _processName;

        public BotRunner(int playerNumber, String workingPath, String executableFilename)
        {
            _inMemoryLog = new MemoryStream();
            _inMemoryLogWriter = new StreamWriter(_inMemoryLog);
            _botTimer = new Stopwatch();

            _workingPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + workingPath;
            _mapFilename = Path.Combine(_workingPath, Settings.Default.BotOutputFolder, Settings.Default.MapFilename);
            _stateFilename = Path.Combine(_workingPath, Settings.Default.BotOutputFolder, Settings.Default.StateFilename);
            _moveFilename = Path.Combine(_workingPath, Settings.Default.BotOutputFolder, Settings.Default.MoveFileName);
            _processName = Path.Combine(_workingPath, executableFilename);

            BotLogFilename = Path.Combine(_workingPath, Settings.Default.BotOutputFolder,
                Settings.Default.BotLogFilename);

            PlayerNumber = playerNumber;
            PlayerName = LoadBotName();

            CreateOutputDirectoryIfNotExists();
            ClearAllOutputFiles();
        }

        public string BotLogFilename { get; private set; }
        public int PlayerNumber { get; private set; }
        public string PlayerName { get; private set; }

        private string LoadBotName()
        {
            BotMeta metaData;
            try
            {
                string textData;
                using (
                    var file =
                        new StreamReader(_workingPath + Path.DirectorySeparatorChar +
                                         Settings.Default.BotMetaDataFilename))
                {
                    textData = file.ReadToEnd();
                }

                metaData = JsonConvert.DeserializeObject<BotMeta>(textData);
            }
            catch
            {
                return "Player " + PlayerNumber;
            }

            return metaData.NickName;
        }

        public string GetMove(MatchRender rendered)
        {
            OutputFile(_mapFilename, rendered.Map);
            OutputFile(_stateFilename, rendered.State);

            _botTimer.Reset();
            _botTimer.Start();

            var process = CreateProcess();
            AddEventHandlersToProcess(process);
            StartProcess(process);
            string result = HandleProcessResponse(process);

            AppendLogs();
            ClearRoundFiles();

            return result;
        }

        private void CreateOutputDirectoryIfNotExists()
        {
            var outputFolder = Path.Combine(_workingPath, Settings.Default.BotOutputFolder);
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
        }

        public void OutputAppendLog(string logEntry)
        {
            _inMemoryLogWriter.WriteLine(logEntry);
        }

        private void AppendLogs()
        {
            _inMemoryLogWriter.Flush();
            Debug.WriteLine("Saving player " + PlayerNumber + " bot log to: " + BotLogFilename);
            using (var file = File.Open(BotLogFilename, FileMode.Append))
            {
                _inMemoryLog.WriteTo(file);
            }

            _inMemoryLog.SetLength(0);
        }

        private void OutputFile(string filename, string value)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (var file = new StreamWriter(filename))
            {
                file.WriteLine(value);
            }
        }

        private void ClearAllOutputFiles()
        {
            File.Delete(_mapFilename);
            File.Delete(_stateFilename);
            File.Delete(_moveFilename);
            File.Delete(BotLogFilename);
        }

        private void ClearRoundFiles()
        {
            File.Delete(_mapFilename);
            File.Delete(_stateFilename);
            File.Delete(_moveFilename);
        }

        private Process CreateProcess()
		{
			if (!File.Exists (_processName)) {
				throw new FileNotFoundException ("Bot process file '" + _processName + "' not found.");
			}

			var arguments = " \"" + Settings.Default.BotOutputFolder + "\"";
			var processName = _processName;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				arguments = _processName + " " + arguments;
				processName = "/bin/bash";
			}

            return new Process
            {
                StartInfo =
                {
                    WorkingDirectory = _workingPath,
					FileName = processName,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
        }

        private void AddEventHandlersToProcess(Process p)
        {
            DataReceivedEventHandler h = (sender, args) =>
            {
                if (!String.IsNullOrEmpty(args.Data))
                {
                    _inMemoryLogWriter.WriteLine(args.Data);
                }
            };
            p.OutputDataReceived += h;
            p.ErrorDataReceived += h;
        }

        private void StartProcess(Process p)
        {
            using (ChangeErrorMode newErrorMode = new ChangeErrorMode(ChangeErrorMode.ErrorModes.FailCriticalErrors | ChangeErrorMode.ErrorModes.NoGpFaultErrorBox))
            {
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                var didExit = p.WaitForExit(Settings.Default.MoveTimeoutSeconds * 1000);
                _botTimer.Stop();

                if (!didExit)
                {
                    if (!p.HasExited)
                        p.Kill();
                    OutputAppendLog(String.Format("[GAME]\tBot {0} timed out after {1} ms.", PlayerName,
                        _botTimer.ElapsedMilliseconds));
                    OutputAppendLog(String.Format("[GAME]\tKilled process {0}.", _processName));
                }
                else
                {
                    OutputAppendLog(String.Format("[GAME]\tBot {0} finished in {1} ms.", PlayerName,
                        _botTimer.ElapsedMilliseconds));
                }

                if ((didExit) && (p.ExitCode != 0))
                {
                    OutputAppendLog(String.Format("[GAME]\tProcess exited with non-zero code {0} from player {1}.",
                        p.ExitCode, PlayerName));
                }
            }
        }

        private string HandleProcessResponse(Process p)
        {
            if (!File.Exists(_moveFilename))
            {
                OutputAppendLog("[GAME]\tNo output file from player " + PlayerName);
                return null;
            }

            var fileLines = File.ReadAllLines(_moveFilename);
            return fileLines.Length > 0 ? fileLines[0] : null;
        }
    }
}