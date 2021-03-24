using System;
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AmagnoVirtualPrinter.Utils;
using JetBrains.Annotations;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using WixSharp;

using Action = WixSharp.Action;
using File = WixSharp.File;
using Files = AmagnoVirtualPrinter.Utils.Files;
using Keys = AmagnoVirtualPrinter.Utils.Keys;
using RegistryHive = WixSharp.RegistryHive;

namespace AmagnoVirtualPrinter.WixSharpInstaller
{
    public class Script
    {
        private static string _filesDir;
        const string SetupDriverId = "setupdrv_exe";

        public static void Main([NotNull]string[] args)
        {
            var workingDir = "";
            if (args.Length > 0)
            {
                foreach(var cmd in args)
                {
                    if (cmd.Contains(@"/p:"))
                    {
                        workingDir = cmd.Remove(0, 3);
                        Console.WriteLine(workingDir);
                    }
                }
            }

            if (workingDir.IsNullOrEmpty())
            {
                throw new ArgumentException("Argument for working directory (/p) not set.");
            }

            _filesDir = Path.Combine(workingDir, Utils.Files.FILES);
            var feature = new Feature("AmagnoPrinter");
            var printerServiceFile = new File(feature, Path.Combine(_filesDir, Utils.Files.PRINTER_SERVICE_EXE))
            {
                ServiceInstaller = new ServiceInstaller
                {
                    Name = "AmagnoPrinterService",
                    StartOn = SvcEvent.Install_Wait,
                    StopOn = SvcEvent.InstallUninstall_Wait,
                    RemoveOn = SvcEvent.Uninstall_Wait,
                    ErrorControl = SvcErrorControl.normal,
                    ConfigureServiceTrigger = ConfigureServiceTrigger.None
                }
            };

            var project = new ManagedProject("AmagnoPrinterInstaller")
            {
                Name = "Amagno Printer",
                GUID = new Guid("8712D2CD-A9F6-456F-99C8-92C2BB070596"),
                UpgradeCode = new Guid("0B37A935-EDEC-4ACA-9307-6D8299496C1D"),
                UI = WUI.WixUI_InstallDir,
                Version = Version.Parse(FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion),
                LicenceFile = Path.Combine(workingDir, Utils.Files.LICENCE_FILE),
                Dirs = CreateProjectDirs(feature, printerServiceFile),
                Actions = CreateActions(),
                RegValues = CreateRegValues(feature).ToArray(),
                InstallPrivileges = InstallPrivileges.elevated
            };

            project.BeforeInstall += ProjectOnBeforeInstall;

            project.BuildMsi();
        }

        private static void ProjectOnBeforeInstall(SetupEventArgs e)
        {
            const string gsNotFound = "Ghostscript not found!\n" +
                                      "Please install Ghostscript version 9.52 or higher.\n" +
                                      "You can find the installer on the official website:\n" +
                                      "https://www.ghostscript.com/download/gsdnld.html";

            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

            var gsKey = RegistryKey
                .OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, registryView)
                .OpenSubKey(@"SOFTWARE\GPL Ghostscript");

            if (gsKey == null)
            {
                MessageBox.Show(gsNotFound);
                e.Result = ActionResult.Failure;
                return;
            }

            var subKeyNames = gsKey.GetSubKeyNames();
            gsKey = gsKey.OpenSubKey(subKeyNames[0]);
            if (gsKey == null)
            {
                MessageBox.Show(gsNotFound);
                e.Result = ActionResult.Failure;
                return;
            }

            var gsAssembly = (string)gsKey.GetValue("GS_DLL");

            if (System.IO.File.Exists(gsAssembly))
            {
                e.Result = ActionResult.Success;
                return;
            }

            MessageBox.Show(gsNotFound);
            e.Result = ActionResult.Failure;
        }

        [NotNull, ItemNotNull]
        private static Dir[] CreateProjectDirs
        (
            Feature feature,
            File printerServiceFile
        )
        {
            return new[]
            {
                new Dir
                (
                    @"%ProgramFiles%\AmagnoPrinterDriver\",
                    new DirFiles(feature, _filesDir + @"\*", s => !s.EndsWith(".exe")),
                    new File(new Id(SetupDriverId), feature, Path.Combine(_filesDir, Utils.Files.SETUP_DRIVER_EXE)),
                    new File(feature, Path.Combine(_filesDir, Utils.Files.DILIVERY_EXE)),
                    new File(feature, Path.Combine(_filesDir, Utils.Files.AGENT_PROGRESS_EXE)),
                    printerServiceFile
                )
            };
        }

