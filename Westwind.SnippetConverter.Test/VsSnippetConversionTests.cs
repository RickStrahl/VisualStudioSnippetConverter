using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Westwind.SnippetConverter.Test
{
    [TestFixture]
    public class VsSnippetConversionTests
    {

        [Test]
        public void GetSnippetObjectTest()
        {
            var snippet = VisualStudioSnippet.ParseFromFile(
                "C:\\Users\\rstrahl\\Documents\\Visual Studio 2017\\Code Snippets\\Visual Web Developer\\My HTML Snippets\\WebConnection\\ww-fv.snippet");

            Assert.IsNotNull(snippet);
            Console.WriteLine(snippet);
            Assert.IsTrue(snippet.Shortcut == "ww-fv");
        }

        [Test]
        public void CreateVsCodeSnippetfromVisualStudioSnippetTest()
        {   var snippet = VisualStudioSnippet.ParseFromFile(
                "C:\\Users\\rstrahl\\Documents\\Visual Studio 2017\\Code Snippets\\Visual Web Developer\\My HTML Snippets\\WebConnection\\ww-fv.snippet");

            Assert.IsNotNull(snippet);           
            Assert.IsTrue(snippet.Shortcut == "ww-fv");


            var vsCodeSnippet = VsCodeSnippet.FromVisualStudioCodeSnippet(snippet);
            Console.WriteLine(vsCodeSnippet.ToJson());

            Assert.IsNotNull(vsCodeSnippet);
            Assert.AreEqual(snippet.Shortcut, vsCodeSnippet.Prefix);            
        }



        [Test]
        public void CreateVsCodeSnippetfromVisualStudioSnippetOnRootTest()
        {
            var converter = new SnippetToCodeSnippet();
            var snippet = VisualStudioSnippet.ParseFromFile(
                "C:\\Users\\rstrahl\\Documents\\Visual Studio 2017\\Code Snippets\\Visual Web Developer\\My HTML Snippets\\WebConnection\\ww-fv.snippet");

            Assert.IsNotNull(snippet);
            Assert.IsTrue(snippet.Shortcut == "ww-fv");


            var vsCodeSnippet = VsCodeSnippet.FromVisualStudioCodeSnippet(snippet);            

            var jsonRoot = new JObject();
            vsCodeSnippet.AttachToJsonRoot(jsonRoot);

            Console.WriteLine(jsonRoot);

            Assert.IsNotNull(vsCodeSnippet);
            Assert.AreEqual(snippet.Shortcut, vsCodeSnippet.Prefix);
        }

        [Test]
        public void GetSnippetFolderTest()
        {
            var snippets = VisualStudioSnippet.ParseSnippetFolder(
                "C:\\Users\\rstrahl\\Documents\\Visual Studio 2017\\Code Snippets\\Visual Web Developer\\My HTML Snippets\\WebConnection");

            Assert.IsNotNull(snippets);
            Assert.IsTrue(snippets.Count > 0);

        }


        [Test]
        public void ConvertFolderToVsCodeSnippetJsonList()
        {
            var snippets = VisualStudioSnippet.ParseSnippetFolder(
                "C:\\Users\\rstrahl\\Documents\\Visual Studio 2017\\Code Snippets\\Visual Web Developer\\My HTML Snippets\\WebConnection");

            var jsonRoot = new JObject();

            VsCodeSnippet.FromVisualStudioCodeSnippetList(snippets, jsonRoot);

            Console.WriteLine(jsonRoot);

            Assert.IsNotNull(snippets);
            Assert.IsTrue(snippets.Count > 0);

        }

        [Test]
        public void ConvertHtmlFolderToVsCodeSnippetJsonFile()
        {
            var visualStudioSnippetFolder =
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual Web Developer\My HTML Snippets\WebConnection";
            var vsCodeSnippetFile =
                @"C:\Users\rstrahl\AppData\Roaming\Code\User\snippets\wc-WebConnection-VsCode.code-snippets";

            var snippets = VisualStudioSnippet.ParseSnippetFolder(visualStudioSnippetFolder);

            var jsonRoot = new JObject();
            VsCodeSnippet.ToFile(snippets, vsCodeSnippetFile, clearSnippetFile: true );

            Console.WriteLine(File.ReadAllText(vsCodeSnippetFile));

            Assert.IsNotNull(snippets);
            Assert.IsTrue(snippets.Count > 0);
        }


        [Test]
        public void ConvertCSharpFolderToVsCodeSnippetJsonFile()
        {
            var visualStudioSnippetFolder =
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets\Visual C#\My Code Snippets";
            var vsCodeSnippetFile =
                @"C:\Users\rstrahl\AppData\Roaming\Code\User\snippets\ww-csharp.code-snippets";

            var snippets = VisualStudioSnippet.ParseSnippetFolder(visualStudioSnippetFolder);

            var jsonRoot = new JObject();
            VsCodeSnippet.ToFile(snippets, vsCodeSnippetFile, true, "ww-");

            Console.WriteLine(File.ReadAllText(vsCodeSnippetFile));

            Assert.IsNotNull(snippets);
            Assert.IsTrue(snippets.Count > 0);

        }

        [Test]
        public void ConvertSnippetsRecursiveToVsCodeSnippetJsonFile()
        {
            var visualStudioSnippetFolder =
                @"C:\Users\rstrahl\Documents\Visual Studio 2017\Code Snippets";
            var vsCodeSnippetFile =
                @"C:\Users\rstrahl\AppData\Roaming\Code\User\snippets\ww2-csharp.code-snippets";

            if (File.Exists(vsCodeSnippetFile))
                File.Delete(vsCodeSnippetFile);

            var snippets = VisualStudioSnippet.ParseSnippetFolder(visualStudioSnippetFolder,true);

            var jsonRoot = new JObject();
            VsCodeSnippet.ToFile(snippets, vsCodeSnippetFile, true, "ww-");

            Console.WriteLine(File.ReadAllText(vsCodeSnippetFile));

            Assert.IsNotNull(snippets);
            Assert.IsTrue(snippets.Count > 0);

        }

        [Test]
        public void JsonWithDotBug()
        {
            var json = "{ \"asp.net-expression\": \"value\", \"wc-button\": \"value3\" }";

            var job = JObject.Parse(json);
            var item = job.SelectToken("wc-button");
            Assert.IsNotNull(item);  //works
            Console.WriteLine(item.Value<string>());


            job.TryGetValue("asp.net-expression", out JToken token);
            //item = job.SelectToken("asp.net-expression");
            Assert.IsNotNull(token,"with . in name fails");  //fails
            Console.WriteLine(token.Value<string>());
        }


    }
}
