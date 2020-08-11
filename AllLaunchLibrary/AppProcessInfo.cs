using System;
using AllLaunchLibrary.Models;
using System.Diagnostics;

namespace AllLaunchLibrary
{
    public static class AppProcessInfo
    {
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

        public static void ProcessLaunch(AppModel app)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = app.PathToExe, 
                Arguments = app.Args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            Process.Start(psi);
        }
    }
}