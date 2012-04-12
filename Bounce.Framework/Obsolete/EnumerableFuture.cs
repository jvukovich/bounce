using System;
using System.Collections.Generic;
using System.Linq;

namespace Bounce.Framework.Obsolete
{
    public abstract class EnumerableFuture<T> : TaskWithValue<IEnumerable<T>> where T : IObsoleteTask {
        private IEnumerable<T> _value;

        public override void InvokeTask(IBounceCommand command, IBounce bounce) {
            _value = GetTasks(bounce);

            foreach (var task in _value)
            {
                bounce.Invoke(command, task);
            }
        }

        public abstract IEnumerable<T> GetTasks(IBounce bounce);

        protected override IEnumerable<T> GetValue()
        {
            return _value;
        }
    }

    public class DependentEnumerableFuture<TInput, TOutput> : EnumerableFuture<TOutput> where TOutput : IObsoleteTask {
        [Dependency] private Task<IEnumerable<TInput>> InputValues;
        private Func<TInput, TOutput> GetTask;

        public DependentEnumerableFuture(Task<IEnumerable<TInput>> inputValues, Func<TInput, TOutput> getTask) {
            InputValues = inputValues;
            GetTask = getTask;
        }

        public override IEnumerable<TOutput> GetTasks(IBounce bounce) {
            return InputValues.Value.Select(v => GetTask(v));
        }
    }

    public class ManyDependentEnumerableFuture<TInput, TOutput> : EnumerableFuture<TOutput> where TOutput : IObsoleteTask {
        [Dependency] private Task<IEnumerable<TInput>> InputValues;
        private Func<TInput, IEnumerable<TOutput>> GetManyTasks;

        public ManyDependentEnumerableFuture(Task<IEnumerable<TInput>> inputValues, Func<TInput, IEnumerable<TOutput>> getManyTasks) {
            InputValues = inputValues;
            GetManyTasks = getManyTasks;
        }

        public override IEnumerable<TOutput> GetTasks(IBounce bounce) {
            return InputValues.Value.SelectMany(v => GetManyTasks(v));
        }
    }

    public static class DependentEnumerableFutureExtensions {
        public static Task<IEnumerable<TOutput>> SelectTasks<TInput, TOutput>(this Task<IEnumerable<TInput>> tasks, Func<TInput, TOutput> getResult) where TOutput : IObsoleteTask {
            return new DependentEnumerableFuture<TInput, TOutput>(tasks, getResult);
        }

        public static Task<IEnumerable<TOutput>> Select<TInput, TOutput>(this Task<IEnumerable<TInput>> tasks, Func<TInput, TOutput> getResult)
        {
            return tasks.WhenBuilt(t => t.Select(getResult));
        }

        public static Task<IEnumerable<TInput>> Where<TInput>(this Task<IEnumerable<TInput>> tasks, Func<TInput, bool> getResult)
        {
            return tasks.WhenBuilt(t => t.Where(getResult));
        }

        public static Task<IEnumerable<TOutput>> SelectManyTasks<TInput, TOutput>(this Task<IEnumerable<TInput>> tasks, Func<TInput, IEnumerable<TOutput>> getResult) where TOutput : IObsoleteTask {
            return new ManyDependentEnumerableFuture<TInput, TOutput>(tasks, getResult);
        }

        public static IObsoleteTask IfTrue<T>(this Task<bool> condition, T optionalTask) where T : IObsoleteTask {
            return new OptionalTask<T>(condition, () => optionalTask, false);
        }

        public static IObsoleteTask IfTrue<T>(this Task<bool> condition, T ifTrueTask, T ifFalseTask) where T : IObsoleteTask {
            return new All(
                new OptionalTask<T>(condition, () => ifTrueTask, false),
                new OptionalTask<T>(condition, () => ifFalseTask, true)
            );
        }

        public static IObsoleteTask IfFalse<T>(this Task<bool> condition, T optionalTask) where T : IObsoleteTask {
            return new OptionalTask<T>(condition, () => optionalTask, true);
        }

        public static IObsoleteTask IfFalse<T>(this Task<bool> condition, T ifFalseTask, T ifTrueTask) where T : IObsoleteTask {
            return condition.IfTrue(ifTrueTask, ifFalseTask);
        }

        public static IObsoleteTask SelectTask<TInput, TTaskOutput>(this Task<TInput> input, Func<TInput, TTaskOutput> getTask) where TTaskOutput : IObsoleteTask {
            return new SelectTask<TInput, TTaskOutput>(input, getTask);
        }
    }
}