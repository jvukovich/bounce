namespace Bounce.Framework {
    public interface ICommandLineParameterParser {
        ParsedCommandLineParameters ParseCommandLineParameters(string [] args);
    }
}