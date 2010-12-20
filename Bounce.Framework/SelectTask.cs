using System;

namespace Bounce.Framework {
    public class SelectTask<TInput, TTaskOutput> : TaskWithValue<TTaskOutput> where TTaskOutput : ITask {
        [Dependency]
        private readonly Future<TInput> Input;
        private readonly Func<TInput, TTaskOutput> GetTask;
        private TTaskOutput _value;

        public SelectTask(Future<TInput> input, Func<TInput, TTaskOutput> getTask) {
            Input = input;
            GetTask = getTask;
        }

        public override TTaskOutput GetValue() {
            return _value;
        }

        public override void InvokeFuture(IBounceCommand command, IBounce bounce) {
            _value = GetTask(Input.Value);
            _value.Invoke(command, bounce);
        }
    }
}