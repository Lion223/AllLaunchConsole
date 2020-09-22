using System;
using System.Collections.Generic;
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
        public event EventHandler<AppEventArgs> AppFileIsNotFound;
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

        // Private method for Start
        private void LaunchApp(AppModel app)
        {
            AppEventArgs e = new AppEventArgs(app.Name, app.PathToExe, app.Args);

                // Check if app's process is launched
                if (AppProcess.ProcessExists(app))
                {
                    OnAppAlreadyRunning(e);
                }
                else
                {
                    if (AppProcess.ProcessLaunch(app))
                    {
                        OnAppLaunched(e);
                    }
                }
        }

        // Launch apps from the storage
        public bool Start()
        {
            if (LaunchData.AppModels.Count == 0)
            {
                return false;
            }

            List<AppModel> falsePaths;

            if (!Validator.AllPathsExist(out falsePaths))
            {
                AppEventArgs e = null;

                foreach (var app in falsePaths)
                {
                    e = new AppEventArgs(app.Name, app.PathToExe, app.Args); 

                    OnAppFileIsNotFound(e);
                }
            }

            foreach (var app in LaunchData.AppModels)
            {
                LaunchApp(app);
            }

            Console.WriteLine();
            MessageTemplate.LaunchProcessFinished();

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

        // Notify listener about app's .exe file is not existing
        private void OnAppFileIsNotFound(AppEventArgs e)
        {
            AppFileIsNotFound?.Invoke(this, e);
        }
    }
}
