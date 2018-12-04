using System;
using System.IO;
using NUnit.Framework;

namespace Westwind.SnippetConverter.Test
{
    [TestFixture]
    public class JetBrainsLiveTemplatesTests
    {
        [Test]
        public void ConvertVsSnippetToJbTemplate()
        {
            var snippet = VisualStudioSnippet.ParseFromFile(
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet");

            var template = JetBrainsLiveTemplate.FromVisualStudioCodeSnippet(snippet);
            
            Assert.IsNotNull(template);
            Assert.IsTrue(template.Shortcut.Equals("proipc",StringComparison.InvariantCulture));
            Assert.IsTrue(template.Code.Contains("OnPropertyChanged("));
        }

        [Test]
        public void GenerateXmlFromCSharpSnippet()
        {
            var snippet = VisualStudioSnippet.ParseFromFile(
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet");
            
            
            var template = JetBrainsLiveTemplate.FromVisualStudioCodeSnippet(snippet);
            var xml = template.ToXml();
            Console.WriteLine(xml);
        }

        [Test]
        public void AddToSettingsFromCSharpSnippet()
        {
            var snippet = VisualStudioSnippet.ParseFromFile(
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets\proIPC.snippet");


            var template = JetBrainsLiveTemplate.FromVisualStudioCodeSnippet(snippet);

            bool result = template.AddCSharpTemplate();

            Assert.IsTrue(result);
        }

        [Test]
        public void GetRiderConfigFolderTest()
        {
            var path = JetBrainsLiveTemplate.GetRiderConfigFolder();
            
            Console.WriteLine(path);
            Assert.IsTrue(Directory.Exists(path));
            
        }

        [Test]
        public void ConvertVsCSharpSnippets()
        {
            var visualStudioSnippetFolder =
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets";
           
            JetBrainsLiveTemplate.AddVisualStudioSnippets(visualStudioSnippetFolder);
            

        }
        
        [Test]
        public void ConvertVsHtmlSnippets()
        {
            var visualStudioSnippetFolder =
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual Web Developer\My HTML Snippets\WebConnection";
           
            JetBrainsLiveTemplate.AddVisualStudioSnippets(visualStudioSnippetFolder);
            
        }
        
                
    }
}