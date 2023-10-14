using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing;

namespace TfsUndoAndDeleteNewFilesExtension.UnitTests.TfsClientManagement.Parsing
{
    [TestClass]
    public class LocalFullPathParserTests
    {
        [TestMethod]
        public void TryParse_ParsesMatchingLine()
        {
            string line = @"  Local item : [UA2103NPC05] C:\some\path\someFile.cs";
            bool parsed = new LocalFullPathParser().TryParse(line, out string value);
            Assert.IsTrue(parsed);
            Assert.AreEqual(@"C:\some\path\someFile.cs", value);
        }
    }
}
