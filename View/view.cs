using System;
using System.Collections.Generic;

namespace FileFragmentationApp.View
{
    public class ConsoleView
    {
        public void Show(string msg = "") => Console.WriteLine(msg);

        public void ShowError(string msg)
        {
            var prev = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = prev;
        }

        public string Prompt(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }

        public void ShowFiles(IEnumerable<string> files)
        {
            foreach (var f in files)
                Console.WriteLine(f);
        }
    }
}
