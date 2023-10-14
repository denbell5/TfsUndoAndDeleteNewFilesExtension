using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing;

namespace TfsUndoAndDeleteNewFilesExtension.UnitTests.TfsClientManagement.Parsing
{
    [TestClass]
    public class ChangeTypeParserTests
    {
        [TestMethod]
        public void TryParse_ParsesMatchingLine()
        {
            string line = "  Change     : edit, add, rollback";
            bool parsed = new ChangeTypeParser().TryParse(line, out ChangeType value);
            Assert.IsTrue(parsed);
            Assert.AreEqual(ChangeType.Edit | ChangeType.Add | ChangeType.Rollback, value);
            Assert.IsTrue(value.HasFlag(ChangeType.Add));
        }
    }
}
