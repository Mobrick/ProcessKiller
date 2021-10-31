using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ProcessKiller
{
    class Program
    {
        static void Main(string[] args)
        {
            // Checking if args massive has sufficient number of input parameters
            if (args.Length < 3 )
            {
                Console.WriteLine("Not enough input parameters."); 
                return;
            }

            string processName = args[0];

            // Checking if lifespan number of minutes could be parsed to int
            if (!Int32.TryParse(args[1], out int maxLifespan))
            {
                Console.WriteLine("Could not read the maximum lifespan number of minutes.");
                return;
            }
            // Checking if number of minutes for lifespan check interval could be parsed to int
            if (!Int32.TryParse(args[2], out int checkTimer))
            {
                Console.WriteLine("Could not read the number of minutes for lifespan check interval.");
                return;
            }

            // Finding a process by the name requested
            var process = Process.GetProcesses().FirstOrDefault(p => p.ProcessName == processName);
            if (process == null)
            {
                Console.WriteLine($"Process with the name: \"{processName}\" was not found.");
                return;
            }
            
            ProcessKiller(process, maxLifespan, checkTimer);
                
            AddNoteToLog($"Process : \"{processName}\" has been killed.");
        }

        /// <summary>
        /// Killing the process if it lives more then in the requested number of minutes.
        /// Checks the process lifespan in the requested time interval.
        /// </summary>
        /// <param name="process"> Process to be killed. </param>
        /// <param name="maxLifespan"> Requested lifespan number of minutes. </param>
        /// <param name="checkTimer"> Requested number of minutes for lifespan check interval. </param>
        private static void ProcessKiller(Process process, int maxLifespan, int checkTimer)
        {
            DateTime startTime = process.StartTime;
            while (DateTime.Now < startTime.AddMinutes(maxLifespan))
            {
                Console.WriteLine("Process started {0} seconds ago.",
                    (startTime.AddMinutes(maxLifespan) - DateTime.Now).TotalSeconds);
                Thread.Sleep(TimeSpan.FromMinutes(checkTimer));
            }

            process.Kill();
        }

        /// <summary>
        /// Adds note to log txt file.
        /// Creates file in "c:\temp" folder if it doesn't exist.
        /// </summary>
        /// <param name="str"> Note to be added. </param>
        static void AddNoteToLog(string str)
        {
            string path = @"c:\temp\ProcessKillerLogs.txt";
            using StreamWriter sw = new StreamWriter(path, true);
            sw.Write(str);
        }
    }
}
