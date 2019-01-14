using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace Westwind.SnippetConverter
{
    [DebuggerDisplay("{Shortcut} - {Description} - {Scope}")]
    public class JetBrainsLiveTemplate
    {

        public string Id { get; set; } = Guid.NewGuid().ToString("N").ToUpper();

        public string Description { get; set; }

        public string Shortcut { get; set; }

        public string Scope { get; set; }

        public string Code { get; set; }

        public List<VisualStudioSnippetVariable> Declarations { get; set; } = new List<VisualStudioSnippetVariable>();


        /// <summary>
        /// Returns the result as an XML string with a root host element
        /// </summary>
        /// <returns></returns>
        public string ToXml(bool addRootElement = true)
        {
            string scope;            
            string lScope = Scope.ToLowerInvariant();
            string xml = null;

            switch (lScope)
            {
                case "csharp":
                case "c#":
                    scope = "InCSharpFile";
                    break;
                case "vb":
                case "vbnet":
                    scope = "InVBFile";
                    break;
                case "html":                    
                    scope = "InHtmlLikeTag";
                    break;
                case "razor":
                    scope = "InRazorTag";
                    break;
                case "css":
                    scope = "InCss";
                    break;
                default:
                    // invalid format
                    throw new ArgumentException("Unsupported snippet format:  " + Scope + ". Snippet: " + Shortcut);
            }


            xml = $@"<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/@KeyIndexDefined"">True</s:Boolean>
<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Applicability/=Live/@EntryIndexedValue"">True</s:Boolean>
<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Reformat/@EntryValue"">True</s:Boolean>
<s:String x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Shortcut/@EntryValue"">{Utils.XmlString(Shortcut)}</s:String>
<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/ShortenQualifiedReferences/@EntryValue"">True</s:Boolean>

<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Scope/=C3001E7C0DA78E4487072B7E050D86C5/@KeyIndexDefined"">True</s:Boolean>
<s:String x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Scope/=C3001E7C0DA78E4487072B7E050D86C5/Type/@EntryValue"">{scope}</s:String>

<s:String x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Text/@EntryValue"">{Utils.XmlString(Code)}</s:String>";
            

            int varCount = 0;
            foreach (var variable in this.Declarations)
            {
                xml +=
                    $@"<s:Boolean x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Field/={variable.Id}/@KeyIndexDefined"">True</s:Boolean>
<s:String x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Field/={variable.Id}/Expression/@EntryValue"">complete()</s:String>
<s:Int64 x:Key=""/Default/PatternsAndTemplates/LiveTemplates/Template/={Id}/Field/={variable.Id}/Order/@EntryValue"">{varCount}</s:Int64>";
                varCount++;
            }

            if (addRootElement)
                return
                    $@"<wpf:ResourceDictionary xml:space=""preserve"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:s=""clr-namespace:System;assembly=mscorlib"" xmlns:ss=""urn:shemas-jetbrains-com:settings-storage-xaml"" xmlns:wpf=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">" +
                    xml + "</wpf:ResourceDictionary>";


            return xml;
        }


        public bool AddCSharpTemplate()
        {
            string xml = ToXml(true);
            var templateXml = XDocument.Parse(xml);

            var configFolder = GetRiderConfigFolder();
            if (string.IsNullOrEmpty(configFolder))
                throw new FileNotFoundException("Rider configuration folder not found.");

            string settingsFile = GetRiderConfigurationFile();

            var doc = XDocument.Load(settingsFile);
            var settingElements = templateXml.Root.Elements().ToList();
            foreach (var el in settingElements)
            {
                doc.Root.Add(el);
            }

            doc.Save(settingsFile);

            return true;
        }


   



        #region Factory Methods

        /// <summary>
        /// Creates a VsCodeSnippet from a Visual Studio code snippet object
        /// </summary>
        /// <param name="snippet"></param>
        /// <returns></returns>
        public static JetBrainsLiveTemplate FromVisualStudioCodeSnippet(VisualStudioSnippet snippet)
        {
            var jbTemplate = new JetBrainsLiveTemplate()
            {
                Shortcut = snippet.Shortcut.ToLowerInvariant(),
                Scope = snippet.Language ?? "csharp",
                Description = snippet.Description,
                Code = snippet.Code,
                Declarations = snippet.Declarations
            };

            jbTemplate.Code = jbTemplate.Code.Replace("$end$", "$END$");

            return jbTemplate;
        }

        #endregion

        #region Worker Methods

        /// <summary>
        /// Attaches a list of Visual Studio snippets to a root JSON instance
        /// </summary>
        /// <param name="folder">Folder to import</param>
        /// <param name="snippetPrefix">optional prefix to prepend to template names</param>
        /// <param name="recursive">search child folders if true</param>
        /// <returns></returns>
        public static void AddVisualStudioSnippets(string folder,string snippetPrefix = null, bool recursive = false)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException("Snippet folder doesn't exist: " + folder);

            var snippets = VisualStudioSnippet.ParseSnippetFolder(folder, recursive);
            if (snippets == null)
                return;
            
            foreach (var vsSnippet in snippets)
            {
                var jbCodeSnippet = FromVisualStudioCodeSnippet(vsSnippet);
                jbCodeSnippet.AddCSharpTemplate();
            }
            
        }


        /// <summary>
        /// Attaches a list of Visual Studio snippets to a root JSON instance
        /// </summary>
        /// <param name="snippets"></param>
        /// <param name="rootJson"></param>
        /// <returns></returns>
        public static void AddVisualStudioSnippets(List<VisualStudioSnippet> snippets, string snippetPrefix = null)
        {            
            foreach (var vsSnippet in snippets)
            {
                var jbCodeSnippet = FromVisualStudioCodeSnippet(vsSnippet);
                jbCodeSnippet.AddCSharpTemplate();
            }            

        }
        
        #endregion


        #region Helpers

        /// <summary>
        /// Finds the Rider Configuration folder in the Home path
        /// </summary>
        /// <returns>Rider config path or null if not found.</returns>
        public static string GetRiderConfigFolder()
        {
            var home = Environment.GetEnvironmentVariable("USERPROFILE");
            if (string.IsNullOrEmpty(home))
                home = Environment.GetEnvironmentVariable("HOME");


            var folder = Directory.GetDirectories(home, ".Rider20*.*")?.Max(s => s);
            if (!Directory.Exists(folder))
                return null;

            return folder;
        }

        /// <summary>
        /// Returns Rider's GlobalSettingsStorage.DotSettings file.
        /// </summary>
        /// <returns></returns>
        public static string GetRiderConfigurationFile()
        {
            return Path.Combine(GetRiderConfigFolder(), @"config\resharper-host\GlobalSettingsStorage.DotSettings");
        }
        #endregion
    }


