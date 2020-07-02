using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Management;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AllLaunchLibrary
{
    public class LaunchManager
    {
        public event EventHandler<AppEventArgs> AppLaunched;
        public event EventHandler<AppEventArgs> AppAlreadyRunning;
        public List<AppModel> AppModels { get; set; } = new List<AppModel>();

        public void AddApp(string name, string pathToExe, string args)
        {
            AppModel app = new AppModel(name, pathToExe, args);
            AppModels.Add(app);
        }

        public string GetLaunchList()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < AppModels.Count; i++)
            {
                sb.Append($"[{i}]: {AppModels[i].Name}" + '\n');
            }

            return sb.ToString();
        }

        public void Start()
        {
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
        }

        private bool ProcessExists(AppModel app)
        {
            Process[] processList = Process.GetProcesses();

            try
            {
                foreach (var process in processList)
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * " +
                    "FROM Win32_Process " +
                    "WHERE ParentProcessId=" + process.Id);
                    ManagementObjectCollection collection = searcher.Get();

                    foreach (var item in collection)
                    {
                        Process childProcess = Process.GetProcessById((int)item["ProcessId"]);

                        if (childProcess.MainModule.FileName == app.PathToExe)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (SystemException) {}


            /*
            Process[] processList = Process.GetProcesses();

            foreach (var process in processList)
            {
                try
                {
                    if (process.MainModule.FileName == app.PathToExe)
                    {
                        return true;
                    }
                    else
                    {
                        ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                        "SELECT * " +
                        "FROM Win32_Process " +
                        "WHERE ParentProcessId=" + process.Id);
                        ManagementObjectCollection collection = searcher.Get();

                        foreach (var item in collection)
                        {
                            Process p = Process.GetProcessById()
                        }
                    }



                    if (collection.Count > 0)
                    {
                        return true;
                    }
                }
                catch(SystemException) { }
            }
            */

            return false;
        }

        public void SaveObj()
        {
            string json = JsonConvert.SerializeObject(AppModels);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json", json);
        }

        public void LoadObj()
        {
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json");
            AppModels = JsonConvert.DeserializeObject<List<AppModel>>(json);
        }

        protected virtual void OnAppLaunched(AppEventArgs e)
        {
            AppLaunched?.Invoke(this, e);
        }

        protected virtual void OnAppAlreadyRunning(AppEventArgs e)
        {
            AppAlreadyRunning?.Invoke(this, e);
        }
    }
}
