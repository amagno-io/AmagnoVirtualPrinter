using System;
using System.Diagnostics;
using System.IO;
using System.Printing;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.Utils;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Delivery
{
    /// <summary>
    /// Redirects instructions to Ghostscript
    /// </summary>
    public class GhostScriptRedirector
    {
        private const string GsWin64 = "gswin64c.exe";
        private const string GsWin32 = "gswin32c.exe";

        [NotNull]
        private readonly PrintQueue _queue;

        public GhostScriptRedirector([NotNull]PrintQueue queue)
        {
            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }

            _queue = queue;
        }

        /// <summary>
        /// Starts the Ghostscript process for the specified <paramref name="file"/>
        /// </summary>
        /// <param name="file">The .redirect file located in the PrinterOutput directory</param>
        public void Redirect([NotNull]string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var logger = new VirtualPrinterLogger<GhostScriptRedirector>();
            var ghostScriptExe = GetGhostScriptPath();

            if (ghostScriptExe == null)
            {
                logger.Error("Can not find Ghostscript.");
                throw new FileNotFoundException("Can not find Ghostscript.");
            }

            // More details about the arguments can be find at https://www.ghostscript.com/doc/current/Use.htm
            var ghostScriptArguments = $"-dPrinted -dBATCH -dNOPAUSE -dNoCancel -dNOSAFER -q -dNumCopies=1 -sDEVICE=mswinpr2 -sOutputFile=\"%printer%{ _queue.FullName}\" \"{file}\"";
            logger.Info("Try to start the process {ghostScriptExe} with the following arguments: {ghostScriptArguments}.", ghostScriptExe, ghostScriptArguments);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = ghostScriptExe,
                Arguments = ghostScriptArguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            try
            {
                using(var process = Process.Start(processStartInfo))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception, "Failed to start {ghostScriptExe} with the following arguments: {ghostScriptArguments}", ghostScriptExe, ghostScriptArguments);
            }
        }

        [CanBeNull]
        private string GetGhostScriptPath()
        {
            return GetGhostScriptPath(GsWin64) ?? GetGhostScriptPath(GsWin32);
        }

        [CanBeNull]
        private string GetGhostScriptPath(string execName)
        {
            if (!new RegistryRepository().TryGetGhostscriptPath(out var path))
            {
                return null;
            }

            var ghostScriptBinPath = Path.Combine(path, "bin");
            var fullPath = Path.Combine(ghostScriptBinPath, execName);

            return File.Exists(fullPath) ? fullPath : null;
        }
    }
}