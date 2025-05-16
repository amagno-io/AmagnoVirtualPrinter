using System.Threading;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IShell
    {
        void WriteIniEntry(string section, string key, string value, string iniFilePath);

        [NotNull]
        T ReadIniEntry<T>(string section, string key, string iniFilePath);

        Thread Execute(IJobInfo job, ISessionInfo session, string exe, string args);

        bool FileExists([NotNull]string path);
    }
}