using System;
using System.IO;
using System.Printing;

using JetBrains.Annotations;

namespace VirtualPrinter.Delivery
{
    public static class Redirector
    {
        public static void RedirectToPrinter([NotNull]string filePath, [NotNull]string printerName)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (printerName == null)
            {
                throw new ArgumentNullException(nameof(printerName));
            }

            var file = Path.GetFullPath(filePath);
            // e.g. "Dell C2665dnf Color MFP"
            using (var localSystem = new LocalPrintServer())
            {
                using (var queue = localSystem.GetPrintQueueSafe(printerName))
                {
                    if (queue != null)
                    {
                        var ghostScriptRedirector = new GhostScriptRedirector(queue);
                        ghostScriptRedirector.Redirect(file);
                    }
                }
            }
        }

        [CanBeNull]
        private static PrintQueue GetPrintQueueSafe(this PrintServer server, string name)
        {
            try
            {
                return server.GetPrintQueue(name);
            }
            catch
            {
                return null;
            }
        }
    }
}