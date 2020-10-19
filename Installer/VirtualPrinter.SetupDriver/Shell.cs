using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using VirtualPrinter.Logging;

namespace VirtualPrinter.SetupDriver
{
    internal class Shell
    {
        private static readonly string WinFolder;

        private static readonly IVirtualPrinterLogger<Shell> Logger = new VirtualPrinterLogger<Shell>();

        static Shell()
        {
            WinFolder = Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";
        }

        [NotNull]
        internal static string GetPrintInf()
        {
            try
            {
                var winFolder = Environment.GetEnvironmentVariable("windir") ?? @"C:\Windows";
                return Path.Combine(winFolder, "inf", "ntprint.inf");
            }
            catch (Exception exception)
            {
                LogError(exception, "Cannot get PrintInf");
                throw;
            }
        }

        [NotNull]
        internal static string GetPrintVbs()
        {
            try
            {
                var printScripts = Path.Combine(WinFolder, "System32", "Printing_Admin_Scripts");
                return Directory.GetFiles(printScripts, "*port.vbs", SearchOption.AllDirectories).First();
            }
            catch (Exception exception)
            {
                LogError(exception, "Cannot get PrintVbs");
                throw;
            }
        }

        internal static void Execute([NotNull]string exe, [NotNull]string args)
        {
            if (exe == null)
            {
                throw new ArgumentNullException(nameof(exe));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            try
            {
                Console.WriteLine(exe + " " + args);
                using(var proc = Process.Start(exe, args))
                {
                    proc?.WaitForExit();
                }
            }
            catch (Exception exception)
            {
                LogError(exception, "Cannot execute {exe} with the following args: {args}", exe, args);
            }
        }

        private static void LogError(Exception exception, string message, params object[] args)
        {
            Logger.Error(exception, message, args);
        }
    }
}