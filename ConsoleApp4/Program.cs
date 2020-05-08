using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {

        static object lock1 = new object();
        static object lock2 = new object();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            var task1 = Transfer2(lock1, lock2, 500);
            var task2 = Transfer2(lock2, lock1, 600);
            Task.WaitAll(task1, task2);
            Console.WriteLine("Finished...");
        }

        static private Task Transfer(object acc1, object acc2, int sum)
        {
            var task = Task.Run(() =>
            {
                lock (acc1)
                {
                    Thread.Sleep(1000);
                    lock (acc2)
                    {
                        Console.WriteLine($"Finished transfering sum {sum}");
                    }
                }
            });
            return task;
        }

        static private Task Transfer2(object acc1, object acc2, int sum)
        {
            var task = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        bool entered = Monitor.TryEnter(acc1, TimeSpan.FromMilliseconds(100));
                        if (!entered) continue;
                        Thread.Sleep(1000);
                        entered = Monitor.TryEnter(acc2, TimeSpan.FromMilliseconds(100));
                        if (!entered) continue;

                        //do operation
                        Console.WriteLine($"Finished transferring sum {sum}");
                        break;
                    }
                    finally
                    {
                        if (Monitor.IsEntered(acc1)) Monitor.Exit(acc1);
                        if (Monitor.IsEntered(acc2)) Monitor.Exit(acc2);
                        Thread.Sleep(200);
                    }
                }
                
            });
            return task;
        }

    }
}
