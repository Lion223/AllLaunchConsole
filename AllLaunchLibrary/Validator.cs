using System.Collections.Generic;
using System.IO;
using AllLaunchLibrary.Models;
using System.Linq;

namespace AllLaunchLibrary
{

    // Validates input data
    public static class Validator
    {
        // Check before launching apps if all .exe paths are existing
        public static bool AllPathsExist(out List<AppModel> falsePaths)
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

        // Check if app already exists in the list
        public static bool AppExists(string name, string pathToExe)
        {
            return LaunchData.AppModels.Any(item => item.Name == name || item.PathToExe == pathToExe);
        }
    }
}