namespace LegacyBounce.Framework {
    class LogValue<T> : TaskWithValue<T> {
        [Dependency]
        public Task<string> Message;
        [Dependency]
        public new Task<T> Value;

        private bool IsDebug;

        public LogValue(bool isDebug) {
            IsDebug = isDebug;
        }

        public override void InvokeTask(IBounceCommand command, IBounce bounce) {
            if (IsDebug) {
                bounce.Log.Debug("{0}: {1}", Message.Value, Value.Value);
            } else {
                bounce.Log.Info("{0}: {1}", Message.Value, Value.Value);
            }
        }

        protected override T GetValue() {
            return Value.Value;
        }
    }
}