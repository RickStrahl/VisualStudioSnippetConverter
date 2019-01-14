# Command Lines to Execute


### Show Help

```
dotnet snippetconverter.dll
```

or 

```
dotnet run
```

### Convert CSharp Folder to VS Code
```
dotnet run "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets" -o "%APPDATA%\Code\User\snippets\ww2-csharp.code-snippets" -d
```

### Convert CSharp Folder to Rider

```
dotnet run "%USERPROFILE%\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets" -m vs-rider -d
```