using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GeneratorOfRandomNumber
{
    //Task 3. Mutex using tasks and async/await. The main difference with System.Threading.Mutex 
    //is that this mutex must not block current thread (for using in UI thread)
    public class MyMutex : IMyMutex
    {
        public readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private Dispatcher _userDispatcher;
        private readonly Task<IDisposable> _releaser;

        public MyMutex()
        {
            _releaser = Task.FromResult((IDisposable) new Releaser(this));
        }

        //Return a Task, wich will be completed only when mutex will become free
        public async Task Lock()
        {
            await Task.Factory.StartNew(() =>
            {
                Semaphore.WaitAsync();
                Console.WriteLine(@"ThreadOne, executing ThreadMethod, " +
                                  @"is {0} from the thread pool.",
                    Thread.CurrentThread.ManagedThreadId);
                _userDispatcher = Dispatcher.CurrentDispatcher;
            });
        }

        //Task 4. Allow using mutex by way, showed in example
        public Task<IDisposable> LockSection()
        {
            var wait = Semaphore.WaitAsync();
            return wait.IsCompleted
                ? _releaser
                : wait.ContinueWith((_, state) => (IDisposable)state,
                    _releaser.Result, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        // Release mutex
        public void Release()
        {
            try
            {
                if (Semaphore != null && _userDispatcher != null)
                {
                    _userDispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                    {
                        Console.WriteLine(@"ThreadOne, executing ThreadMethod, " +
                                          @"is {0} from the thread pool.",
                            Thread.CurrentThread.ManagedThreadId);
                        Console.WriteLine(@"Releasing");
                        Semaphore.Release();
                    }));
                }
                else
                {
                    Console.WriteLine(@"Null");
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }
    }
}
