using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(System.Windows.Forms.Clipboard.GetText());

            Console.WriteLine("Start Clipboard Test App");
            var logger = new Logger();
            var ocm = new OCMClip.Manager(logger);
            ocm.ClipboardTextChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clipboard Text => " + e.Value);
                Console.ResetColor();
            };
            ocm.ClipboardImageChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clipboard Image bytes length => " + e.Value.Length);
                Console.ResetColor();
            };

            ocm.Load(new OCMClip.Configuration(new OCMClip.ConfigurationWatcher(50, 0, true, true, false)));
            ocm.StartWatcher();

            Console.ReadKey();
        }
    }
}
