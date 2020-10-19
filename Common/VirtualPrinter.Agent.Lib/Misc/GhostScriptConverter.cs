using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core;
using VirtualPrinter.Agent.Lib.Model;
using VirtualPrinter.Logging;

namespace VirtualPrinter.Agent.Lib.Misc
{
    public class GhostScriptConverter : IPostScriptConverter
    {
        private const string GsWin64 = "gswin64c.exe";
        private const string GsWin32 = "gswin32c.exe";

        [NotNull]
        private readonly IVirtualPrinterLogger<GhostScriptConverter> _logger;

        [NotNull]
        private readonly IRegistryRepository _registryRepository;

        [NotNull]
        private readonly IShell _shell;

        public event EventHandler<IJob> ProgressInitialized;
        public event EventHandler<IJob> ProgressFinished;
        public event EventHandler<ProgressUpdateArgs> ProgressUpdate;

        public GhostScriptConverter
        (
            [NotNull]IVirtualPrinterLogger<GhostScriptConverter> logger,
            [NotNull]IRegistryRepository registryRepository,
            [NotNull]IShell shell
        )
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (registryRepository == null)
            {
                throw new ArgumentNullException(nameof(registryRepository));
            }

            if (shell == null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            _logger = logger;
            _registryRepository = registryRepository;
            _shell = shell;
        }

        public void Convert(IJob job, string target, PostScriptRenderOptions options)
        {
            var ghostScriptExe = GetGhostScriptPath();

            if (ghostScriptExe == null)
            {
                throw new PostScriptConversionException("GhostScript not found. Please place local variable.");
            }

            ProgressInitialized?.Invoke(this, job);

            var pdfTarget = target + ".pdf";
            if (options.PdfOptions.Enabled)
            {
                Convert(job, ghostScriptExe, GetArgumentsForPdfConversion(job.RawDataPath, pdfTarget, options));

                if (!_shell.FileExists(pdfTarget))
                {
                    throw new PostScriptConversionException("Postscript conversion failed after output.");
                }
            }

            if (options.TiffOptions.Enabled)
            {
                var tiffTarget = target + ".tif";

                Convert(job, ghostScriptExe, GetArgumentsForTiffConversion(job.RawDataPath, tiffTarget, options));
                Convert(job, ghostScriptExe, GetArgumentsForPdfConversion(job.RawDataPath, pdfTarget, options));

                if (!_shell.FileExists(tiffTarget))
                {
                    throw new PostScriptConversionException("Postscript conversion failed after output.");
                }
            }

            ProgressFinished?.Invoke(this, job);
        }

        [CanBeNull]
        private string GetGhostScriptPath()
        {
            return GetGhostScriptPath(GsWin64) ?? GetGhostScriptPath(GsWin32);
        }

        [CanBeNull]
        private string GetGhostScriptPath(string execName)
        {
            if (!_registryRepository.TryGetGhostscriptPath(out var path))
            {
                return null;
            }

            var ghostScriptBinPath = Path.Combine(path, "bin");
            var fullPath = Path.Combine(ghostScriptBinPath, execName);

            return _shell.FileExists(fullPath) ? fullPath : null;
        }

        [NotNull]
        private static string GetArgumentsForPdfConversion(string source, string target, PostScriptRenderOptions options)
        {
            const string initialArguments = "-q -P- -dSAFER -dNOPAUSE -dBATCH -dNoCancel -sDEVICE=pdfwrite";
            var finalArguments = $"-sOutputFile=\"{target}\" \"{source}\"";

            var optionalArguments = "";

            if (options.PdfOptions.Archivable)
            {
                optionalArguments += "-sColorConversionStrategy=/RGB -dUseCIEColor -dPDFACompatibilityPolicy=2";
            }

            if (options.UserRenderDpi != null && options.UserRenderDpi > 0)
            {
                optionalArguments += "-r" + options.UserRenderDpi.Value;
            }

            return $"{initialArguments} {optionalArguments} {finalArguments}";
        }

        [NotNull]
        private static string GetArgumentsForTiffConversion(string source, string target, PostScriptRenderOptions options)
        {
            const string initialArguments = "-q -P- -dSAFER -dNOPAUSE -dBATCH -sDEVICE=tiff12nc -sCompression=lzw";
            var finalArguments = $"-sOutputFile=\"{target}\" \"{source}\"";

            var optionalArguments = "-dTextAlphaBits=4 ";

            if (options.UserRenderDpi != null && options.UserRenderDpi > 0)
            {
                optionalArguments += "-r" + options.UserRenderDpi.Value;
            }
            else
            {
                optionalArguments += "-r300";
            }

            return $"{initialArguments} {optionalArguments} {finalArguments}";
        }

        private void Convert(IJob job, string gsExe, string gsArguments)
        {
            LogDebug("Starting ps conversion ...\n"+gsExe + " " + gsArguments);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gsExe,
                    Arguments = gsArguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                var readLine = process.StandardOutput.ReadLine();
                var progress = ParseProgress(readLine);

                if (progress == null)
                {
                    continue;
                }

                ProgressUpdate?.Invoke(this, new ProgressUpdateArgs(job, progress.Value));
            }
        }

        private static uint? ParseProgress([CanBeNull]string line)
        {
            // Example output:
            // %%[Page: 49]%%
            if (line == null)
            {
                return null;
            }

            var groups = Regex.Match(line, @"%%\[Page:(.*)\]%%").Groups;
            if (groups.Count < 2)
            {
                return null;
            }

            if (!uint.TryParse(groups[1].Value, out var result))
            {
                return null;
            }

            return result;
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
}
