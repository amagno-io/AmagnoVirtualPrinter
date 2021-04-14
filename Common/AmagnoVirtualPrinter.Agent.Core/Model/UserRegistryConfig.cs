﻿using System.IO;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public class UserRegistryConfig : IUserConfig
    {
        public bool RedirectEnabled { get; set; }

        public string RedirectPrinter { get; set; }

        public double? UserRenderDpi { get; set; }

        public string Format { get; set; }

        public string OutputDirectory { get; set; }

        public string ResolvedOutputDirectory
        {
            get { return string.IsNullOrWhiteSpace(OutputDirectory) ? "" : Path.GetFullPath(OutputDirectory); }
        }
    }
}