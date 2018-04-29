using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

/// <summary>
/// Pixels .NET namespace
/// </summary>
namespace PixelsNET
{
    /// <summary>
    /// Singleton class
    /// </summary>
    public static class Singleton
    {
        /// <summary>
        /// Listener thread
        /// </summary>
        private static Thread listenerThread;

        /// <summary>
        /// Keep listener running
        /// </summary>
        private static bool keepListenerRunning = true;

        /// <summary>
        /// Command line arguments
        /// </summary>
        private static List<string> commandLineArguments = new List<string>();

        /// <summary>
        /// Fetch command line arguments
        /// </summary>
        public static string[] FetchCommandLineArguments
        {
            get
            {
                string[] ret = null;
                lock (commandLineArguments)
                {
                    ret = commandLineArguments.ToArray();
                    commandLineArguments.Clear();
                }
                return ret;
            }
        }

        /// <summary>
        /// Prior process
        /// </summary>
        private static Process PriorProcess
        {
            get
            {
                Process ret = null;
                Process current_process = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(current_process.ProcessName);
                foreach (Process process in processes)
                {
                    if ((process.Id != current_process.Id) && (process.StartTime < current_process.StartTime) && (process.MainModule.FileName == current_process.MainModule.FileName))
                    {
                        if (ret == null)
                        {
                            ret = process;
                        }
                        else if (ret.StartTime > process.StartTime)
                        {
                            ret = process;
                        }
                    }
                }
                return ret;
            }
        }

        /// <summary>
        /// Start as a singleton
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>"true" if this is the main process, otherwise "false"</returns>
        public static bool StartAsSingleton(string[] args)
        {
            bool ret = true;
            Process prior_process = PriorProcess;
            if (prior_process == null)
            {
                listenerThread = new Thread(() =>
                {
                    Process current_process = Process.GetCurrentProcess();
                    while (keepListenerRunning)
                    {
                        using (StreamReader reader = new StreamReader(Console.OpenStandardInput()))
                        {
                            string command_line_argument = reader.ReadLine();
                            lock (commandLineArguments)
                            {
                                commandLineArguments.Add(command_line_argument);
                            }
                        }
                    }
                });
            }
            else
            {
                
                if ((args != null) && (prior_process.StandardInput != null))
                {
                    foreach (string arg in args)
                    {
                        if (arg != null)
                        {
                            prior_process.StandardInput.WriteLine(arg);
                        }
                    }
                    prior_process.StandardInput.Flush();
                }
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// Cleans everything up
        /// </summary>
        public static void CleanUp()
        {
            keepListenerRunning = false;
            if (listenerThread != null)
            {
                listenerThread.Abort();
                //listenerThread.Join();
                listenerThread = null;
            }
        }
    }
}
