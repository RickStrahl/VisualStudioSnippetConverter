# Visual Studio Snippet Converter

This is a small hacky project to convert Visual Studio Code Snippets to:

* Visual Studio Code Code Snippets
* Rider Live Templates


It consists of a library that can take individual Visual Studio snippets or a folder full of `.snippet` files, and create `.code-snippet` file with the snippets. In VS Code the snippets are updated, in Rider Snippets can only be added to the global configuration.

A .NET Core Console application via .NET tooling lets you run the console application to 



> ### Use at your own Risk
> This is a very rough project I created to just get the job of converting my CSharp and HTML templates between environments. I make no guarantees that it'll move all types of snippets nor that it will capture all features of the snippets. However, it works very well for my use case which has been able to easily migrate all of my Visual Studio snippets to both VS Code and Rider.
> 
> Your mileage may vary!

### Using the .NET Core Console Application
The easiest way to use this tool is to install the dotnet tool from NuGet:

```
install-package dotnet-snippetconverter
```

**Requires:**  
[.NET Core 2.1 SDK](https://dotnet.microsoft.com/downloadhttps://dotnet.microsoft.com/download) or later

&copy;

Once installed you should able to run:

```ps
snippetconverter
```

from the Console.

```
Visual Studio Snippet Converter
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
-m,--mode              vs-vscode  (default)
                       vs-rider
-o <outputFile>        Output file where snippets are generated into
                       (Ignored for Rider as it uses a global configuration file)

Examples (each on a single line):
---------
# vs-vscode: Individual VS Snippet
SnippetConverter ""%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet"" 
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d
                 

# vs-vscode: All VS Snippets in a folder
SnippetConverter ""%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets"" 
                 ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets""
```


