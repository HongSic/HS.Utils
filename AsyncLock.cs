using System;
using System.Threading;
using System.Threading.Tasks;

namespace HS.Utils
{
    /// async인 맥락에서의 lock을 제공한다.  
    /// Lock 해제를 위해 반드시 처리 완료 후에 LockAsync가 생성한 IDisposable을 Dispose 한다. 
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public IDisposable Lock()
        {
            _semaphore.Wait();
            return new Handler(_semaphore);
        }
        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new Handler(_semaphore);
        }

        private sealed class Handler : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed = false;

            public Handler(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore.Release();
                    _disposed = true;
                }
            }
        }
    }
}
