#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          © West Wind Technologies, 
 *          http://www.west-wind.com/
 * 
 * Created: 8/29/2018
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Westwind.SnippetConverter.ConsoleApp
{
    public class SnippetConverterProcessor
    {
        SnippetConverterCommandLineParser Parser { get; set; }


        public SnippetConverterProcessor(SnippetConverterCommandLineParser parser = null)
        {
            Parser = parser;
        }

        public bool Process()
        {
            var consoleColor = Console.ForegroundColor;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var ver = version.Major + "." + version.Minor + (version.Build > 0 ? "." + version.Build : string.Empty);

            WriteConsole($"Visual Studio Snippet Converter v{ver}", MessageModes.Information);
            WriteConsole($"- Rick Strahl, West Wind Technologies", MessageModes.Information);


            Console.WriteLine($"Processing {Parser.SourceFileOrDirectory}...");

            if (Parser.Mode == "vs-vscode")
            {
                if (string.IsNullOrEmpty(Parser.TargetFile))
                {
                    WriteConsole("Error: Please specify an output VS Code snippet file with the `-o` option.",
                        MessageModes.Error);
                    return false;
                }

                try
                {
                    List<VisualStudioSnippet> snippets;
                    if (Parser.DirectoryMode)
                    {
                        snippets = VisualStudioSnippet.ParseSnippetFolder(Parser.SourceFileOrDirectory);
                        if (snippets == null || snippets.Count < 1)
                        {
                            WriteConsole("Error: No snippets found in path: " + Parser.SourceFileOrDirectory,
                                MessageModes.Error);
                            return false;
                        }

                    }
                    else
                    {
                        var snippet = VisualStudioSnippet.ParseFromFile(Parser.SourceFileOrDirectory);
                        if (snippet == null)
                        {
                            WriteConsole("Error: No snippets found in path: " + Parser.SourceFileOrDirectory, MessageModes.Error);
                            return false;
                        }
                        snippets = new List<VisualStudioSnippet>() { snippet };
                    }

                    VsCodeSnippet.ToFile(snippets, Parser.TargetFile, false, Parser.SnippetPrefix);
                }
                catch (Exception ex)
                {
                    WriteConsole("Error: Snippet conversion failed for: " + Parser.SourceFileOrDirectory
                                + "\n" + ex.Message, MessageModes.Error);
                    return false;
                }

                if (Parser.ShowFileInExplorer)
                    Utils.OpenFileInExplorer(Parser.TargetFile);
            }
            else if (Parser.Mode == "vs-rider" && Parser.DirectoryMode)
            {

                var snippets = VisualStudioSnippet.ParseSnippetFolder(Parser.SourceFileOrDirectory);
                if (snippets == null || snippets.Count < 1)
                {
                    WriteConsole("Error: No snippets found in path: " + Parser.SourceFileOrDirectory, MessageModes.Error);
                    return false;
                }

                try
                {
                    JetBrainsLiveTemplate.AddVisualStudioSnippets(Parser.SourceFileOrDirectory, Parser.SnippetPrefix);
                }
                catch (Exception ex)
                {
                    WriteConsole("Error: Snippet conversion failed for: " + Parser.SourceFileOrDirectory + "\n" + ex.Message, MessageModes.Error);
                    return false;
                }


            }

            if (Parser.ShowFileInExplorer)
                Utils.OpenFileInExplorer(Parser.TargetFile);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Done. Created {Parser.TargetFile}.");
            Console.ForegroundColor = consoleColor;

            return true;
        }

        public Action<string> WriteMessage { get; set; }

        private static void WriteConsole(string msg, MessageModes mode)
        {
            var consoleColor = Console.ForegroundColor;

            if (mode == MessageModes.Error)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (mode == MessageModes.Warning)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (mode == MessageModes.Information)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (mode == MessageModes.Success)
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(msg);

            Console.ForegroundColor = consoleColor;
        }

        /// <summary>
        /// Event that allows you to override the output that is sent 
        /// by this class. If not set output is sent to the Console.
        /// </summary>
        public event Action<string> ShowMessage;

        public virtual void OnShowMessage(string message)
        {

            if (ShowMessage != null)
                ShowMessage(message);
            else
                Console.WriteLine(message);
        }


    }


    public enum MessageModes
    {
        Information,
        Success,
        Error,
        Warning,
        None
    }

}
