using System;
using System.IO;

namespace Bounce
{
    public static class TargetsAssemblyArgumentsParser
    {
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

        public static OptionsAndArguments GetTargetsAssembly(string[] args)
        {
            var optionsAndArguments = new OptionsAndArguments {RemainingArguments = args};

            TryGetTargetsFromArguments(optionsAndArguments);

            if (optionsAndArguments.BounceDirectory != null)
                return optionsAndArguments;

            optionsAndArguments.BounceDirectory = Directory.GetCurrentDirectory();
            optionsAndArguments.WorkingDirectory = Directory.GetCurrentDirectory();

            return optionsAndArguments;
        }
    }
}