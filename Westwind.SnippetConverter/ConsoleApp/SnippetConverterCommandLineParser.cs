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
        public string Mode { get; set; }
        
        
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
            SourceFileOrDirectory = Args[0];
            TargetFile = ParseStringParameterSwitch("-o", null);            
            Verbose = ParseParameterSwitch("-v");
            ShowFileInExplorer = ParseParameterSwitch("-s");
            Mode = ParseStringParameterSwitch("-m",null);
            if (string.IsNullOrEmpty(Mode))
                Mode = ParseStringParameterSwitch("--mode", "vs-vscode");
            
            SnippetPrefix = ParseStringParameterSwitch("-p",null);
            if (string.IsNullOrEmpty(SnippetPrefix))
                SnippetPrefix = ParseStringParameterSwitch("--prefix",null);
            
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
