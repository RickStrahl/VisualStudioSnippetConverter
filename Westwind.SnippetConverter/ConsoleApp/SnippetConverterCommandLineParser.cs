using System;
using System.IO;

namespace Westwind.SnippetConverter.ConsoleApp
{
    public class SnippetConverterCommandLineParser : CommandLineParser
    {

        /// <summary>
        /// The source snippet file or snippet folder. Specify with
        /// the -o switch.
        ///
        /// If specifying a folder set the `-d` flag to indicate
        /// directory mode. 
        /// </summary>
        public string SourceFileOrDirectory { get; set; }

        /// <summary>
        /// The target that the snippet is generated to.
        ///
        /// For Rider this target is ignored.
        /// </summary>
        public string TargetFile { get; set; }
        
        /// <summary>
        /// Determines whether SourceFileOrDirectory is
        /// a directory or a file. 
        /// </summary>
        public bool DirectoryMode { get; set; }

        /// <summary>
        /// Available modes:
        /// 
        /// vs-vscode  (default)
        /// vs-rider
        /// </summary>
        public string Mode { get; set; } = "vs-vsode";
                
        /// <summary>
        /// Determines whether the source snippet folder
        /// recurses through child directories
        /// </summary>
        public bool Recurse { get; set; }

        /// <summary>
        /// Determines whether the parser is using verbose mode to display messages
        /// </summary>
        public bool Verbose { get; set; }
        
        
        /// <summary>
        /// Shows the file in explorer -s
        /// </summary>
        public bool ShowFileInExplorer { get; set; }
        
        
        /// <summary>
        /// SnippetPrefix for generated snippets
        /// </summary>
        public string SnippetPrefix { get; set;  }        

        public SnippetConverterCommandLineParser(string[] args = null, string cmdLine = null)
            : base(args, cmdLine)
        {
        }

        public override void Parse()
        {
            Mode = ParseStringParameterSwitch("-m", null);
            if (string.IsNullOrEmpty(Mode))
                Mode = ParseStringParameterSwitch("--mode", "vs-vscode");
            Mode = Mode.ToLower();

            SourceFileOrDirectory = Args[0];
            SourceFileOrDirectory = Environment.ExpandEnvironmentVariables(SourceFileOrDirectory);

            TargetFile = ParseStringParameterSwitch("-o", null);
            if (string.IsNullOrEmpty(TargetFile))
                TargetFile = ParseStringParameterSwitch("--output", null);

            if (Mode == "vs-vscode")
            {
                if (string.IsNullOrEmpty(TargetFile))
                    TargetFile = "~\\visualstudio-exported.code-snippets";
                TargetFile = VsCodeSnippet.FixupSnippetPath(TargetFile);                
            }
                
            
            Verbose = ParseParameterSwitch("-v");
            Recurse = ParseParameterSwitch("-r");

            ShowFileInExplorer = ParseParameterSwitch("-d");
            
          
            
            SnippetPrefix = ParseStringParameterSwitch("-p",null);
            if (string.IsNullOrEmpty(SnippetPrefix))
                SnippetPrefix = ParseStringParameterSwitch("--prefix",null);


            // Fix up for Visual Studio Snippet Path
            if(Mode.StartsWith("vs-"))
                SourceFileOrDirectory = VisualStudioSnippet.FixupSnippetPath(SourceFileOrDirectory);

            if (!string.IsNullOrEmpty(SourceFileOrDirectory) && 
                Directory.Exists(SourceFileOrDirectory))
                DirectoryMode = true;
            
            if (string.IsNullOrEmpty(SourceFileOrDirectory))
                throw new ArgumentException("Source File or Directory shouldn't be empty.");


            if (!DirectoryMode && !File.Exists(SourceFileOrDirectory))
                throw new ArgumentException("Source file or directory does not exist.");            
        }

    }
}
