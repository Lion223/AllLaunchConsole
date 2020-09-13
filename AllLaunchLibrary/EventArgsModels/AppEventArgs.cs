using System;
using AllLaunchLibrary.Models;

namespace AllLaunchLibrary.EventArgsModels
{
    public class AppEventArgs : EventArgs 
    {
		private readonly string _name, _pathToExe, _args;

		public AppEventArgs(string name) : this(name, "")
		{}

		public AppEventArgs(string name, string pathToExe) : this(name, pathToExe, "")
		{}

		public AppEventArgs(string name, string pathToExe, string args) 
        {
			_name = name; 
			_pathToExe = pathToExe;
			_args = args;
		}

		public string Name => _name;
		public string PathToExe => _pathToExe;
		public string Args => _args;
	}
}
