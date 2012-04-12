namespace LegacyBounce.Framework {
    public interface IBounceFactory {
        ITargetBuilderBounce GetBounce();
        ITargetBuilderBounce GetBounce(LogOptions logOptions);
    }
}
