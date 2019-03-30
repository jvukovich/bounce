using System;
using System.IO;

namespace Bounce.Console
{
    public class TargetsAssemblyArgumentsParser
    {
        private readonly IBounceDirectoryFinder _bounceDirectoryFinder;

        public TargetsAssemblyArgumentsParser() : this(new BounceDirectoryFinder())
        {
        }

        public TargetsAssemblyArgumentsParser(IBounceDirectoryFinder bounceDirectoryFinder)
        {
            _bounceDirectoryFinder = bounceDirectoryFinder;
        }

        private static void TryGetTargetsFromArguments(OptionsAndArguments optionsAndArguments)
        {
            var args = optionsAndArguments.RemainingArguments;

            if (args.Length <= 0)
                return;

            var firstArg = args[0];

            const string bounceDir = "bounceDir";

            if (firstArg.StartsWith("/" + bounceDir + ":"))
            {
                var remainingArgs = new string[args.Length - 1];

                Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);

                optionsAndArguments.BounceDirectory = firstArg.Substring(("/" + bounceDir + ":").Length);
                optionsAndArguments.RemainingArguments = remainingArgs;
                optionsAndArguments.WorkingDirectory = Directory.GetCurrentDirectory();
            }
            else if (firstArg == "/" + bounceDir)
            {
                var remainingArgs = new string[args.Length - 2];

                Array.Copy(args, 2, remainingArgs, 0, remainingArgs.Length);

                optionsAndArguments.BounceDirectory = args[1];
                optionsAndArguments.RemainingArguments = remainingArgs;
                optionsAndArguments.WorkingDirectory = Directory.GetCurrentDirectory();
            }
        }

        private static void TryGetRecurseFromArguments(OptionsAndArguments optionsAndArguments)
        {
            var args = optionsAndArguments.RemainingArguments;

            if (args.Length <= 0)
                return;

            var firstArg = args[0];

            if (firstArg != "/recurse")
                return;

            var remainingArgs = new string[args.Length - 1];

            Array.Copy(args, 1, remainingArgs, 0, remainingArgs.Length);

            optionsAndArguments.Recurse = true;
            optionsAndArguments.RemainingArguments = remainingArgs;
        }

        public OptionsAndArguments GetTargetsAssembly(string[] args)
        {
            var optionsAndArguments = new OptionsAndArguments {RemainingArguments = args};

            TryGetTargetsFromArguments(optionsAndArguments);
            TryGetRecurseFromArguments(optionsAndArguments);

            if (optionsAndArguments.BounceDirectory != null)
                return optionsAndArguments;

            var targets = _bounceDirectoryFinder.FindBounceDirectory();

            if (targets == null)
                throw new TargetsAssemblyNotFoundException("unable to find valid Bounce assembly in this or any parent directory");

            optionsAndArguments.BounceDirectory = targets;
            optionsAndArguments.WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(targets));

            return optionsAndArguments;
        }
    }
}