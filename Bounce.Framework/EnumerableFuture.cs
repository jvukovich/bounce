using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework
{
    public abstract class EnumerableFuture<T> : TaskWithValue<IEnumerable<T>> where T : ITask {
        private IEnumerable<T> _value;

        public override void InvokeFuture(BounceCommand command, IBounce bounce) {
            _value = GetTasks(bounce);

            foreach (var task in _value)
            {
                bounce.Invoke(command, task);
            }
        }

        public abstract IEnumerable<T> GetTasks(IBounce bounce);

        public override IEnumerable<T> GetValue()
        {
            return _value;
        }
    }

    public class DependentEnumerableFuture<TInput, TOutput> : EnumerableFuture<TOutput> where TOutput : ITask {
        [Dependency] private Future<IEnumerable<TInput>> InputValues;
        private Func<TInput, TOutput> GetValue;

        public DependentEnumerableFuture(Future<IEnumerable<TInput>> inputValues, Func<TInput, TOutput> getValue) {
            InputValues = inputValues;
            GetValue = getValue;
        }

        public override IEnumerable<TOutput> GetTasks(IBounce bounce) {
            return InputValues.Value.Select(v => GetValue(v));
        }
    }

    public class ManyDependentEnumerableFuture<TInput, TOutput> : EnumerableFuture<TOutput> where TOutput : ITask {
        [Dependency] private Future<IEnumerable<TInput>> InputValues;
        private Func<TInput, IEnumerable<TOutput>> GetValue;

        public ManyDependentEnumerableFuture(Future<IEnumerable<TInput>> inputValues, Func<TInput, IEnumerable<TOutput>> getValue) {
            InputValues = inputValues;
            GetValue = getValue;
        }

        public override IEnumerable<TOutput> GetTasks(IBounce bounce) {
            return InputValues.Value.SelectMany(v => GetValue(v));
        }
    }

    public static class DependentEnumerableFutureExtensions {
        public static Future<IEnumerable<TOutput>> SelectTasks<TInput, TOutput>(this Future<IEnumerable<TInput>> tasks, Func<TInput, TOutput> getResult) where TOutput : ITask {
            return new DependentEnumerableFuture<TInput, TOutput>(tasks, getResult);
        }

        public static Future<IEnumerable<TOutput>> SelectManyTasks<TInput, TOutput>(this Future<IEnumerable<TInput>> tasks, Func<TInput, IEnumerable<TOutput>> getResult) where TOutput : ITask {
            return new ManyDependentEnumerableFuture<TInput, TOutput>(tasks, getResult);
        }

        public static ITask OptionalTask<T>(this Future<bool> condition, Func<T> getOptionalTask) where T : ITask {
            return new OptionalTask<T>(condition, getOptionalTask);
        }

        public static ITask SelectTask<TInput, TTaskOutput>(this Future<TInput> input, Func<TInput, TTaskOutput> getTask) where TTaskOutput : ITask {
            return new SelectTask<TInput, TTaskOutput>(input, getTask);
        }
    }
}