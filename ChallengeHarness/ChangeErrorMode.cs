using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ChallengeHarness.Runners
{
    /// <summary>
    /// Set the window's error mode for this process. Be very careful with this --- this is a process wide
    /// setting, so in a multi-threaded situation it can do crazy things!
    /// Code pulled from: http://msdn.microsoft.com/en-us/magazine/cc163866.aspx
    /// </summary>
    public struct ChangeErrorMode : IDisposable
    {
        /// <summary>
        /// Error modes that can be enabled or disabled.
        /// </summary>
        [Flags]
        public enum ErrorModes
        {
            Default = 0x0,
            FailCriticalErrors = 0x1,
            NoGpFaultErrorBox = 0x2,
            NoAlignmentFaultExcept = 0x4,
            NoOpenFileErrorBox = 0x8000
        }

        private int _oldMode;

        /// <summary>
        /// Set the new error mode. When Dispose is called it will reset
        /// to the original value (use the "using" statement).
        /// </summary>
        /// <param name="mode"></param>
        public ChangeErrorMode(ErrorModes mode)
        {
            _oldMode = SetErrorMode((int)mode);
        }

        void IDisposable.Dispose() { SetErrorMode(_oldMode); }

        [DllImport("kernel32.dll")]
        private static extern int SetErrorMode(int newMode);
    }
}