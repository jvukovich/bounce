using System;

namespace Bounce.Console
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                BounceAssemblyRunner.Run(args);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return 70;
            }

            return 0;
        }
    }
}