using System;

using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.SetupDriver
{
    internal static class Windows
    {
        public static void AddPrinter([NotNull]string name, [NotNull]string model, [NotNull]string port, [CanBeNull]string driver = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (port == null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            driver = driver ?? Shell.GetPrintInf();
            var args = $@"printui.dll,PrintUIEntry /if /b ""{name}"" /f ""{driver}"" /r ""{port}"" /m ""{model}"" /u";
            Shell.Execute("rundll32", args);
        }

        public static void TestPrinter([NotNull]string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var args = $@"printui.dll,PrintUIEntry /k /n ""{name}""";
            Shell.Execute("rundll32", args);
        }

        public static void ConfigPrinter([NotNull]string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var args = $@"printui.dll,PrintUIEntry /e /n ""{name}""";
            Shell.Execute("rundll32", args);
        }

        public static void DelPrinter([NotNull]string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var args = $@"printui.dll,PrintUIEntry /dl /n ""{name}""";
            Shell.Execute("rundll32", args);
        }

        public static void AddPrinterPort([NotNull]string name, [NotNull]string ip, int port, [CanBeNull]string vbs = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }

            vbs = vbs ?? Shell.GetPrintVbs();
            var args = $@"""{vbs}"" -a -r {name} -h {ip} -o raw -n {port}";
            Shell.Execute("cscript", args);
        }

        public static void DelPrinterPort([NotNull]string name, [CanBeNull]string vbs = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            vbs = vbs ?? Shell.GetPrintVbs();
            var args = $@"""{vbs}"" -d -r {name}";
            Shell.Execute("cscript", args);
        }
    }
}