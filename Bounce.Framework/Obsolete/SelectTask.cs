using System;

namespace Bounce.Framework.Obsolete {
    public class SelectTask<TInput, TTaskOutput> : TaskWithValue<TTaskOutput> where TTaskOutput : ITask {
        [Dependency]
        private readonly Task<TInput> Input;
        private readonly Func<TInput, TTaskOutput> GetTask;
        private TTaskOutput _value;

        public SelectTask(Task<TInput> input, Func<TInput, TTaskOutput> getTask) {
            Input = input;
            GetTask = getTask;
        }

        protected override TTaskOutput GetValue() {
            return _value;
        }

        public override void InvokeTask(IBounceCommand command, IBounce bounce) {
            _value = GetTask(Input.Value);
            bounce.Invoke(command, _value);
        }
    }
}