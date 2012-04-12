namespace LegacyBounce.Framework {
    public interface ICommandLineParameterParser {
        ParsedCommandLineParameters ParseCommandLineParameters(string [] args);
    }
}