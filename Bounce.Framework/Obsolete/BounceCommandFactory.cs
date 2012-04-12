namespace Bounce.Framework.Obsolete {
    public class BounceCommandFactory {
        public static IBounceCommand GetCommandByName(string command) {
            var commandParser = new BounceCommandParser();
            return commandParser.Parse(command);
        }
    }
}
