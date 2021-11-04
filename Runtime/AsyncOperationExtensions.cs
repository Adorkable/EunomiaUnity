using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EunomiaUnity
{
    public static class AsyncOperationExtensions
    {
        public static UniTask<AsyncOperation>.Awaiter GetAwaiter(this AsyncOperation asyncOperation)
        {
            var completionSource = new UniTaskCompletionSource<AsyncOperation>();
            asyncOperation.completed += (result) =>
            {
                completionSource.TrySetResult(result);
            };
            return completionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter<AsyncOperation> GetSystemAwaiter(this AsyncOperation asyncOperation)
        {
            var completionSource = new TaskCompletionSource<AsyncOperation>();
            asyncOperation.completed += (result) =>
            {
                completionSource.SetResult(result);
            };
            return completionSource.Task.GetAwaiter();
        }
    }
}
