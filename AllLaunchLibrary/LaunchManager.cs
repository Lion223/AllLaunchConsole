using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AllLaunchLibrary.Models;
using AllLaunchLibrary.EventArgsModels;

namespace AllLaunchLibrary
{
    public sealed class LaunchManager
    {
        // Events notifying about app launching state
        public event EventHandler<AppEventArgs> AppLaunched;
        public event EventHandler<AppEventArgs> AppAlreadyRunning;
        // Singleton implementation
        private static readonly Lazy<LaunchManager> lazy 
            = new Lazy<LaunchManager>(() => new LaunchManager());
        private LaunchManager() {}
        public static LaunchManager Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        // Add app's info in the storage
        public void AddApp(string name, string pathToExe, string args)
        {
            AppModel app = new AppModel(name, pathToExe, args);
            LaunchData.AppModels.Add(app);
        }

        // Get string list of added apps
        public string GetLaunchList()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < LaunchData.AppModels.Count; i++)
            {
                sb.Append($"[{i}]: {LaunchData.AppModels[i].Name}" + '\n');
            }

            return sb.ToString();
        }

        // Launch apps from the storage
        public bool Start()
        {
            if (LaunchData.AppModels.Count == 0)
            {
                return false;
            }
            
            AppEventArgs e = null;

            foreach (var app in LaunchData.AppModels)
            {
                // Check if app's process is launched
                if (AppProcessInfo.ProcessExists(app))
                {
                    e = new AppEventArgs(app.Name, app.Args);
                    OnAppAlreadyRunning(e);
                }
                else
                {
                    AppProcessInfo.ProcessLaunch(app);

                    e = new AppEventArgs(app.Name, app.Args);
                    OnAppLaunched(e);
                }
            }

            return true;
        }

        // Notify listener about app's launched state
        private void OnAppLaunched(AppEventArgs e)
        {
            AppLaunched?.Invoke(this, e);
        }

        // Notify listener about app's already launched state
        private void OnAppAlreadyRunning(AppEventArgs e)
        {
            AppAlreadyRunning?.Invoke(this, e);
        }
    }
}
