using System.Printing;

namespace VirtualPrinter.Agent.Core
{
    public struct JobInfo : IJobInfo
    {
        public int JobId { get; set; }
        public string Name { get; set; }
        public string DomainName { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public PrintJobStatus Status { get; set; }
        public string DeviceName { get; set; }
    }
}