//    <s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/@KeyIndexDefined">True</s:Boolean>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Applicability/=Live/@EntryIndexedValue">True</s:Boolean>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Field/=VALUE/@KeyIndexDefined">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Field/=VALUE/Expression/@EntryValue">complete()</s:String>
//	<s:Int64 x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Field/=VALUE/Order/@EntryValue">0</s:Int64>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Reformat/@EntryValue">True</s:Boolean>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Scope/=C3001E7C0DA78E4487072B7E050D86C5/@KeyIndexDefined">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Scope/=C3001E7C0DA78E4487072B7E050D86C5/CustomProperties/=minimumLanguageVersion/@EntryIndexedValue">2.0</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Scope/=C3001E7C0DA78E4487072B7E050D86C5/Type/@EntryValue">InCSharpFile</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Shortcut/@EntryValue">ifempty</s:String>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/ShortenQualifiedReferences/@EntryValue">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=4FB8D58FA7B4A44EA28ABC8331637986/Text/@EntryValue">if(string.IsNullOrEmpty($VALUE$))
//    $END$    </s:String>
//    
//<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/@KeyIndexDefined">True</s:Boolean>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Applicability/=Live/@EntryIndexedValue">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Description/@EntryValue">Property with INotifyPropertyChanged</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=TYPE/Expression/@EntryValue">suggestVariableType()</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=VARNAME/Expression/@EntryValue">suggestVariableName()</s:String>
//	
//	
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=TYPE/@KeyIndexDefined">True</s:Boolean>
//	<s:Int64 x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=TYPE/Order/@EntryValue">0</s:Int64>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=VARNAME/@KeyIndexDefined">True</s:Boolean>
//	<s:Int64 x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Field/=VARNAME/Order/@EntryValue">1</s:Int64>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Reformat/@EntryValue">True</s:Boolean>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Scope/=C3001E7C0DA78E4487072B7E050D86C5/@KeyIndexDefined">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Scope/=C3001E7C0DA78E4487072B7E050D86C5/CustomProperties/=minimumLanguageVersion/@EntryIndexedValue">2.0</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Scope/=C3001E7C0DA78E4487072B7E050D86C5/Type/@EntryValue">InCSharpFile</s:String>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Shortcut/@EntryValue">proipc</s:String>
//	<s:Boolean x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/ShortenQualifiedReferences/@EntryValue">True</s:Boolean>
//	<s:String x:Key="/Default/PatternsAndTemplates/LiveTemplates/Template/=FC02D5F819511843AAE1E591641DDFF3/Text/@EntryValue">public $TYPE$ $VARNAME$ 
//{
//    get =&gt; _$VARNAME$;
//    set 
//    {
//        if(value == _$VARNAME$) return;
//        
//        _$VARNAME$ = value;        
//        OnPropertyChanged("$VARNAME$");   
//    }
//}
//public $TYPE$ _$VARNAME$;
//$END$</s:String>
}