using System;
using System.Threading;

namespace VeryLongRunningConsole
{
    static class Program
    {
        private static void Main(string[] args)
        {
            Thread.Sleep(TimeSpan.FromMinutes(5));
        }
    }
}