        [NotNull, ItemNotNull]
        private static Action[] CreateActions()
        {
            return new Action[]
            {
                new InstalledFileAction(SetupDriverId, "install ps", Return.check, When.After, Step.InstallFinalize, Condition.NOT_Installed),
                new InstalledFileAction(SetupDriverId, "uninstall", Return.check, When.Before, Step.RemoveFiles, Condition.BeingUninstalled)
            };
        }

        [NotNull]
        private static IEnumerable<RegValue> CreateRegValues(Feature feature)
        {
            var converterKey = $@"{Utils.Keys.PRINTER_DRIVER_KEY32}\{Utils.Keys.CONVERTER_KEY}";

            var regValues = new List<RegValue>();
            regValues.AddRange(CreateLocalMachineValues(feature, converterKey));
            regValues.AddRange(CreateCurrentUserValues(feature, converterKey));

            return regValues;
        }

        [NotNull]
        private static IEnumerable<RegValue> CreateLocalMachineValues(Feature feature, string converterKey)
        {
            var postConverterKey = $@"{Utils.Keys.PRINTER_DRIVER_KEY32}\{Utils.Keys.POSTCONVERTER_KEY}";
            var preConverterKey = $@"{Utils.Keys.PRINTER_DRIVER_KEY32}\{Utils.Keys.PRECONVERTER_KEY}";
            var converterPdfKey = $@"{Utils.Keys.PRINTER_DRIVER_KEY32}\{Utils.Keys.CONVERTER_PDF_KEY}";
            var converterTiffKey = $@"{Utils.Keys.PRINTER_DRIVER_KEY32}\{Utils.Keys.CONVERTER_TIFF_KEY}";
            var registryHive = RegistryHive.LocalMachine;

            var result = new List<RegValue>
            {
                new RegValue(feature, registryHive, Utils.Keys.PRINTER_DRIVER_KEY32, KeyNames.INSTALLATION_DIR, "[INSTALLDIR]"),
                new RegValue(feature, registryHive, converterKey, KeyNames.SERVER_PORT, 9101) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterKey, KeyNames.THREADS, 2),
                new RegValue(feature, registryHive, converterKey, KeyNames.SHOW_PROGRESS, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterKey, KeyNames.PAGES_PER_SHEET, 1),
                new RegValue(feature, registryHive, converterKey, KeyNames.FILE_NAME_MASK, "{yyyy}{MM}{DD}{hh}{mm}{ss}{job05}{page03}"),
                new RegValue(feature, registryHive, converterKey, KeyNames.OUTPUT_DIR, string.Empty),
                new RegValue(feature, registryHive, converterKey, KeyNames.FORMAT, "ps"),
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.ENABLED, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.MULTIPAGE, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.PRODUCE_PDFA, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.ALLOW_COPYING, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.ALLOW_PRINTING, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey, KeyNames.SUBSETTING, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterPdfKey,KeyNames.QUALITY , 80),
                new RegValue(feature, registryHive, converterTiffKey, KeyNames.ENABLED, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterTiffKey, KeyNames.BITS_PIXEL, 24),
                new RegValue(feature, registryHive, converterTiffKey, KeyNames.MULTIPAGE, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, converterTiffKey, KeyNames.COMPRESSION, 8)
            };

            var registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            var addPostConverterKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, registryView).OpenSubKey(postConverterKey) == null;
            var addPreConverterKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, registryView).OpenSubKey(preConverterKey) == null;
            if (addPostConverterKey)
            {
                result.Add(new RegValue(feature, registryHive, postConverterKey, KeyNames.EXECUTABLE_FILE, Files.POST_CONVERTER));
            }

            if (addPreConverterKey)
            {
                result.Add(new RegValue(feature, registryHive, preConverterKey, KeyNames.EXECUTABLE_FILE, Files.PRE_CONVERTER));
            }

            return result;
        }

        [NotNull]
        private static IEnumerable<RegValue> CreateCurrentUserValues(Feature feature, string converterKey)
        {
            var redirectKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.CONVERTER_REDIRECT_KEY}";
            var registryHive = RegistryHive.CurrentUser;

            return new List<RegValue>
            {
                new RegValue(feature, registryHive, converterKey, KeyNames.PRINT_FORMAT, "PDF"),
                new RegValue(feature, registryHive, converterKey, KeyNames.RENDER_DPI, 300) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, redirectKey, KeyNames.ENABLED, 1) {AttributesDefinition = "Type=integer"},
                new RegValue(feature, registryHive, redirectKey, KeyNames.PRINTER, "")
            };
        }
    }
}
