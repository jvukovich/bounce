namespace Bounce.Framework {
    public interface ICompilationLog {
        void Error(string message);
        void Warning(string message);
    }
}