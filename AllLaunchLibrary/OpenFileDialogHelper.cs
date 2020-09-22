using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace AllLaunchConsoleUI
{
    // Helper methods for OpenFileDialog calls
    public static class OpenFileDialogHelper
    {
        // Parses an app's name from the received path
        public static string ApplicationNameParser(string fileName)
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                    
            string appName = String.IsNullOrEmpty(fileVersionInfo.ProductName) ? 
                String.Concat(fileVersionInfo.FileName
                .Substring(fileVersionInfo.FileName.LastIndexOf(@"\") + 1)
                .SkipLast(4)) : fileVersionInfo.ProductName;

            return appName.First().ToString().ToUpper() + appName.Substring(1);
        }

        // Calls a browse dialog window and returns a path to the chosen .exe
        public static string DialogFileName(string initialDirectory = null)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (initialDirectory != null)
                {
                    openFileDialog.InitialDirectory = initialDirectory;
                }

                openFileDialog.Filter = "Exe (*.exe)|*.exe|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {          
                    return openFileDialog.FileName;
                }

                return null;
            }
        }
    }
}