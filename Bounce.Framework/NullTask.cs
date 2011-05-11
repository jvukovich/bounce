namespace Bounce.Framework {
    public class NullTask : Task {
        public override bool IsLogged
        {
            get { return false; }
        }
    }
}