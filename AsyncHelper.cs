using System;
using System.Diagnostics;
using System.Threading.Tasks;

public static class AsyncHelper
{
    private static bool _isAsyncEnvironment;

    static AsyncHelper()
    {
        _isAsyncEnvironment = SynchronizationContext.Current == null;
    }

    public static void RunAsync(Func<Task> asyncAction, Action callback = null)
    {
        if (_isAsyncEnvironment)
        {
            _ = RunAsyncInternal(asyncAction, callback);
        }
        else
        {
            asyncAction().GetAwaiter().GetResult();
            callback?.Invoke();
        }
    }

    public static T RunAsync<T>(Func<Task<T>> asyncFunc, Action<T> callback = null)
    {
        if (_isAsyncEnvironment)
        {
            var task = RunAsyncInternal(asyncFunc, callback);
            return task.GetAwaiter().GetResult();
        }
        else
        {
            var result = asyncFunc().GetAwaiter().GetResult();
            callback?.Invoke(result);
            return result;
        }
    }

    public static async Task RunWithErrorHandlingAsync(Func<Task> asyncAction, Action onSuccess = null, Action<Exception> onError = null)
    {
        try
        {
            await asyncAction();
            onSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
            Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static async Task RunAsyncInternal(Func<Task> asyncAction, Action callback)
    {
        await asyncAction();
        callback?.Invoke();
    }

    private static async Task<T> RunAsyncInternal<T>(Func<Task<T>> asyncFunc, Action<T> callback)
    {
        var result = await asyncFunc();
        callback?.Invoke(result);
        return result;
    }
}
