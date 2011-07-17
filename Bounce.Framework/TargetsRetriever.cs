using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Bounce.Framework {
    public class TargetsRetriever : ITargetsRetriever {
        private readonly ITargetsMethodInvoker TargetsMethodInvoker;
        private readonly ITargetsParser TargetsParser;

        public TargetsRetriever() : this(new TargetsMethodInvoker(), new TargetsParser()) { }

        public TargetsRetriever(ITargetsMethodInvoker targetsMethodInvoker, ITargetsParser targetsParser) {
            TargetsMethodInvoker = targetsMethodInvoker;
            TargetsParser = targetsParser;
        }


        public IDictionary<string, ITask> GetTargetsFromAssembly(MethodInfo getTargetsMethod, IParameters parameters) {
            return TargetsParser.ParseTargetsFromObject(TargetsMethodInvoker.InvokeTargetsMethod(getTargetsMethod, parameters));
        }

        public IDictionary<string, ITask> GetTargetsFromObject(object targets) {
            return TargetsParser.ParseTargetsFromObject(targets);
        }
    }
}