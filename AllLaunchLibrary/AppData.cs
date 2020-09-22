using System;
using System.Collections.Generic;
using AllLaunchLibrary.Models;
using Newtonsoft.Json;
using System.IO;

namespace AllLaunchLibrary
{
    // Data management
    public static class LaunchData
    {
        // App info storage
        public static List<AppModel> AppModels { get; set; } = new List<AppModel>();

        // Save app info into .json file
        public static void SaveObj()
        {
            string json = JsonConvert.SerializeObject(AppModels);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json", json);
        }

        // Load app info from .json file
        public static void LoadObj()
        {
            string json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\apps.json");
            AppModels = JsonConvert.DeserializeObject<List<AppModel>>(json);
        }
    }
}