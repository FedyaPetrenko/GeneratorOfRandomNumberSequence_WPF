using System;

namespace GeneratorOfRandomNumber
{
    sealed class Releaser : IDisposable
    {
        private readonly MyMutex _mToRelease;

        internal Releaser(MyMutex toRelease)
        {
            _mToRelease = toRelease;
        }

        public void Dispose()
        {
            _mToRelease.Semaphore.Dispose();
        }
    }
}
