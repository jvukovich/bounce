using System;
using System.Collections.Generic;
using System.Text;

namespace Bounce.Framework
{
    public static class UsageHelp
    {
        public static void WriteUsage()
        {
            Console.WriteLine("usage: bounce task [options]");
        }
        
        public static void WriteUsage(IEnumerable<ITask> tasks)
        {
            WriteUsage();
            Console.WriteLine();
            Console.WriteLine("tasks:");
            Console.Write(GetAvailableTasks(tasks));
        }

        public static string GetAvailableTasks(IEnumerable<ITask> tasks)
        {
            var sb = new StringBuilder();

            foreach (var task in tasks)
            {
                if (task == null || task.Parameters == null)
                    continue;

                sb.AppendLine();
                sb.AppendLine("    " + task.FullName);

                foreach (var parameter in task.Parameters)
                {
                    if (parameter.IsRequired)
                        sb.AppendFormat("        /{0}:{1}", parameter.Name, parameter.TypeDescription).AppendLine();
                    else if (parameter.DefaultValue != null)
                        sb.AppendFormat("        /{0}:{1} = {2}", parameter.Name, parameter.TypeDescription, parameter.DefaultValue).AppendLine();
                    else
                        sb.AppendFormat("        /{0}:{1} (optional)", parameter.Name, parameter.TypeDescription).AppendLine();
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}