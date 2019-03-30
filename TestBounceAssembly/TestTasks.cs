using System;
using Bounce.Framework;

namespace TestBounceAssembly
{
    public class TestTasks
    {
        [Task]
        public void Task1() {
            Console.WriteLine("task1");
        }

        [Task]
        public void Task2(string a) {
            Console.WriteLine("task2 a=" + a);
        }

        [Task]
        public void Task3(string a) {
            Console.WriteLine("task3 a=" + a);
        }

        [Task]
        public void Task4(string b) {
            Console.WriteLine("task4 b=" + b);
        }

        [Task]
        public void Task5(string a, string b, string c) {
            Console.WriteLine("task5 a={0}, b={1}, c={2}", a, b, c);
        }
    }
}