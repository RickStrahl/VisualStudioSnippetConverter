using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Westwind.Utilities;

namespace Westwind.SnippetConverter
{

    [DebuggerDisplay("{Prefix} - {Title} - {Scope}")]
    public class VsCodeSnippet
    {
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("body")]
        public string[] Body { get; set; }

        public string Description { get; set; }



        [JsonIgnore]
        public string Title { get; set; }

        public static JsonSerializerSettings SerializerSettings { get; set; } =  new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
        };

        /// <summary>
        /// Returns the current object as Json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {            
            return JsonConvert.SerializeObject(this);
        }


        /// <summary>
        /// Attaches the current object to a JSON root as is required for
        /// a Visual Studio Code snippet library.
        /// </summary>
        /// <param name="rootJson"></param>
        public void AttachToJsonRoot(JObject rootJson)
        {
            var snippet = this;
            var title = snippet.Title.ToLower().Replace(" ", "-");
            var node = rootJson.SelectToken(title);
            if (node != null)
                rootJson.Remove(title);

            var ser = JsonSerializer.Create(SerializerSettings);
            JObject jSnippet = JObject.FromObject(snippet,ser);

            rootJson.Add(title,jSnippet);
        }


        /// <summary>
        /// Adds this snippet to a JSON vsCode Snippet file
        /// </summary>
        /// <param name="snippetFile"></param>
        public void ToFile(string snippetFile)
        {
            snippetFile = Environment.ExpandEnvironmentVariables(snippetFile);

            JObject rootJson = null;
            if (File.Exists(snippetFile))
            {
                using (StreamReader reader = new StreamReader(snippetFile))
                {
                    using (var tr = new JsonTextReader(reader))
                    {
                        rootJson = JObject.Load(tr);
                    }
                }
            }
            else
                rootJson = new JObject();

            var snippet = this;
            var node = rootJson.SelectToken(snippet.Title);
            if (node != null)
                rootJson.Remove(snippet.Title);

            JObject jSnippet = JObject.FromObject(snippet);
            rootJson.Add(snippet.Title, jSnippet);

            using (var sw = new StreamWriter(snippetFile))
            {
                using (var jw = new JsonTextWriter(sw))
                {
                    rootJson.WriteTo(jw);
                }
            }
        }


        /// <summary>
        /// Writes a set of snippets to Visual Studio Code Snippet File
        /// </summary>
        /// <param name="snippets"></param>
        /// <param name="snippetFile"></param>
        /// <param name="prefix">Optional prefix to assign snippet shortcuts and title</param>
        /// <param name="clearSnippetFile"></param>
        public static void ToFile(List<VisualStudioSnippet> snippets, string snippetFile, bool clearSnippetFile = false, string prefix = null)
        {
            snippetFile = Environment.ExpandEnvironmentVariables(snippetFile);

            if (clearSnippetFile && File.Exists(snippetFile))
                File.Delete(snippetFile);

            JObject rootJson = null;
            if (File.Exists(snippetFile))
            {
                using (StreamReader reader = new StreamReader(snippetFile))
                {
                    using (var tr = new JsonTextReader(reader))
                    {
                        rootJson = JObject.Load(tr);
                    }
                }
            }
            else
                rootJson = new JObject();

            
            foreach (var snippet in snippets)
            {
                var vsCodeSnippet = VsCodeSnippet.FromVisualStudioCodeSnippet(snippet);
                if (!string.IsNullOrEmpty(prefix))
                {
                    vsCodeSnippet.Prefix = prefix + vsCodeSnippet.Prefix;
                    vsCodeSnippet.Title = prefix + vsCodeSnippet.Title;
                }

                vsCodeSnippet.AttachToJsonRoot(rootJson);                
            }

            using (var sw = new StreamWriter(snippetFile))
            {
                using (var jw = new JsonTextWriter(sw) { Formatting = Formatting.Indented, })
                {
                    rootJson.WriteTo(jw);
                }
            }
        }

        #region Factory Methods

        /// <summary>
        /// Creates a VsCodeSnippet from a Visual Studio code snippet object
        /// </summary>
        /// <param name="snippet"></param>
        /// <returns></returns>
        public static VsCodeSnippet FromVisualStudioCodeSnippet(VisualStudioSnippet snippet)
        {
            var vsCode = new VsCodeSnippet
            {
                Prefix = snippet.Shortcut.ToLowerInvariant(),
                Title = snippet.Title,
                Scope = snippet.Language  ?? "plain,html,javascript,typescript,css",   
                Description = snippet.Description                                
            };
            
            int counter = 1;
            var delim = snippet.Delimiter ?? "$";
            var code = snippet.Code;

            vsCode.Scope = vsCode.Scope.ToLowerInvariant();          
                        
            // Fix up embedded references
            foreach (var dep in snippet.Declarations)
            {
                var id = dep.Id;
                var replace = delim + "{" + counter + "}";
                if (!string.IsNullOrEmpty(dep.Default))
                    replace = delim + "{" + counter + ":" + dep.Default + "}";                
                code = code.Replace($"{delim}{id}{delim}", replace);                                     
                counter++;
            }

            code = code.Replace($"{delim}end{delim}", "$0");

            vsCode.Body = StringUtils.GetLines(code);


            return vsCode;
        }


        /// <summary>
        /// Attaches a list of Visual Studio snippets to a root JSON instance
        /// </summary>
        /// <param name="snippets"></param>
        /// <param name="rootJson"></param>
        /// <returns></returns>
        public static JObject FromVisualStudioCodeSnippetList(List<VisualStudioSnippet> snippets, JObject rootJson = null)
        {

            if (rootJson == null)
                rootJson = new JObject();

            foreach (var vsSnippet in snippets)
            {
                var vsCodeSnippet = VsCodeSnippet.FromVisualStudioCodeSnippet(vsSnippet);
                vsCodeSnippet.AttachToJsonRoot(rootJson);
            }

            return rootJson;
        }

        #endregion

    }
}

