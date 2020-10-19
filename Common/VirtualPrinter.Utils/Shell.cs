using System;
using System.IO;
using System.Text;
using System.Threading;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core;
using VirtualPrinter.Logging;

namespace VirtualPrinter.Utils
{
    public class Shell : IShell
    {
        private readonly IVirtualPrinterLogger<Shell> _logger = new VirtualPrinterLogger<Shell>();

        public void WriteIniEntry(string section, string key, string value, string iniFilePath)
        {
            Win32Sys.WritePrivateProfileString(section, key, value, iniFilePath);
        }

        public T ReadIniEntry<T>(string section, string key, string iniFilePath)
        {
            var buffer = new StringBuilder(64 * 1024);
            Win32Sys.GetPrivateProfileString(section, key, string.Empty, buffer, buffer.Capacity, iniFilePath);
            var value = buffer.ToString().Trim();
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void Execute(IJobInfo job, ISessionInfo session, string exe, string args)
        {
            var thr = new Thread
            (
                () =>
                {
                    try
                    {
                        if (!File.Exists(exe))
                            throw new FileNotFoundException(exe);
#if DEBUG
                        System.Diagnostics.Process.Start(exe, args);
#else
                        StartProcessAsUser(job, session, exe, args);
#endif
                    }
                    catch (Exception exception)
                    {
                        LogError(exception, "Failed to create process");
                    }
                }
            );

            thr.Start();
        }

        public bool FileExists(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("The path may not be null or empty.");
            }

            return File.Exists(path);
        }

        private void StartProcessAsUser([NotNull]IJobInfo job, [NotNull]ISessionInfo session, string exe, string args)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var cmd = $@"""{exe}"" {args}";
            var id = session.Id;
            var user = job.MachineName.TrimStart('\\') + '\\' + job.UserName;
            LogDebug($"Executing '{cmd}' for '{user}' ({id})...");
            Win32Sys.CreateProcessAsUser(id, user, cmd);
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        private void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}
