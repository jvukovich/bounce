namespace LegacyBounce.Framework {
    public class BounceFactory : IBounceFactory {
        public ITargetBuilderBounce GetBounce() {
            return new Bounce();
        }

        public ITargetBuilderBounce GetBounce(LogOptions logOptions) {
            return new Bounce(logOptions);
        }
    }
}
