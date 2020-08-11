using System;

namespace AllLaunchLibrary
{
    public class NoMenuArgument : IArgument
    {
        public string Signature { get; } = "-n";

        public void Operate()
        {
            LaunchData.LoadObj();
            LaunchManager.Instance.Start();
            Environment.Exit(0);
        }
    }
}