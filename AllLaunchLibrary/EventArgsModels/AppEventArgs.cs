using System;

namespace AllLaunchLibrary.EventArgsModels
{
    public class AppEventArgs : EventArgs 
    {
		private readonly string _name, _args;

		public AppEventArgs(string name, string args) 
        {
			_name = name; 
			_args = args;
		}

		public string Name => _name;
		public string Args => _args;
	}
}
