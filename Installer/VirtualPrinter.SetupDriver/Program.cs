using System;
using System.IO;

using JetBrains.Annotations;

using VirtualPrinter.Logging;
using VirtualPrinter.Utils;

using static VirtualPrinter.SetupDriver.Windows;
using static VirtualPrinter.SetupDriver.Defaults;

namespace VirtualPrinter.SetupDriver
{
    internal class Program
    {
        private const string InstallCmd = "install";
        private const string TestCmd = "test";
        private const string ConfigCmd = "config";
        private const string UninstallCmd = "uninstall";

        private static readonly IVirtualPrinterLogger<Program> Logger = new VirtualPrinterLogger<Program>();

        private static void Main([CanBeNull]string[] args)
        {
            if (args == null || args.Length < 1)
            {
                NotUseful();
                return;
            }

            switch (args[0])
            {
                case InstallCmd:
                {
                    if (args.Length < 2)
                    {
                        NotUseful();
                        return;
                    }

                    switch (args[1].ToLower())
                    {
                        case "xps":
                            try
                            {
                                AddPrinterPort(PrinterPort, "127.0.0.1", 9101);
                                var isWin7 = Environment.OSVersion.VersionString.Contains("NT 6.1.");
                                AddPrinter(PrinterName, "Microsoft XPS Document Writer" + (isWin7 ? string.Empty : " v4"), PrinterPort);
                            }
                            catch (Exception exception)
                            {
                                LogError(exception, "Failed to add xps printer.");
                            }
                            break;
                        case "ps":
                            try
                            {
                                AddPrinterPort(PrinterPort, "127.0.0.1", 9101);
                                new RegistryRepository().TryGetGhostscriptPath(out var ghostScriptPath);
                                if (ghostScriptPath == null)
                                {
                                    throw new ArgumentNullException(nameof(ghostScriptPath), "Ghostscript path could not be found.");
                                }

                                AddPrinter(PrinterName, "Ghostscript PDF", PrinterPort, Path.Combine(ghostScriptPath, @"lib\ghostpdf.inf"));
                            }
                            catch (Exception exception)
                            {
                                LogError(exception, "Failed to add ps printer.");
                            }
                            break;
                        default:
                            NotUseful();
                            return;
                    }

                    break;
                }
                case TestCmd: {
                    TestPrinter(PrinterName);
                    break;
                }
                case ConfigCmd: {
                    try
                    {
                        ConfigPrinter(PrinterName);
                    }
                    catch (Exception exception)
                    {
                        LogError(exception, "Failed to configurate printer {printerName}.", PrinterName);
                    }
                    break;
                }
                case UninstallCmd: {
                    try
                    {
                        DelPrinter(PrinterName);
                        DelPrinterPort(PrinterPort);
                    }
                    catch (Exception exception)
                    {
                        LogError(exception, "Failed to uninstall {printerName} on port {printerPort}.", PrinterName, PrinterPort);
                    }
                    break;
                }
                default: {
                    NotUseful();
                    break;
                }
            }
        }

        private static void NotUseful()
        {
            var exception = new ArgumentNullException($"Use '{InstallCmd} [PS|XPS]', '{TestCmd}' or '{UninstallCmd}'!");
            LogError(exception, "Failed to handle the arguments.");
            throw exception;
        }

        private static void LogError(Exception exception, string message, params object[] args)
        {
            Logger.Error(exception, message, args);
        }
    }
}