using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace EunomiaUnity
{
    public static class AsyncOperationExtensions
    {
        public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation asyncOperation)
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
