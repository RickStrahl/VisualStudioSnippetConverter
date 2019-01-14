using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Westwind.SnippetConverter
{
    
    public static class Utils
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

        /// <summary>
        /// Replicates an input string n number of times
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public static string Replicate(string input, int charCount)
        {
            return new StringBuilder().Insert(0, input, charCount).ToString();
        }

        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        public static string[] GetLines(this string s, int maxLines = 0)
        {
            if (s == null)
                return null;

            s = s.Replace("\r\n", "\n");

            if (maxLines < 1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }

        ///  <summary>
        ///  Turns a string into a properly XML Encoded string.
        ///  Uses simple string replacement.
        /// 
        ///  Also see XmlUtils.XmlString() which uses XElement
        ///  to handle additional extended characters.
        ///  </summary>
        ///  <param name="text">Plain text to convert to XML Encoded string</param>
        /// <param name="isAttribute">
        /// If true encodes single and double quotes, CRLF and tabs.
        /// When embedding element values quotes don't need to be encoded.
        /// When embedding attributes quotes need to be encoded.
        /// </param>
        /// <returns>XML encoded string</returns>
        ///  <exception cref="InvalidOperationException">Invalid character in XML string</exception>
        public static string XmlString(string text, bool isAttribute = false)
        {
            var sb = new StringBuilder(text.Length);

            foreach (var chr in text)
            {
                if (chr == '<')
                    sb.Append("&lt;");
                else if (chr == '>')
                    sb.Append("&gt;");
                else if (chr == '&')
                    sb.Append("&amp;");

                // special handling for quotes
                else if (isAttribute && chr == '\"')
                    sb.Append("&quot;");
                else if (isAttribute && chr == '\'')
                    sb.Append("&apos;");

                // Legal sub-chr32 characters
                else if (chr == '\n')
                    sb.Append(isAttribute ? "&#xA;" : "\n");
                else if (chr == '\r')
                    sb.Append(isAttribute ? "&#xD;" : "\r");
                else if (chr == '\t')
                    sb.Append(isAttribute ? "&#x9;" : "\t");

                else
                {
                    if (chr < 32)
                        throw new InvalidOperationException("Invalid character in Xml String. Chr " +
                                                            Convert.ToInt16(chr) + " is illegal.");
                    sb.Append(chr);
                }
            }

            return sb.ToString();
        }
    }
}