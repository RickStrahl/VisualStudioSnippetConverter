# Visual Studio Snippet Converter

<img src="SnippetConverterIcon.png" height=200>

This is a small hacky project to convert Visual Studio Code Snippets (XML .snippet files) to:

* VS Code Code Snippets
* Rider Live Templates

It consists of a library that can take individual Visual Studio snippets or a folder full of `.snippet` files, and create `.code-snippet` file with the snippets in VS Code, or create the appropriate live templates for JetBrains Rider and add it to the default configuration file. 

In VS Code the snippets are updated if they exist, in Rider Snippets can only be added to the global configuration (ie. you end up duplicating if you run multiple times).

This tooling provides both the library and a .NET Core Console application and .NET Core Tooling package.

### Using the .NET Core Console Application
The easiest way to use this tool is to install the dotnet tool from NuGet:

> **The package is not available yet!**

```ps
install-package dotnet-snippetconverter
```

**Requires:**  
[.NET Core 2.1 SDK](https://dotnet.microsoft.com/downloadhttps://dotnet.microsoft.com/download) or later

Once installed you should able to run:

```ps
snippetconverter
```

from the Console.

Alternately, if you just use the source code to compile the project, you can use


```
dotnet run
```

from the project folder.

Or in the publish folder:

```
snippetconverter
```

### Command Line Options

```
Visual Studio Snippet Converter
-------------------------------
© Rick Strahl, West Wind Technologies

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
-o <outputFile>        VS Code: Output file where one or more snippets are generated into
                       Rider: not used - output goes into the Rider Configuration file

Examples:
---------
# vs-vscode: Individual VS Snippet
SnippetConverter "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet" 
                 -o "%APPDATA%\Code\User\snippets\ww-csharp.code-snippets" -d

# vs-vscode: All VS Snippets in a folder
SnippetConverter "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets"
                 -o ""%APPDATA%\Code\User\snippets\ww-csharp.code-snippets"" -d

# vs-rider: Individual VS Snippet
SnippetConverter "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet"

SnippetConverter "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets"
```



### Use at your own Risk
This is a very rough project I created to just get the job of converting my CSharp, HTML and JavaScript templates between environments. I make no guarantees that it'll move all types of snippets nor that it will capture all features of the snippets. However, it works very well for my use case which has been able to easily migrate all of my Visual Studio snippets to both VS Code and Rider.
 
Your mileage may vary!

## License
The SnippetConverter library is licensed  under the [MIT License](https://opensource.org/licenses/MIT) and there's no charge to use, integrate or modify the code for this project. You are free to use it in personal, commercial, government and any other type of application.

All source code is copyright **West Wind Technologies**, regardless of changes made to them. Any source code modifications must leave the original copyright code headers intact.

<!-- 
> It's free as in free beer, but if this saved you some time and you're overflowing with gratitude you can buy me a beer:
>
> [Make a Donation with PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=DJJHMXWYPT3E2)
-->

## Warranty Disclaimer: No Warranty!
IN NO EVENT SHALL THE AUTHOR, OR ANY OTHER PARTY WHO MAY MODIFY AND/OR REDISTRIBUTE THIS PROGRAM AND DOCUMENTATION, BE LIABLE FOR ANY COMMERCIAL, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM INCLUDING, BUT NOT LIMITED TO, LOSS OF DATA OR DATA BEING RENDERED INACCURATE OR LOSSES SUSTAINED BY YOU OR LOSSES SUSTAINED BY THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS, EVEN IF YOU OR OTHER PARTIES HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.