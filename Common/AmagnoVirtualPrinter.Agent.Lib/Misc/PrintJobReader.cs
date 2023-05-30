using System;
using System.Collections.Generic;
using System.Printing;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class PrintJobReader
    {
        [ItemNotNull]
        public static IEnumerable<IJobInfo> GetCurrentPrintJobs(string printerName)
        {
            using (var server = new LocalPrintServer())
            {
                using (var queue = server.GetPrintQueue(printerName))
                {
                    using (var jobs = queue.GetPrintJobInfoCollection())
                    {
                        foreach (var job in jobs)
                        {
                            using (job)
                            {
                                var id = job.JobIdentifier;
                                var machine = server.Name;
                                var domain = Environment.UserDomainName;
                                var user = job.Submitter;
                                var name = job.Name;
                                yield return new JobInfo
                                {
                                    JobId = id,
                                    Name = name,
                                    DomainName = domain,
                                    MachineName = machine,
                                    UserName = user,
                                    Status = job.JobStatus,
                                    DeviceName = queue.Name
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}