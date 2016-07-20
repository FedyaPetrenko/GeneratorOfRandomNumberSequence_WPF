using System.Threading.Tasks;

namespace GeneratorOfRandomNumber
{
    //Mutex`s interface
    internal interface IMyMutex
    {
        // Returns a Task, wich will be completed only when mutex will become free
        Task Lock();

        // Release mutex
        void Release();
    }
}
