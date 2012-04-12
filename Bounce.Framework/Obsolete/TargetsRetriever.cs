using System.Collections.Generic;
using System.Reflection;

namespace Bounce.Framework.Obsolete {
    public class TargetsRetriever : ITargetsRetriever {
        private readonly ITargetsMethodInvoker TargetsMethodInvoker;
        private readonly ITargetsParser TargetsParser;

        public TargetsRetriever() : this(new TargetsMethodInvoker(), new TargetsParser()) { }

        public TargetsRetriever(ITargetsMethodInvoker targetsMethodInvoker, ITargetsParser targetsParser) {
            TargetsMethodInvoker = targetsMethodInvoker;
            TargetsParser = targetsParser;
        }


        public IDictionary<string, IObsoleteTask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, IParameters parameters) {
            return TargetsParser.ParseTargetsFromObject(TargetsMethodInvoker.InvokeTargetsMethod(getTargetsMethod, parameters));
        }

        public IDictionary<string, IObsoleteTask> GetTargetsFromObject(object targets) {
            return TargetsParser.ParseTargetsFromObject(targets);
        }
    }
}