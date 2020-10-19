using System.Printing;

namespace VirtualPrinter.Agent.Core
{
    public interface IJobInfo
    {
        int JobId { get; set; }
        string Name { get; set; }
        string DomainName { get; set; }
        string MachineName { get; set; }
        string UserName { get; set; }
        PrintJobStatus Status { get; set; }
        string DeviceName { get; set; }
    }
}