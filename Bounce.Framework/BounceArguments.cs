using System.Collections.Generic;

namespace Bounce.Framework {
    public static class BounceArguments {
        public static RemoteBounceArguments ForTarget(string target, params IParameter [] parameters) {
            return ForTarget(target, (IEnumerable<IParameter>) parameters);
        }

        public static RemoteBounceArguments ForTarget(string target, IEnumerable<IParameter> parameters) {
            return new RemoteBounceArguments {Targets = new [] {target}, Parameters = parameters};
        }
    }
}