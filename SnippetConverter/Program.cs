using System;
using System.Reflection;
using Westwind.SnippetConverter.ConsoleApp;
using Westwind.Utilities;

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

                var header = $"Visual Studio Snippet Converter v{version}";
                var line = StringUtils.Replicate("-", header.Length);
                
                string options =
                    $@"
{header}
{line}
(c) Rick Strahl, West Wind Technologies

Syntax:
-------
SnippetConverter <sourceFileOrDirectory> -o <outputFile> -d

Commands:
---------
HELP || /?          This help display           

Options:
--------
sourceFileOrDirectory  Either an individual snippet file, or a source folder
                       Optional special start syntax using `~\` to point at User Code Snippets folder:
                       ~\       -  Visual Studio User Code Snippets folder (latest version installed)
                       ~\2017\  -  Visual Studio User Code Snippets folder (specific VS version)
                       ~\2019\  -  Visual Studio User Code Snippets folder (specific VS version)


-m,--mode              vs-vscode  (default)
                       vs-rider
-d                     display the target file in Explorer
-o <outputFile>        Output file where snippets are generated into
                       (Rider uses a default configuration file)
-r                     if specifying a source folder recurses into child folders
-v                     verbose echoes additional information to the console
-p,--prefix            snippet prefix generate for all snippets exported
                       Example: `ww-` on a snippet called `ifempty` produces `ww-ifempty`

Examples:
---------
# vs-vscode: Individual VS Snippet
SnippetConverter ""~\2017\Visual C#\My Code Snippets\proIPC.snippet"" 
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d

# vs-vscode: All VS Snippets in a folder
SnippetConverter ""~\2017\Visual C#\My Code Snippets"" 
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d

# vs-vscode: All the user VS Snippets and in recursive child folers
SnippetConverter ~\2017\ -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d -r

# vs-vscode: All the user VS Snippets and in recursive child folers
SnippetConverter ~\2017\ -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d -r



# vs-rider: Individual VS Snippet
SnippetConverter ""~\2017\proIPC.snippet"" -m vs-rider 

# vs-rider: All VS Snippets in a folder
SnippetConverter ""~\2017\Visual C#\My Code Snippets"" -m vs-rider
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
