using System;

namespace AllLaunchLibrary.Models
{
    public class AppModel
    {
        public string Name { get; set; }
        public string PathToExe { get; set; }
        public string Args { get; set; }

        public AppModel(string name, string pathToExe, string args)
        {
            Name = name;
            PathToExe = pathToExe;
            Args = args;
        }
    }
}
