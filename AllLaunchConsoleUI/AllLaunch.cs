using System;
using System.Text;

// <Project Sdk="Microsoft.NET.Sdk">
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using AllLaunchLibrary;
using AllLaunchLibrary.EventArgsModels;

namespace AllLaunchConsoleUI
{
    internal sealed class AllLaunch
    {
        // Wait for 5 sec to close after all apps have launched
        private static System.Timers.Timer timer 
            = new System.Timers.Timer(5000);
        private static bool showMenu = true;

        [STAThread]
        static void Main(string[] args)
        {   
            AttachEvents();

            // Could have an alternative on argument appliance
            ArgumentProcessor.Process(args);
            
            while (showMenu)
            {
                showMenu = Menu(); 
            }
            
            // Readkey changes behavior of the app - the key needs to be pushed to launch other apps and therefore the app closes
            // ReadLine works as intended - firstly, apps are launching and then Enter is pressed on the app which leads to its end
            //Console.ReadLine();
            
            Console.ReadKey();
        }

        private static void AttachEvents()
        {
            LaunchManager.Instance.AppLaunched += AppLaunched;
            LaunchManager.Instance.AppAlreadyRunning += AppAlreadyRunning;
            LaunchManager.Instance.AppFileIsNotFound += AppFileIsNotFound;

            timer.Elapsed += Timer_Elapsed;
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
                    LaunchData.SaveObj();
                    return true;
                case '6':
                    LaunchData.LoadObj();
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
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Exe (*.exe)|*.exe|All files (*.*)|*.*";
                //openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                    
                    string appName = String.IsNullOrEmpty(fileVersionInfo.ProductName) ? 
                        String.Concat(fileVersionInfo.FileName.Substring(fileVersionInfo.FileName.LastIndexOf(@"\") + 1).SkipLast(4)) : 
                        fileVersionInfo.ProductName;
                    appName = appName.First().ToString().ToUpper() + appName.Substring(1);
                    Console.Clear();
                    Console.WriteLine($"Application's name: {appName}");
                    Console.Write("Enter launch arguments: ");
                    string args = Console.ReadLine();

                    LaunchManager.Instance.AddApp(appName, openFileDialog.FileName, args);
                }
            }
        }

        private static void RemoveApp()
        {
            Console.Clear();
            Console.Write("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());

            LaunchData.AppModels.RemoveAt(index);
        }

        private static void UpdateApp()
        {
            Console.Clear();
            Console.Write("Enter application's index: ");
            int index = int.Parse(Console.ReadLine());

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = LaunchData.AppModels[index].PathToExe
                    .Substring(0, LaunchData.AppModels[index].PathToExe.LastIndexOf(@"\"));
                openFileDialog.Filter = "Exe (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                    LaunchData.AppModels[index].Name = fileVersionInfo.ProductName;
                    LaunchData.AppModels[index].PathToExe = openFileDialog.FileName;

                    Console.Write("Enter launch arguments: ");
                    SendKeys.SendWait(LaunchData.AppModels[index].Args);
                    LaunchData.AppModels[index].Args = Console.ReadLine();
                }
            }
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

        private static void AppFileIsNotFound(object sender, AppEventArgs e)
        {
            timer.Stop();
            
            DialogResult res = MessageBox.Show
            (
                $"{e.Name}: .exe file is not found. Wish to update the path? Otherwise, it will be deleted from the list", 
                "Update an application info", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Information
            );  

            var item = LaunchData.AppModels.Find(x => x.Name == e.Name);

            if (res == DialogResult.Yes) {  
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = e.PathToExe
                        .Substring(0, e.PathToExe.LastIndexOf(@"\"));
                    openFileDialog.Filter = "Exe (*.exe)|*.exe|All files (*.*)|*.*";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(openFileDialog.FileName);
                        item.PathToExe = openFileDialog.FileName;
                    }
                }
            }
            else if (res == DialogResult.No)
            {
                LaunchData.AppModels.Remove(item);
            }

            timer.Start();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
