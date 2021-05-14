using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Phoenix.Net.Internal
{
    internal static class TaskHelpers
    {
        internal static CancellationTokenSource CreateLinkedCancellationTokenSource(CancellationToken token)
        {
            return token.CanBeCanceled ? CancellationTokenSource.CreateLinkedTokenSource(token) : new CancellationTokenSource();
        }

        internal static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, string errorMessage)
        {
            return WithTimeout(task, timeout, errorMessage, CancellationToken.None);
        }

        internal static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout, string errorMessage, CancellationToken token)
        {
            using (CancellationTokenSource cts = CreateLinkedCancellationTokenSource(token))
            {
                if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
                {
                    cts.Cancel();
                    return await task;
                }
            }

            // Ignore fault from task to avoid UnobservedTaskException
            task.IgnoreFault();

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            throw new TimeoutException(string.Format("{0}. Timeout: {1}.", errorMessage, timeout));
        }

        internal static Task WithTimeout(this Task task, TimeSpan timeout, string errorMessage)
        {
            return WithTimeout(task, timeout, errorMessage, CancellationToken.None);
        }

        internal static async Task WithTimeout(this Task task, TimeSpan timeout, string errorMessage, CancellationToken token)
        {
            using (CancellationTokenSource cts = CreateLinkedCancellationTokenSource(token))
            {
                if (task == await Task.WhenAny(task, Task.Delay(timeout, cts.Token)))
                {
                    cts.Cancel();
                    await task;
                    return;
                }
            }

            // Ignore fault from task to avoid UnobservedTaskException
            task.IgnoreFault();

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            throw new TimeoutException(string.Format("{0}. Timeout: {1}.", errorMessage, timeout));
        }

        internal static void IgnoreFault(this Task task)
        {
            if (task.IsCompleted)
            {
                var ignored = task.Exception;
            }
            else
            {
                task.ContinueWith
                    (t =>
                    {
                        var ignored = t.Exception;
                    },
                    TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously
                    );
            }
        }
    }
}
