namespace Bounce.Framework.Obsolete {
    public class NullTask : Task {
        public override bool IsLogged
        {
            get { return false; }
        }
    }
}