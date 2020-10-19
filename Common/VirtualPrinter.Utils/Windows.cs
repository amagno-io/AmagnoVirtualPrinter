using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace VirtualPrinter.Utils
{
    public class Windows
    {
        [NotNull]
        public static Exception LastError => new Win32Exception(Marshal.GetLastWin32Error());
    }
}
