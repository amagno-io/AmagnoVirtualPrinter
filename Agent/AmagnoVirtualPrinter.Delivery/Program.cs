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

                    if (!PrinterExists(printerName))
                    {
                        Console.WriteLine($"Printer {printerName} does not exist!");
                    }
                    
                    RedirectToPrinter(filePath, printerName);
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
            throw new ArgumentNullException($"Use '{RedirectCmd}'!");
        }
    }
}