using System;
using AllLaunchLibrary.Models;
using System.Diagnostics;

namespace AllLaunchLibrary
{
    // Application process control
    public static class AppProcess
    {
        // Check if the process already exists
        public static bool ProcessExists(AppModel app)
        {
            Process[] processList = Process.GetProcesses();

            foreach (var process in processList)
            {
                try
                {
                    if (process.MainModule.FileName == app.PathToExe)
                    {
                        return true;
                    }
                }
                catch (SystemException) { }
            }

            return false;
        }

        // Launch a process with returning results
        public static bool ProcessLaunch(AppModel app)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = app.PathToExe, 
                Arguments = app.Args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            try
            {
                Process.Start(psi);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return false;
            }

            return true;
        }
    }
}