using System;
using System.Threading;

namespace Parikhmaher
{
    class Program
    {

        private static int client = 25;

        private static int _ClientsWaitingCount = 0;
        private static int _seatCount = 5;
        private static bool _cutting = false;

        private static object _ClientsWaitingCountLockObject = new object();
        private static Semaphore _ClientsMustWait = new Semaphore(0, 5);
        private static Mutex _HairdresserMustWait = new Mutex();
        private static readonly Random _random = new Random();

        static void Main(string[] args)
        {
            Thread[] clients = new Thread[client];
            for (int i = 0; i < client; i++)
            {
                clients[i] = new Thread(GoGetHaircut);
            }

            foreach (var t in clients)
            {
                t.Start();
            }

            foreach (var t in clients)
            {
                t.Join();
            }
        }

        static void GoGetHaircut()
        {
            Thread.Sleep(_random.Next(1, 10000));

            if (_ClientsWaitingCount < _seatCount)
            {
                if (_cutting)
                {
                    ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId,
                        " хочет подстричься, но парикхмахер занят!");
                    
                    IncreaseClientsWaitingCount();
                    _ClientsMustWait.WaitOne();
                    
                    DecreaseClientsWaitingCount();
                }

                Cutting();
            }
            else
            {
                ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId,
                        " не доволен, сервис отвратительный!!!");
            }
        }

        static void Cutting()
        {
            _HairdresserMustWait.WaitOne();
            _cutting = true;
            var cuttingTime = _random.Next(500, 1500);

            ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId, " начал стричся");

            Thread.Sleep(cuttingTime);

            ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId, " постригся");

            _HairdresserMustWait.ReleaseMutex();
            _cutting = false;

            if (_ClientsWaitingCount != 0)
                _ClientsMustWait.Release(1);
        }

        public static void IncreaseClientsWaitingCount()
        {
            lock (_ClientsWaitingCountLockObject)
            {
                _ClientsWaitingCount++;
                ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId,
                    " увеличил значение очереди до " + _ClientsWaitingCount);
            }
        }



        public static void DecreaseClientsWaitingCount()
        {
            lock (_ClientsWaitingCountLockObject)
            {
                _ClientsWaitingCount--;
                ConsoleHelper.WriteToConsole("Клиент" + Thread.CurrentThread.ManagedThreadId,
                    " уменьшил значение очереди до " + _ClientsWaitingCount);
            }
        }
    }
}
