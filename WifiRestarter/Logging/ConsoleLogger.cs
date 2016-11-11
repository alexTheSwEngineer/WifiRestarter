using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WifiRestarter
{
    class ConsoleLogger:ILogger
    {
        public  void Info(string line)
        {
            OverWriteLine(AddTimeStamp(line));
        }

        public void Trace(string line)
        {
            WriteLine(AddTimeStamp(line));
        }

        public void Error(string line)
        {
            WriteLine(AddTimeStamp(line));
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void ClearPreviousLine()
        {
            int currentLineCursor = Console.CursorTop - 1;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void OverWriteLine(string line)
        {
            ClearPreviousLine();
            Console.WriteLine(line);
        }

        public static void WriteLine(string line)
        {
            ClearPreviousLine();
            Console.WriteLine(line);
            Console.WriteLine();
        }        

        public static string AddTimeStamp(string str)
        {
            return $"{DateTime.Now.ToString("MM/dd/yy HH:mm:ss")} {str}";
        }

        public void Error(Exception e)
        {
            WriteLine(AddTimeStamp($"Error {e}"));
            Console.Write(e.StackTrace);
            Console.WriteLine();
        }
    }

    
}
