using System;
using System.IO;
using System.Printing;

using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Delivery
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

            var ghostScriptRedirector = new GhostScriptRedirector(printerName);
            ghostScriptRedirector.Redirect(file);
        }
        
        /// <summary>
        /// Check if the printer exists
        /// </summary>
        /// <param name="printerName">e.g. "Dell C2665dnf Color MFP"</param>
        /// <returns>True, if the printer exists</returns>
        public static bool PrinterExists(string printerName)
        {
            using (var localSystem = new LocalPrintServer())
            {
                using (localSystem.GetPrintQueue(printerName))
                {
                    return true;
                }
            }
        }
    }
}