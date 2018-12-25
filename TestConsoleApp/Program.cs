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
            /*ocm.ClipboardTextChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clipboard Text => " + e.Value);
                Console.ResetColor();
            };*/
            ocm.ClipboardImageChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clipboard Image bytes length => " + e.Value.Length);
                Console.ResetColor();
            };
            ocm.ClipboardFileChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Clipboard Files => " + e.Value.Count);
                for (int i = 0; i < e.Value.Count; i++)
                {
                    Console.WriteLine($"{i}. File: " + e.Value[i]);
                }
                Console.ResetColor();
            };

            ocm.Load(new OCMClip.Configuration(new OCMClip.ConfigurationWatcher(50, 0, true, true, true)));
            ocm.Query();
            ocm.StartWatcher();

            var ocm1 = new OCMClip.Manager(logger);
            ocm1.ClipboardTextChanged += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[Instance 2] Clipboard Text => " + e.Value);
                Console.ResetColor();
            };

            var hotKey = new OCMHotKey.Manager();
            hotKey.Add(OCMHotKey.Enums.Key.B, 
                OCMHotKey.Enums.KeyModifier.Ctrl, 
                (e) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("HotKey callback => " + e.UniqueName);
                Console.ResetColor();
            });
            hotKey.HotKeyPressed += (obj, e) =>
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("HotKey event => " + e.UniqueName);
                Console.ResetColor();
            };

            Console.ReadKey();
        }
    }
}
