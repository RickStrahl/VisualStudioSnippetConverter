using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Westwind.SnippetConverter.Test
{
    [TestFixture]
    public class VsSnippetHelperTests
    {
        [Test]
        public void GetLatestVisualStudioFolderPathTest()
        {
            var path = VisualStudioSnippet.GetUserVisualStudioVersionFolder(null);

            Console.WriteLine(path);

            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void GetSpecificVisualStudioFolderPathTest()
        {
            var path = VisualStudioSnippet.GetUserVisualStudioVersionFolder("2017");

            Console.WriteLine(path);

            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void ParseSnippetPathTest()
        {
            var path = VisualStudioSnippet.FixupSnippetPath(@"~\2017\Visual C#\My Code Snippets");
            Console.WriteLine(path);
            Assert.IsFalse(path.Contains("~\\"));
            Assert.IsTrue(path == 
                          Path.Combine(VisualStudioSnippet.GetUserVisualStudioVersionFolder("2017"),"Visual C#\\My Code Snippets"));
        }


    }
}
