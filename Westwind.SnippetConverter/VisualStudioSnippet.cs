using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Westwind.SnippetConverter
{
    [DebuggerDisplay("{Shortcut} - {Title}")]
    public class VisualStudioSnippet
    {
        public string Code { get; set; }

        public string Title { get; set; }

        public string Shortcut { get; set; }

        public string Description { get; set; }


        public string Language { get; set; }
        public string Kind { get; set; }
        public string Delimiter { get; set; }


        public string HelpUrl { get; set; }
        public string Author { get; set; }

        public string Keywords { get; set; }

        public List<VisualStudioSnippetVariable> Declarations { get; set; } =
            new List<VisualStudioSnippetVariable>();

        #region Top Level Snippet Parsing
        public static VisualStudioSnippet ParseFromFile(string visualStudioSnippetFile)
        {
            visualStudioSnippetFile = FixupSnippetPath(visualStudioSnippetFile);
            var filetext = File.ReadAllText(visualStudioSnippetFile);
            return Parse(filetext);
        }


        /// <summary>
        /// Parses a Visual Studio snippet from string into a snippet object
        /// </summary>
        /// <param name="visualStudioSnippet"></param>
        /// <returns></returns>
        public static VisualStudioSnippet Parse(string visualStudioSnippet)
        {
            XElement el = XElement.Parse(visualStudioSnippet);
            var res = new XmlNamespaceManager(new NameTable());
            res.AddNamespace("z", el.GetDefaultNamespace().NamespaceName);
            
            var header = el.XPathSelectElement("./z:CodeSnippet/z:Header", res);
            var snippet = el.XPathSelectElement("./z:CodeSnippet/z:Snippet", res);
            var declarations = el.XPathSelectElements("./z:CodeSnippet/z:Snippet/z:Declarations/z:Literal", res);
            var code = el.XPathSelectElement("./z:CodeSnippet/z:Snippet/z:Code", res);

            var vsSnippet = new VisualStudioSnippet()
            {
                Code = code.Value,
                Kind = code.Attribute("Kind")?.Value,
                Language = code.Attribute("Language")?.Value,
                Delimiter = code.Attribute("Delimiter")?.Value,

                Shortcut = header?.XPathSelectElement("./z:Shortcut", res)?.Value,
                Keywords = header?.XPathSelectElement("./z:Keywords", res)?.Value,
                Description = header?.XPathSelectElement("./z:Description", res)?.Value,
                Title = header?.XPathSelectElement("./z:Title", res)?.Value,
                Author = header?.XPathSelectElement("./z:Author", res)?.Value,
                HelpUrl = header?.XPathSelectElement("./z:HelpUrl", res)?.Value
            };

            foreach (var decl in declarations)
            {
                var variable = new VisualStudioSnippetVariable();
                variable.Id = decl.XPathSelectElement("z:ID", res)?.Value;
                variable.ToolTip = decl.XPathSelectElement("z:ToolTip", res)?.Value;
                variable.Default = decl.XPathSelectElement("z:Default", res)?.Value;
                var editValue = decl.Attribute("Editable")?.Value;
                if (editValue != null && editValue == "true")
                    variable.Editable = true;

                vsSnippet.Declarations.Add(variable);
            }

            return vsSnippet;
        }

        /// <summary>
        /// Parses all snippets in a folder into a list of snippet objects.
        /// </summary>
        /// <param name="folder">Folder to grab snippets from</param>
        /// <param name="parseChildFolders">If true parses snippets in sub-folders</param>
        /// <returns></returns>
        public static List<VisualStudioSnippet> ParseSnippetFolder(string folder, bool parseChildFolders = false)
        {            
            var snippetList = new List<VisualStudioSnippet>();

            if (string.IsNullOrEmpty(folder))
                return snippetList;

            folder = FixupSnippetPath(folder);

            if (parseChildFolders)
            {
                var directories = Directory.GetDirectories(folder, "*.*");
                foreach (var dir in directories)
                {
                    var list = ParseSnippetFolder(dir, parseChildFolders);
                    if (list.Count > 0)
                        snippetList.AddRange(list);
                }
            }

            var files = Directory.GetFiles(folder, "*.snippet");
            foreach (var filename in files)
            {
                var snippet = ParseFromFile(filename);  
                if(snippet != null)
                    snippetList.Add(snippet);
            }

            return snippetList;
        }

        #endregion


        #region Export Routines

        public string ToXml()
        {
            throw new NotImplementedException();
        }

        public string ToFile(string filename)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Path Helpers

        /// <summary>
        /// Returns the latest version of Visual Studio installed
        /// </summary>
        /// <param name="yearVersion">optional version number to return as a number (ie. 2017, 2019 etc)
        /// if not passed returns the latest installed version (ie. highest year version)</param>
        /// <returns></returns>
        public static string GetUserVisualStudioVersionFolder(string yearVersion = null)
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (!string.IsNullOrEmpty(yearVersion))
            {
                string folder = Path.Combine(docs, "Visual Studio " + yearVersion);
                if (!Directory.Exists(folder))
                    return null;

                return folder;
            }

            var folders = Directory.GetDirectories(docs, "Visual Studio*.*");
            if (folders.Length < 1)
                return null;

            return folders.OrderByDescending(f => f).First();
        }

        public static string FixupSnippetPath(string folder)
        {
            if (string.IsNullOrEmpty(folder))
                return folder;

            folder = Environment.ExpandEnvironmentVariables(folder);
            if (folder.StartsWith("~"))
            {
                var part = "\\Code Snippets";

                if (folder.Contains("~2017"))
                    return folder.Replace("~2017", GetUserVisualStudioVersionFolder("2017") + part);

                if (folder.Contains("~2019"))
                    return folder.Replace("~2019", GetUserVisualStudioVersionFolder("2019") + part);
                if (folder.Contains("~2021"))
                    return folder.Replace("~2021", GetUserVisualStudioVersionFolder("2021") + part);
                if (folder.Contains("~2015\\"))
                    return folder.Replace("~2015", GetUserVisualStudioVersionFolder("2015") + part);
                if (folder.Contains("~2012"))
                    return folder.Replace("~2012", GetUserVisualStudioVersionFolder("2012") + part);

                return folder.Replace("~\\",GetUserVisualStudioVersionFolder() + part);
            }
            
            return folder;
        }

        #endregion
    }



    [DebuggerDisplay("{Id}")]
    public class VisualStudioSnippetVariable
    {
        public string Id { get; set; }
        public string Default { get; set; }
        public string Type { get; set; }

        public string ToolTip { get; set; }

        public bool Editable { get; set; }
    }
}
