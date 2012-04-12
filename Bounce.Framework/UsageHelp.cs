using System;
using System.Collections.Generic;
using System.IO;

namespace Bounce.Framework {
    public static class UsageHelp {
        public static void WriteAvailableTasks(TextWriter writer, IEnumerable<ITask> tasks)
        {
            foreach (var task in tasks)
            {
                writer.WriteLine();
                writer.WriteLine("    " + task.FullName);

                foreach (var parameter in task.Parameters)
                {
                    if (parameter.IsRequired) {
                        writer.WriteLine("        /{0}:{1}", parameter.Name, parameter.TypeDescription);
                    } else if (parameter.DefaultValue != null) {
                        writer.WriteLine("        /{0}:{1} = {2}", parameter.Name, parameter.TypeDescription, parameter.DefaultValue);
                    } else {
                        writer.WriteLine("        /{0}:{1} (optional)", parameter.Name, parameter.TypeDescription);
                    }
                }
            }

            writer.WriteLine();
        }

        public static void WriteUsage(TextWriter writer, IEnumerable<ITask> tasks) {
            writer.WriteLine("usage: bounce task [options]");
            writer.WriteLine();
            writer.WriteLine("tasks:");
            WriteAvailableTasks(writer, tasks);
        }
    }
}