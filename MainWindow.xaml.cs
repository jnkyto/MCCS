using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MCCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void decomp_Click(object sender, RoutedEventArgs e)
        {
            string sourceDir = pathInput.Text;
            var result = MessageBox.Show(
                "This will create a copy of your logs folder, and decompress every log file. This may take a few seconds. Proceed?",  // the message to show
                "Are you sure?",           // the title for the dialog box
                MessageBoxButton.OKCancel, // show two buttons: OK and Cancel
                MessageBoxImage.Question); // show a question mark icon

            if(result == MessageBoxResult.OK)
            {
                LogDecompInit("windows", sourceDir);   // redundant os check, might implement later
            }
        }

        private void pathsel_Click(object sender, RoutedEventArgs e)
        {
        }

        public static void LogDecompInit(string os, string sourceDir)
        {
            if (os.Equals("windows") && sourceDir.Contains("minecraft") && sourceDir.Contains("logs"))
            {
                // String mcDir = @"C:\Users\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1] + @"\AppData\Roaming\.minecraft\";
                string targetDir = sourceDir + @"\..\MCCS_DecompLogs";
                copyFolder(sourceDir, targetDir, true);
                DirectoryInfo selDir = new DirectoryInfo(targetDir);
                foreach (FileInfo fileToDeCmprs in selDir.GetFiles("*.gz"))
                {
                    Decomp(fileToDeCmprs);
                }
                MessageBox.Show("Decompression successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                        "Invalid path! Please check your input.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
        }
        public static void Decomp(FileInfo fileToDeCmprs)
        {
            string currentFile = fileToDeCmprs.FullName;
            using (FileStream originalFileStream = fileToDeCmprs.OpenRead())
            {
                string newFile = currentFile.Remove(currentFile.Length - fileToDeCmprs.Extension.Length);
                using (FileStream decompFileStream = File.Create(newFile))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompFileStream);
                    }
                }
            }
            File.Delete(currentFile);   // ensures .gz files are deleted in target folder after decompression
        }

        public static void copyFolder(string source, string target, bool copySubDirs)   // this is pretty much skidded from microsoft's site
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            
            if(!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source does not exist! Dir: " + source);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(target);
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo file in files)
            {
                string tempPath = System.IO.Path.Combine(target, file.Name);
                file.CopyTo(tempPath, true);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = System.IO.Path.Combine(target, subdir.Name);
                    copyFolder(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static void LogHandler(string target)
        {
            DirectoryInfo dir = new(target);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Target does not exist! Dir: " + target);
            }

            FileInfo[] files = dir.GetFiles();

            foreach(FileInfo file in files)
            {
                if(!file.Name.Contains("debug"))
                {

                }
            }
        }
    }
}
