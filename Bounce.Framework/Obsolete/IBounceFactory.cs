namespace Bounce.Framework.Obsolete {
    public interface IBounceFactory {
        ITargetBuilderBounce GetBounce();
        ITargetBuilderBounce GetBounce(LogOptions logOptions);
    }
}
