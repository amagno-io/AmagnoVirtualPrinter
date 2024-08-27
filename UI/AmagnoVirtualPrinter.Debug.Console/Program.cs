using System.Drawing.Printing;
using System.Linq;
using System.Printing;

namespace AmagnoVirtualPrinter.Debug.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Use the following parameters");
            System.Console.WriteLine("-l - Print all local printers");
            System.Console.WriteLine("-a - Print all printers");
            System.Console.WriteLine("-d - Print printers of System.Drawing");
            System.Console.WriteLine("-v - Verbose information");

            var verbose = args.Contains("-v");
            
            if (args.Contains("-l"))
            {
                GetLocalPrinters(verbose);
                System.Console.WriteLine("------");
            }

            if (args.Contains("-a"))
            {
                GetAllPrinters(verbose);
                System.Console.WriteLine("------");
            }

            if (args.Contains("-d"))
            {
                GetAllSystemDrawingPrinters();
                System.Console.WriteLine("------");
            }
        }

        private static void GetAllSystemDrawingPrinters()
        {
            System.Console.WriteLine("System Drawing Printers:");
            foreach (var printer in PrinterSettings.InstalledPrinters)
            {
                System.Console.WriteLine($"Name: {printer}");
            }
        }

        private static void GetLocalPrinters(bool verbose)
        {
            System.Console.WriteLine("Local Printers:");
            using (var localSystem = new LocalPrintServer())
            {
                foreach (var queue in localSystem.GetPrintQueues())
                {
                    System.Console.WriteLine($"Name: {queue.Name}");
                    if (verbose)
                    {
                        System.Console.WriteLine($"Fullname: {queue.FullName}");
                        System.Console.WriteLine($"Location: {queue.Location}");
                        System.Console.WriteLine($"IsShared: {queue.IsShared}");
                        System.Console.WriteLine($"IsDirect: {queue.IsDirect}");
                        System.Console.WriteLine("---");
                    }
                }
            }
        }

        private static void GetAllPrinters(bool verbose)
        {
            System.Console.WriteLine("All Printers:");
            using (var localSystem = new PrintServer())
            {
                foreach (var queue in localSystem.GetPrintQueues())
                {
                    System.Console.WriteLine($"Name: {queue.Name}");
                    if (verbose)
                    {
                        System.Console.WriteLine($"Fullname: {queue.FullName}");
                        System.Console.WriteLine($"Location: {queue.Location}");
                        System.Console.WriteLine($"IsShared: {queue.IsShared}");
                        System.Console.WriteLine($"IsDirect: {queue.IsDirect}");
                        System.Console.WriteLine("---");
                    }
                }
            }
        }
    }
}