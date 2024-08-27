using System;

using JetBrains.Annotations;

using static AmagnoVirtualPrinter.Delivery.Redirector;

namespace AmagnoVirtualPrinter.Delivery
{
    internal class Program
    {
        private const string RedirectCmd = "redirect";

        [STAThread]
        private static void Main([CanBeNull]string[] args)
        {
            if (args == null || args.Length < 1) {
                NotUseful();
                return;
            }

            switch (args[0]) {
                case RedirectCmd: {
                    var filePath = args[1];
                    var printerName = args[2];

#if DEBUG
                    CheckPrinter(printerName);
#endif
                    
                    RedirectToPrinter(filePath, printerName);
                    break;
                }
                default: {
                    NotUseful();
                    break;
                }
            }
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Can be used to debug the redirection
        /// </summary>
        /// <param name="printerName"></param>
        private static void CheckPrinter(string printerName)
        {
            try
            {
                if (!PrinterExists(printerName))
                {
                    Console.WriteLine($"Printer {printerName} does not exist!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error checking printer:" + e);
            }
        }

        private static void NotUseful()
        {
            throw new ArgumentNullException($"Use '{RedirectCmd}'!");
        }
    }
}