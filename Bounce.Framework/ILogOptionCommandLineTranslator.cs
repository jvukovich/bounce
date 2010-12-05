namespace Bounce.Framework {
    public interface ILogOptionCommandLineTranslator {
        void ParseCommandLine(ParsedCommandLineParameters parsedParameters, IBounce bounce);
        string GenerateCommandLine(IBounce bounce);
    }
}