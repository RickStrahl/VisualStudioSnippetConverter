using System;
using System.Reflection;
using Westwind.SnippetConverter;
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

                if (ver.EndsWith(".0.0"))
                    ver = ver.Substring(0, ver.Length - 4);
                if (ver.EndsWith(".0"))
                    ver = ver.Substring(0, ver.Length - 2);

                var header = $"Visual Studio Snippet Converter v{ver}";
                var line = Utils.Replicate("-", header.Length);

                string options =
                    $@"
{header}
{line}
(c) Rick Strahl, West Wind Technologies

Syntax:
-------
SnippetConverter <sourceFileOrDirectory> -o <outputFile> 
                 --mode --prefix --recurse --display

Commands:
---------
HELP || /?          This help display           

Options:
--------
sourceFileOrDirectory  Either an individual snippet file, or a source folder
                       Optional special start syntax using `~` to point at User Code Snippets folder:
                       ~      -  Visual Studio User Code Snippets folder (latest version installed)
                       ~2017  -  Visual Studio User Code Snippets folder (specific VS version 2019-2012)                       

-o <outputFile>        Output file where VS Code snippets are generated into (ignored by Rider)   
                       Optional special start syntax using `~` to point at User Code Snippets folder:
                       %APPDATA%\Code\User\snippets\ww-my-codesnippets.code-snippets
                       ~\ww-my-codesnippets.code-snippets
                       if omitted generates `~\exported-visualstudio.code-snippets`
                       
-m,--mode              vs-vscode  (default)
                       vs-rider   experimental - (C#,VB.NET,html only)
-d                     display the target file in Explorer
-r                     if specifying a source folder recurses into child folders
-p,--prefix            snippet prefix generate for all snippets exported
                       Example: `ww-` on a snippet called `ifempty` produces `ww-ifempty`

Examples:
---------
# vs-vscode: Individual Visual Studio Snippet
SnippetConverter ""~2017\Visual C#\My Code Snippets\proIPC.snippet"" 
                 -o ""~\ww-csharp.code-snippets"" -d

# vs-vscode: All snippets in a folder user VS Snippets and in recursive child folers
SnippetConverter ""~2017\Visual C#\My Code Snippets"" -o ""~\ww-csharp.code-snippets"" -r -d

# vs-vscode: All the user VS Snippets and in recursive child folders
SnippetConverter ~2017\ -o ""~\Code\User\snippets\ww-all.code-snippets"" -r -d

# vs-vscode: All defaults: Latest version of VS, all snippets export to  ~\visualstudio-export.code-snippets
SnippetConverter ~ -r -d --prefix ww-

# vs-rider: Individual VS Snippet
SnippetConverter ""~2017\proIPC.snippet"" -m vs-rider -d

# vs-rider: All VS Snippets in a folder
SnippetConverter ""~2017\Visual C#\My Code Snippets"" -m vs-rider -d
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
