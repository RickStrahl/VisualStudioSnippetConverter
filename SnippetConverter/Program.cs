using System;
using System.Reflection;
using Westwind.SnippetConverter.ConsoleApp;

namespace SnippetConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args[0] == "HELP" || args[0] == "/?" || args[0] == "-?")
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var ver = version.Major + "." + version.Minor +
                          (version.Build > 0 ? "." + version.Build : string.Empty);
                
                string options =
                    $@"
Visual Studio Snippet Converter v{version}
- Rick Strahl, West Wind Technologies

Syntax:
-------
SnippetConverter <sourceFileOrDirectory> -o <outputFile> -d

Commands:
---------
HELP || /?          This help display           

Options:
--------
sourceFileOrDirectory  Either an individual snippet file, or a source folder
-m,--mode              vs-vscode  (default)
                       vs-rider
-d                     display the target file in Explorer
-o <outputFile>        Output file where snippets are generated into
                       (Rider uses a default configuration file)

Examples:
---------
# vs-vscode: Individual VS Snippet
SnippetConverter ""%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet"" 
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d

# vs-vscode: All VS Snippets in a folder
SnippetConverter ""%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets"" 
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d
";

                Console.WriteLine(options);
            }
            else
            {
                var cmdLine = new SnippetConverterCommandLineParser();
                try
                {
                    cmdLine.Parse();
                }
                catch (Exception ex)
                {
                    var oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"*** Error processing snippet files:\n{ex.Message}");
                    Console.ForegroundColor = oldColor;
                }

                var del = new SnippetConverterProcessor(cmdLine);
                del.Process();
            }

#if DEBUG

            Console.WriteLine("Done. Press any key to exit...");
            Console.ReadKey();
#endif
        }
    }
}
