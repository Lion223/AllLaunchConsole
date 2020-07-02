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
        private readonly static LaunchManager lm = new LaunchManager();
        private readonly static System.Timers.Timer t = new System.Timers.Timer(5000);
        static void Main(string[] args)
        {
            lm.AppLaunched += AppLaunched;
            lm.AppAlreadyRunning += AppAlreadyRunning;

            t.Elapsed += Timer_Elapsed;

            if (args.Length != 0) 
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "-n")
                    {
                        lm.LoadObj();
                        lm.Start();
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
            string appList = lm.GetLaunchList();

            Console.Write
            (
                "AllLaunch..." + "\n" +
                appList + "-----\n" +
                "1. Add an application" + '\n' +
                "2. Remove an application" + '\n' +
                "3. Update an application" + '\n' +
                "4. Launch applications" + '\n' +
                "5. Save a list of applications" + '\n' +
                "6. Load a list of applications" + '\n' +
                "7. Arguments" + '\n' +
                "Esc. Quit" + '\n' +
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
                    lm.Start();
                    //t.Start();
                    return false;
                case '5':
                    lm.SaveObj();
                    return true;
                case '6':
                    lm.LoadObj();
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

        private void ArgsSetup(string[] args)
        {
            
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

            lm.AddApp(name, pathToExe, args);
        }

        private static void RemoveApp()
        {
            Console.Clear();
            Console.WriteLine("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());

            lm.AppModels.RemoveAt(index);
        }

        private static void UpdateApp()
        {
            Console.Clear();
            Console.WriteLine("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());
            
            Console.WriteLine("Enter application name: ");
            SendKeys.SendWait(lm.AppModels[index].Name);
            lm.AppModels[index].Name = Console.ReadLine();

            Console.WriteLine("\nEnter path to .exe file: ");
            SendKeys.SendWait(lm.AppModels[index].PathToExe);
            lm.AppModels[index].PathToExe = Console.ReadLine();

            Console.WriteLine("\nEnter launch arguments: ");
            SendKeys.SendWait(lm.AppModels[index].Args);
            lm.AppModels[index].Args = Console.ReadLine();
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
            //t.Start();
        }

        private static void AppAlreadyRunning(object sender, AppEventArgs e)
        {
            Console.WriteLine($"{e.Name} {e.Args}: is already running");
            //t.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
