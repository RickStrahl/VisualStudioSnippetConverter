using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Westwind.SnippetConverter
{
    
    public class Utils
    {

        /// <summary>
        /// Uses the Shell Extensions to launch a program based on URL moniker or file name
        /// Basically a wrapper around ShellExecute
        /// </summary>
        /// <param name="url">Any URL Moniker that the Windows Shell understands (URL, Word Docs, PDF, Email links etc.)</param>
        /// <returns></returns>
        public static int GoUrl(string url)
        {
            string TPath = Path.GetTempPath();

            ProcessStartInfo info = new ProcessStartInfo();
            info.UseShellExecute = true;
            info.Verb = "Open";
            info.WorkingDirectory = TPath;
            info.FileName = url;

            Process process = new Process();
            process.StartInfo = info;
            process.Start();

            return 0;

        }

        
        /// <summary>
        /// Opens a File or Folder in Explorer. If the path is a file
        /// Explorer is opened in the parent folder with the file selected
        /// </summary>
        /// <param name="filename"></param>
        public static void OpenFileInExplorer(string filename)
        {
            if (Directory.Exists(filename))
                GoUrl(filename);
            else
                Process.Start("explorer.exe", $"/select,\"{filename}\"");
        }
    }
}