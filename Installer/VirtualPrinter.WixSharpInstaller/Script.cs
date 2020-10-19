using System;
using System.Collections.Generic;
// ReSharper disable once RedundantUsingDirective
using System.Diagnostics;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using VirtualPrinter.Utils;

using WixSharp;

using Action = WixSharp.Action;
using File = WixSharp.File;
using Files = VirtualPrinter.Utils.Files;
using RegistryHive = WixSharp.RegistryHive;

namespace VirtualPrinter.WixSharpInstaller
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

            _filesDir = Path.Combine(workingDir, Files.FILES);
            var feature = new Feature("VPD");
            var printerServiceFile = new File(feature, Path.Combine(_filesDir, Files.PRINTER_SERVICE_EXE))
            {
                ServiceInstaller = new ServiceInstaller
                {
                    Name = "VirtualPrinterService",
                    StartOn = SvcEvent.Install_Wait,
                    StopOn = SvcEvent.InstallUninstall_Wait,
                    RemoveOn = SvcEvent.Uninstall_Wait,
                    ErrorControl = SvcErrorControl.normal,
                    ConfigureServiceTrigger = ConfigureServiceTrigger.None
                }
            };

            var project = new ManagedProject("VPDInstaller")
            {
                Name = "Virtual Printer Driver",
                GUID = new Guid("8712D2CD-A9F6-456F-99C8-92C2BB070596"),
                UpgradeCode = new Guid("0B37A935-EDEC-4ACA-9307-6D8299496C1D"),
                UI = WUI.WixUI_InstallDir,
                Version = Version.Parse(FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion),
                LicenceFile = Files.LICENCE_FILE,
                Dirs = CreateProjectDirs(feature, printerServiceFile),
                Actions = CreateActions(),
                RegValues = CreateRegValues(feature).ToArray(),
                InstallPrivileges = InstallPrivileges.elevated
            };

            project.BuildMsi();
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
                    @"%ProgramFiles%\MyPrinterDriver\",
                    new DirFiles(feature, _filesDir + @"\*", s => !s.EndsWith(".exe")),
                    new File(new Id(SetupDriverId), feature, Path.Combine(_filesDir, Files.SETUP_DRIVER_EXE)),
                    new File(feature, Path.Combine(_filesDir, Files.DILIVERY_EXE)),
                    new File(feature, Path.Combine(_filesDir, Files.AGENT_PROGRESS_EXE)),
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
            var converterKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.CONVERTER_KEY}";

            var regValues = new List<RegValue>();
            regValues.AddRange(CreateLocalMachineValues(feature, converterKey));
            regValues.AddRange(CreateCurrentUserValues(feature, converterKey));

            return regValues;
        }

        [NotNull]
        private static IEnumerable<RegValue> CreateLocalMachineValues(Feature feature, string converterKey)
        {
            var postConverterKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.POSTCONVERTER_KEY}";
            var preConverterKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.PRECONVERTER_KEY}";
            var converterPdfKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.CONVERTER_PDF_KEY}";
            var converterTiffKey = $@"{Keys.PRINTER_DRIVER_KEY32}\{Keys.CONVERTER_TIFF_KEY}";
            var registryHive = RegistryHive.LocalMachine;

            return new List<RegValue>
            {
                new RegValue(feature, registryHive, Keys.PRINTER_DRIVER_KEY32, KeyNames.INSTALLATION_DIR, "[INSTALLDIR]"),
                new RegValue(feature, registryHive, postConverterKey, KeyNames.EXECUTABLE_FILE, Files.POST_CONVERTER),
                new RegValue(feature, registryHive, preConverterKey, KeyNames.EXECUTABLE_FILE, Files.PRE_CONVERTER),
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
