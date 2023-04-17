using System;
using System.Threading.Tasks;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core; 

public static class TaskExtensions {
    public static Task OnCompleted(this Task task, Action action) => OnCompleted(task, _ => action());
    public static Task OnCancel(this Task task, Action action) => OnCancel(task, _ => action());
    public static Task OnException(this Task task, Action<Exception> action, bool rethrow = false) =>
        task.OnException(t => {
            var ex = task.Exception?.InnerException;
            if (ex != null) {
                action(ex);
                if (rethrow) throw ex;
            }
        });        
        
    public static Task OnCompleted(this Task task, Action<Task> action) => task.ContinueWith(action, TaskContinuationOptions.OnlyOnRanToCompletion);        
    public static Task OnCancel(this Task task, Action<Task> action) => task.ContinueWith(action, TaskContinuationOptions.OnlyOnCanceled);
    public static Task OnException(this Task task, Action<Task> action) => task.ContinueWith(action, TaskContinuationOptions.OnlyOnFaulted);

        
    // TODO: tests
    public static async Task<Variant[]> RealTimeout(this SignalAwaiter awaiter, float seconds) {
        return await OnRealTimeout(awaiter, seconds, () => throw new TimeoutException());
    }

    public static async Task<Variant[]> OnRealTimeout(this SignalAwaiter awaiter, float seconds, Action action) {
        return await OnRealTimeout(awaiter, seconds, () => {
            action();
            return null;
        });
    }

    public static async Task<Variant[]> OnRealTimeout(this SignalAwaiter awaiter, float seconds,
        Func<Variant[]> action) {
        Func<Task> userTaskFactory = async () => await awaiter;
        await Task.WhenAny(userTaskFactory.Invoke(),
            Task.Delay((int)TimeSpan.FromSeconds(seconds).TotalMilliseconds));
        return awaiter.IsCompleted ? awaiter.GetResult() : action();
    }

    public static async Task<Variant[]> Timeout(this SignalAwaiter awaiter, SceneTree sceneTree, float seconds) {
        return await OnTimeout(awaiter, sceneTree, seconds, () => throw new TimeoutException());
    }

    public static async Task<Variant[]> OnTimeout(this SignalAwaiter awaiter, SceneTree sceneTree, float seconds, Action action) {
        return await OnTimeout(awaiter, sceneTree, seconds, () => {
            action();
            return null;
        });
    }

    public static async Task<Variant[]> OnTimeout(this SignalAwaiter awaiter, SceneTree sceneTree, float seconds, Func<Variant[]> action) {
        Func<Task> userTaskFactory = async () => await awaiter;
        Func<Task> timeoutTaskFactory =
            async () => await sceneTree.CreateTimer(seconds).AwaitTimeout();
        await Task.WhenAny(userTaskFactory.Invoke(), timeoutTaskFactory.Invoke());
        return awaiter.IsCompleted ? awaiter.GetResult() : action();
    }
}