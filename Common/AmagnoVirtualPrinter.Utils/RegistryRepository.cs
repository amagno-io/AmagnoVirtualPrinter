﻿using System;
using System.IO;
using System.Linq;
using AmagnoVirtualPrinter.Agent.Core.Enums;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using AmagnoVirtualPrinter.Logging;
using JetBrains.Annotations;

using Microsoft.Win32;

namespace AmagnoVirtualPrinter.Utils
{
    public class RegistryRepository : IRegistryRepository
    {
        private const short DefaultServerPort = 9101;
        
        private static readonly IVirtualPrinterLogger<RegistryRepository> Logger = new VirtualPrinterLogger<RegistryRepository>();

        public bool TryGetGhostscriptPath(out string path)
        {
            path = null;

            try
            {
                var regView = GetRegistryView();

                using(var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regView))
                {
                    const string gsSubKey = @"SOFTWARE\GPL Ghostscript";
                    using(var ghostscript = baseKey.OpenSubKey(gsSubKey))
                    {
                        CheckForNull(ghostscript, gsSubKey);
                        var subKeys = ghostscript.GetSubKeyNames();

                        var lastSubKey = subKeys.Last();
                        using(var subKey = ghostscript.OpenSubKey(lastSubKey))
                        {
                            CheckForNull(subKey, lastSubKey);
                            path = subKey.GetValue("GS_LIB").ToString().Split(';').FirstOrDefault();

                            if (string.IsNullOrWhiteSpace(path))
                            {
                                return false;
                            }

                            path = Directory.GetParent(path)?.FullName;
                            return !string.IsNullOrWhiteSpace(path);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return false;
        }

        public IExConfig GetRegistryConfig()
        {
            try
            {
                return GetRegistryConfigInternal();
            }
            catch (Exception e)
            {
                throw new RegistryException("Error getting registry config", e);
            }
        }

        private IExConfig GetRegistryConfigInternal()
        {
            var regView = GetRegistryView();
            var subKey = GetSubKey();
            var registryConfig = new RegistryConfig();

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regView))
            {
                using (var driver = baseKey.OpenSubKey(subKey))
                {
                    CheckForNull(driver, subKey);

                    registryConfig.CustomRedirection = GetRegKeyStringValue(driver, KeyNames.CUSTOM_REDIRECTION_KEY);
                    
                    using(var key = driver.OpenSubKey(Keys.POSTCONVERTER_KEY))
                    {
                        CheckForNull(key, Keys.POSTCONVERTER_KEY);
                        registryConfig.Postconverter = key.GetValue(KeyNames.EXECUTABLE_FILE).ToString();
                    }

                    using(var key = driver.OpenSubKey(Keys.PRECONVERTER_KEY))
                    {
                        CheckForNull(key, Keys.PRECONVERTER_KEY);
                        registryConfig.Preconverter = key.GetValue(KeyNames.EXECUTABLE_FILE).ToString();
                    }

                    using (var key = driver.OpenSubKey(Keys.CONVERTER_KEY))
                    {
                        CheckForNull(key, Keys.CONVERTER_KEY);
                        registryConfig.FileNameMask = key.GetValue(KeyNames.FILE_NAME_MASK).ToString();
                        var portStr = key.GetValue(KeyNames.SERVER_PORT).ToString();
                        registryConfig.PrinterPort = short.TryParse(portStr, out var portVal) ? portVal : DefaultServerPort;
                        registryConfig.IntermediateFormat = key.GetValue(KeyNames.FORMAT)?.ToString().ToLower() == "ps" ? IntermediateFormat.Ps : IntermediateFormat.Xps;
                    }
                }
            }

            return registryConfig;
        }

        private static string GetRegKeyStringValue(RegistryKey key, string name)
        {
            var value = key.GetValue(name);

            if (value != null)
            {
                return value.ToString();
            }

            return string.Empty;
        }

        [ContractAnnotation("key:null => void")]
        private static void CheckForNull(RegistryKey key, string keyName)
        {
            if (key == null)
            {
                throw new NullReferenceException(keyName);
            }
        }

        public IUserConfig GetUserRegistryConfig(string sid)
        {
            Logger.Trace("GetUserRegistryConfig for {sid}", sid);

            var registryConfig = GetRegistryConfig();
            var regView = GetRegistryView();
            var userConfig = new UserRegistryConfig();

            using (var users = RegistryKey.OpenBaseKey(RegistryHive.Users, regView))
            {
                var subKey = $@"{sid}\{Keys.PRINTER_DRIVER_KEY32}";
                using (var driver = users.OpenSubKey(subKey))
                {
                    CheckForNull(driver, subKey);

                    using (var converter = driver.OpenSubKey(Keys.CONVERTER_KEY))
                    {
                        CheckForNull(converter, Keys.CONVERTER_KEY);
                        userConfig.OutputDirectory = converter.GetValue(KeyNames.OUTPUT_DIR).ToString();

                        if (!string.IsNullOrEmpty(registryConfig.CustomRedirection))
                        {
                            return userConfig;
                        }

                        subKey = "Redirect";
                        using (var redirect = converter.OpenSubKey(subKey))
                        {
                            CheckForNull(redirect, subKey);

                            userConfig.RedirectEnabled = (int?) redirect.GetValue("Enabled") == 1;
                            userConfig.RedirectPrinter = redirect.GetValue("Printer").ToString();
                            userConfig.Format = converter.GetValue("Format").ToString();

                            var dpiStr = (string)driver.GetValue("RENDER: DPI");
                            if (dpiStr == null)
                            {
                                userConfig.UserRenderDpi = null;
                            }
                            else
                            {
                                userConfig.UserRenderDpi = double.TryParse(dpiStr, out var dpiVal) ? dpiVal : (double?) null;
                            }
                        }
                    }
                }
            }

            return userConfig;
        }

        private RegistryView GetRegistryView()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return RegistryView.Registry64;
            }

            return RegistryView.Registry32;
        }

        [NotNull]
        private string GetSubKey()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return Keys.PRINTER_DRIVER_KEY64;
            }

            return Keys.PRINTER_DRIVER_KEY32;
        }
    }
}