using System;
using System.Text;

// <Project Sdk="Microsoft.NET.Sdk">
using System.Windows.Forms;
using System.Timers;
using AllLaunchLibrary;

namespace AllLaunchConsoleUI
{
    internal sealed class AllLaunch
    {
        // Wait for 5 sec to close after all apps have launched
        private static System.Timers.Timer timer 
            = new System.Timers.Timer(5000);

        static void Main(string[] args)
        {
            LaunchManager.Instance.AppLaunched += AppLaunched;
            LaunchManager.Instance.AppAlreadyRunning += AppAlreadyRunning;

            timer.Elapsed += Timer_Elapsed;


            // Could have an alternative on argument appliance
            if (args.Length != 0) 
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-n")
                    {
                        LaunchManager.Instance.LoadObj();
                        LaunchManager.Instance.Start();
                        Environment.Exit(0);
                        
                    }
                }
            }
            
            bool showMenu = true;
            while (showMenu)
            {
                showMenu = Menu(); 
            }
            
            // Readkey changes behavior of the app - the key needs to be pushed to launch other apps and therefore the app closes
            // ReadLine works as intended - firstly, apps are launching and then Enter is pressed on the app which leads to its end
            Console.ReadLine();
        }

        private static bool Menu()
        {
            Console.Clear();
            string appList = LaunchManager.Instance.GetLaunchList();

            Console.Write
            (
                "AllLaunch..." + "\n" +
                appList + "-----\n" +
                "1. Add;" + " " +
                "2. Remove;" + " " +
                "3. Update;" + " " +
                "4. Launch;" + " " +
                "5. Save;" + " " +
                "6. Load;" + " " +
                "7. Arguments;" + " " +
                "Esc. Quit." + "\n" +
                "Enter: "
            );

            switch(Console.ReadKey().KeyChar)
            {
                case '1':
                    AddApp();
                    return true;
                case '2':
                    RemoveApp();
                    return true;
                case '3':
                    UpdateApp();
                    return true;
                case '4':
                    Console.Clear();
                    if (LaunchManager.Instance.Start())
                    {
                        return false;
                    }
                    return true;
                case '5':
                    LaunchManager.Instance.SaveObj();
                    return true;
                case '6':
                    LaunchManager.Instance.LoadObj();
                    return true;
                case '7':
                    ListArguments();
                    return true;
                case (char) Keys.Escape:
                    Environment.Exit(0);
                    return false;
                default:
                    return true;
            }
        }

        private static void AddApp()
        {
            Console.Clear();
            Console.WriteLine("Enter application name: ");
            string name = Console.ReadLine();
            
            Console.WriteLine("\nEnter path to .exe file: ");
            string pathToExe = Console.ReadLine();

            Console.WriteLine("\nEnter launch arguments: ");
            string args = Console.ReadLine();

            LaunchManager.Instance.AddApp(name, pathToExe, args);
        }

        private static void RemoveApp()
        {
            Console.Clear();
            Console.WriteLine("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());

            LaunchManager.Instance.AppModels.RemoveAt(index);
        }

        private static void UpdateApp()
        {
            Console.Clear();
            Console.WriteLine("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter application name: ");
            SendKeys.SendWait(LaunchManager.Instance.AppModels[index].Name);
            LaunchManager.Instance.AppModels[index].Name = Console.ReadLine();

            Console.WriteLine("\nEnter path to .exe file: ");
            SendKeys.SendWait(LaunchManager.Instance.AppModels[index].PathToExe);
            LaunchManager.Instance.AppModels[index].PathToExe = Console.ReadLine();

            Console.WriteLine("\nEnter launch arguments: ");
            SendKeys.SendWait(LaunchManager.Instance.AppModels[index].Args);
            LaunchManager.Instance.AppModels[index].Args = Console.ReadLine();
        }

        private static void ListArguments()
        {
            Console.Clear();
            Console.Write
            (
                "Example: AllLaunchConsoleUI.exe -n -t -a" + '\n' +
                "-n: no menu (saved list of apps required)" + '\n' +
                "\nPress a key to return..."
            );
            Console.ReadKey();
        }

        private static void AppLaunched(object sender, AppEventArgs e)
        {
            Console.WriteLine($"{e.Name} {e.Args}: is launched");
            
            timer.Stop();
            timer.Start();
        }

        private static void AppAlreadyRunning(object sender, AppEventArgs e)
        {
            Console.WriteLine($"{e.Name} {e.Args}: is already running");
            
            timer.Stop();
            timer.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
