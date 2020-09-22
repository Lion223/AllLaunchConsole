using System;
using System.Windows.Forms;
using AllLaunchLibrary;
using AllLaunchLibrary.EventArgsModels;

namespace AllLaunchConsoleUI
{
    internal sealed class AllLaunch
    {
        private static bool showMenu = true;
        
        [STAThread]
        static void Main(string[] args)
        {   
            AttachEvents();

            // Could have an alternative on argument appliance
            ArgumentProcessor.Process(args);
            
            while (showMenu)
            {
                Console.Clear();
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
        }

        private static bool Menu()
        {
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
                    Console.Clear();
                    AddApp();
                    return true;
                case '2':
                    Console.Clear();
                    RemoveApp();
                    return true;
                case '3':
                    Console.Clear();
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
                    Console.Clear();
                    MessageTemplate.Arguments();
                    Console.ReadKey();
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
            string fileName = OpenFileDialogHelper.DialogFileName();

            if (fileName != null)
            {
                string appName = OpenFileDialogHelper.ApplicationNameParser(fileName);

                if (!Validator.AppExists(appName, fileName))
                {
                    Console.WriteLine($"Application's name: {appName}");
                    Console.Write("Enter launch arguments: ");
                    string args = Console.ReadLine();

                    LaunchManager.Instance.AddApp(appName, fileName, args);
                }
                else
                {
                    MessageTemplate.AppAlreadyExists();
                    Console.ReadKey();
                }
            }
        }

        private static void RemoveApp()
        {
            string appList = LaunchManager.Instance.GetLaunchList();

            Console.WriteLine(appList);

            Console.Write("Enter application's index: ");
            int index = int.TryParse(Console.ReadLine(), out index) ? index : -1;

            if (index >= 0 && index < LaunchData.AppModels.Count)
            {
                LaunchData.AppModels.RemoveAt(index);
            }
            else
            {
                MessageTemplate.WrongInput();
                Console.ReadKey();
            }
        }

        private static void UpdateApp()
        {
            Console.Write("Enter application's index: ");
            int index = int.TryParse(Console.ReadLine(), out index) ? index : -1;

            if (index >= 0 && index < LaunchData.AppModels.Count)
            {
                string initialDirectory = LaunchData.AppModels[index].PathToExe
                    .Substring(0, LaunchData.AppModels[index].PathToExe.LastIndexOf(@"\"));

                string fileName = OpenFileDialogHelper.DialogFileName(initialDirectory);

                if (fileName != null)
                {
                    string appName = OpenFileDialogHelper.ApplicationNameParser(fileName);

                    if (!Validator.AppExists(appName, fileName))
                    {
                        LaunchData.AppModels[index].Name = appName;
                        LaunchData.AppModels[index].PathToExe = fileName;

                        Console.WriteLine($"Application's name: {appName}");
                        Console.Write("Enter launch arguments: ");
                        SendKeys.SendWait(LaunchData.AppModels[index].Args);
                        LaunchData.AppModels[index].Args = Console.ReadLine();
                    }
                    else
                    {
                        MessageTemplate.AppAlreadyExists();
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                MessageTemplate.WrongInput();
                Console.ReadKey();
            }
        }

        private static void AppLaunched(object sender, AppEventArgs e)
        {
            Console.WriteLine($"{e.Name} {e.Args}: is launched");
        }

        private static void AppAlreadyRunning(object sender, AppEventArgs e)
        {
            Console.WriteLine($"{e.Name} {e.Args}: is already running");
        }

        private static void AppFileIsNotFound(object sender, AppEventArgs e)
        {            
            DialogResult res = MessageBox.Show
            (
                $"{e.Name}: .exe file is not found. Wish to update the path? Otherwise, it will be deleted from the list", 
                "Update an application info", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Information
            );  

            var item = LaunchData.AppModels.Find(x => x.Name == e.Name);

            if (res == DialogResult.Yes) { 

                string fileName = OpenFileDialogHelper.DialogFileName(e.PathToExe
                        .Substring(0, e.PathToExe.LastIndexOf(@"\")));

                if (fileName != null)
                {
                    string appName = OpenFileDialogHelper.ApplicationNameParser(fileName);

                    if (!Validator.AppExists(appName, fileName))
                    {
                        item.Name = appName;
                        item.PathToExe = fileName;
                    }
                    else
                    {
                        LaunchData.AppModels.Remove(item);
                    }
                }
            }
            else if (res == DialogResult.No)
            {
                LaunchData.AppModels.Remove(item);
            }
        }
    }
}
