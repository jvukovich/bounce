using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Framework;
using Bounce.Framework.VisualStudio;

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
    }
}
