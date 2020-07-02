using System;

namespace AllLaunchLibrary
{
    public class AppEventArgs : EventArgs 
    {
		private readonly string _name, _args;

		public AppEventArgs(string name, string args) 
        {
			_name = name; _args = args;
		}

		public string Name { get { return _name; } }
		public string Args { get { return _args; } }
	}
}
