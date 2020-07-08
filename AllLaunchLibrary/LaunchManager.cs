using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AllLaunchLibrary
{
    public sealed class LaunchManager
    {
        // Events notifying about app launching state
        public event EventHandler<AppEventArgs> AppLaunched;
        public event EventHandler<AppEventArgs> AppAlreadyRunning;
        // App info storage
        public List<AppModel> AppModels { get; set; } = new List<AppModel>();
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
            AppModels.Add(app);
        }

        // Get string list of added apps
        public string GetLaunchList()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < AppModels.Count; i++)
            {
                sb.Append($"[{i}]: {AppModels[i].Name}" + '\n');
            }

            return sb.ToString();
        }

        // Launch apps from the storage
        public bool Start()
        {
            if (AppModels.Count == 0)
            {
                return false;
            }
            
            AppEventArgs e = null;

            foreach (var app in AppModels)
            {
                if (ProcessExists(app))
                {
                    e = new AppEventArgs(app.Name, app.Args);
                    OnAppAlreadyRunning(e);
                }
                else
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

                    e = new AppEventArgs(app.Name, app.Args);
                    OnAppLaunched(e);
                }
            }

            return true;
        }

        // Check if app's process is launched
        private bool ProcessExists(AppModel app)
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

        // Save app info into .json file
        public void SaveObj()
        {
            string json = JsonConvert.SerializeObject(AppModels);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json", json);
        }

        // Load app info from .json file
        public void LoadObj()
        {
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json");
            AppModels = JsonConvert.DeserializeObject<List<AppModel>>(json);
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
