using System;
using CommandLine;
using AmagnoVirtualPrinter.Utils;

namespace ShellTestConsoleApp
{
    internal class Program
    {
        public class Options
        {
            [Option('s', "session", Required = false, Default = 1)]
            public int Session { get; set; }

            [Option('u', "user", Required = false, Default = 1)]
            public string User { get; set; }

            [Option('c', "command", Required = false, Default = 1)]
            public string Command { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    Console.WriteLine($"Session: {o.Session}");
                    Console.WriteLine($"User: {o.User}");
                    Console.WriteLine($"Command: {o.Command}");

                    Win32Sys.CreateProcessAsUser(o.Session, o.User, o.Command);
                });
        }
    }
}
