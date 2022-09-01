using System;
using System.Threading.Tasks;
using Betauer.Signal;
using Godot;

namespace Betauer {
    public static class TaskExtensions {
        
        // TODO: tests

        public static async Task<object[]> RealTimeout(this SignalAwaiter awaiter, float seconds) {
            return await OnRealTimeout(awaiter, seconds, () => throw new TimeoutException());
        }

        public static async Task<object[]> OnRealTimeout(this SignalAwaiter awaiter, float seconds, Action action) {
            return await OnRealTimeout(awaiter, seconds, () => {
                action();
                return null;
            });
        }

        public static async Task<object[]> OnRealTimeout(this SignalAwaiter awaiter, float seconds,
            Func<object[]> action) {
            Func<Task> userTaskFactory = async () => await awaiter;
            await Task.WhenAny(userTaskFactory.Invoke(),
                Task.Delay((int)TimeSpan.FromSeconds(seconds).TotalMilliseconds));
            return awaiter.IsCompleted ? awaiter.GetResult() : action();
        }

        public static async Task<object[]> Timeout(this SignalAwaiter awaiter, float seconds) {
            return await OnTimeout(awaiter, seconds, () => throw new TimeoutException());
        }

        public static async Task<object[]> OnTimeout(this SignalAwaiter awaiter, float seconds, Action action) {
            return await OnTimeout(awaiter, seconds, () => {
                action();
                return null;
            });
        }

        public static async Task<object[]> OnTimeout(this SignalAwaiter awaiter, float seconds, Func<object[]> action) {
            Func<Task> userTaskFactory = async () => await awaiter;
            Func<Task> timeoutTaskFactory =
                async () => await SceneTreeHolder.SceneTree.CreateTimer(seconds).AwaitTimeout();
            await Task.WhenAny(userTaskFactory.Invoke(), timeoutTaskFactory.Invoke());
            return awaiter.IsCompleted ? awaiter.GetResult() : action();
        }
    }
}