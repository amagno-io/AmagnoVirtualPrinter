using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.Agent.Lib.Misc
{
    public class Job : IJob
    {
        public string RawDataPath { get; set; }

        public string IniDataPath { get; set; }

        public IJobInfo JobInfo { get; set; }

        public ISessionInfo SessionInfo { get; set; }
    }
}