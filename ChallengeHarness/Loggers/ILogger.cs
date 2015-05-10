using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChallengeHarnessInterfaces;

namespace ChallengeHarness.Loggers
{
    public interface ILogger
    {
        void Log(MatchRender rendered);

        void Log(MatchSummary summary);
    }
}
