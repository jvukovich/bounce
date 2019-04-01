using System;
using System.Linq;
using System.Reflection;
using Bounce.Framework;

namespace Bounce
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length == 1)
                {
                    if (args.Contains("--help") || args.Contains("-h"))
                    {
                        UsageHelp.WriteUsage();
                        return 0;
                    }

                    if (args.Contains("--version") || args.Contains("-v"))
                    {
                        var version =
                            Assembly.GetEntryAssembly()
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                .InformationalVersion;

                        Console.WriteLine(version);

                        return 0;
                    }
                }

                BounceAssemblyRunner.Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 70;
            }

            return 0;
        }
    }
}