using System;

namespace Bounce.Framework.Obsolete
{
    public interface IBuilder<T> {
        T Build { get; }
        IBuilder<T> With(Action<T> setting);
    }

    public abstract class BaseBuilder<T> : IBuilder<T> {
        public virtual IBuilder<T> With(Action<T> setting) {
            return new ActionBuilder<T>(this, setting);
        }

        public abstract T Build { get; }
    }

    public class BuilderWithCreator<T> : BaseBuilder<T> {
        private Func<T> Create;

        public BuilderWithCreator(Func<T> create)
        {
            Create = create;
        }

        public override T Build {
            get {
                return Create();
            }
        }
    }

    public class Builder<T> : BuilderWithCreator<T> where T : new() {
        public Builder(Func<T> creator) : base(creator) {}
        public Builder() : this(() => new T()) {}
    }

    class ActionBuilder<T> : BaseBuilder<T> {
        private IBuilder<T> Parent;
        private Action<T> Setting;

        public ActionBuilder(IBuilder<T> parent, Action<T> setting)
        {
            Parent = parent;
            Setting = setting;
        }

        public override T Build {
            get {
                var t = Parent.Build;
                Setting(t);
                return t;
            }
        }
    }
}
