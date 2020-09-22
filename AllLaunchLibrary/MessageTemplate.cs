using System;

namespace AllLaunchLibrary
{
    // Templates for console messages
    public static class MessageTemplate
    {
        public static void LaunchProcessFinished()
        {
            Console.WriteLine("All apps are launched");
        }

        public static void AppAlreadyExists()
        {
            Console.WriteLine("Application already exists in the list"); 
        }

        public static void WrongInput()
        {
            Console.WriteLine("Wrong input");
        }

        public static void Arguments()
        {
            Console.WriteLine
            (
                "Example: AllLaunchConsoleUI.exe -n -t -a" + '\n' +
                "-n: no menu (saved list of apps required)" + '\n' +
                "\nPress a key to return..."
            );
        }
    }
}