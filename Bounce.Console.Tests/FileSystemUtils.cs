using System;
using System.IO;

namespace Bounce.Console.Tests {
    public static class FileSystemUtils {
        public static void Pushd(string dir, Action doWhenInDirectory)
        {
            string cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            try
            {
                doWhenInDirectory();
            }
            finally
            {
                Directory.SetCurrentDirectory(cwd);
            }
        }
    }
}