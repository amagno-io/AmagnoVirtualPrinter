using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class Job : IJob
    {
        public string RawDataPath { get; set; }

        public string IniDataPath { get; set; }

        public IJobInfo JobInfo { get; set; }

        public ISessionInfo SessionInfo { get; set; }
    }
}