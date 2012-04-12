using System.IO;

namespace Bounce.Framework.Obsolete {
    class CompilationLog : ICompilationLog {
        private TextWriter StdErr;

        public CompilationLog(TextWriter stdErr) {
            StdErr = stdErr;
        }

        public void Error(string message) {
            StdErr.WriteLine(message);
        }

        public void Warning(string message) {
            StdErr.WriteLine(message);
        }
    }
}