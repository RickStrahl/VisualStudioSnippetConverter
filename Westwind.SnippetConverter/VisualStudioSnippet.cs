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


        public static VisualStudioSnippet ParseFromFile(string visualStudioSnippetFile)
        {
            visualStudioSnippetFile = Environment.ExpandEnvironmentVariables(visualStudioSnippetFile);
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
        /// Parses all snippets in a folder into a list of snippets
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static List<VisualStudioSnippet> ParseSnippetFolder(string folder)
        {
            folder = Environment.ExpandEnvironmentVariables(folder);

            var snippetList = new List<VisualStudioSnippet>();

            var files = Directory.GetFiles(folder, "*.snippet");
            foreach (var filename in files)
            {
                var snippet = ParseFromFile(filename);
                if (snippet != null)
                    snippetList.Add(snippet);
            }

            return snippetList;
        }

        public string ToXml()
        {
            throw new NotImplementedException();
        }

        public string ToFile(string filename)
        {
            throw new NotImplementedException();
        }
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
