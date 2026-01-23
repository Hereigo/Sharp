using System;
using System.Threading;

namespace Test_Async
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread myThread = new Thread(MyMethod);
            myThread.Start();
            
            ThreadPool.QueueUserWorkItem(MyMethod);
            
            while (true)
            {
                Console.WriteLine("Main thread is running...");
                Thread.Sleep(50);
            }
        }

        private static void MyMethod(object _)
        {
            throw new Exception("Exception in the Second Thread.");
        }
    }
}
