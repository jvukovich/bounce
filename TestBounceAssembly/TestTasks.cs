using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bounce.Framework;
using Bounce.Framework.VisualStudio;

namespace TestBounceAssembly {
	public class TestTasks {
		[Task]
		public void Task1() {
			Console.WriteLine("task1");
		}

		[Task]
		public void Task2(string a) {
			Console.WriteLine("task2 a=" + a);
		}

		[Task]
		public void Task3() {
			Console.WriteLine("task3 a=" + Props.Get("a"));
		}

		[Task]
		public void Task4() {
			Console.WriteLine("task4 b=" + Props.Get("b"));
		}

		[Task]
		public void Task5() {
			Console.WriteLine("task5 a={0} b={1} c={2}", Props.Get("a"), Props.Get("b"), Props.Get("c"));
		}
	}
}