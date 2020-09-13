using System;
using System.Collections.Generic;
using System.IO;
using AllLaunchLibrary.Models;

namespace AllLaunchLibrary
{
    public static class Validator
    {
        public static bool PathsExist(out List<AppModel> falsePaths)
        {
            falsePaths = new List<AppModel>();
            
            foreach (var app in LaunchData.AppModels)
            {
                if (!File.Exists(app.PathToExe))
                {
                    falsePaths.Add(app);
                }      
            }

            if (falsePaths.Count != 0)
            {
                return false;
            }

            return true;
        }
    }
}