namespace Bounce.Framework.Obsolete {
    public interface ILogOptionCommandLineTranslator {
        void ParseCommandLine(ParsedCommandLineParameters parsedParameters, IBounce bounce);
        string GenerateCommandLine(IBounce bounce);
    }
}