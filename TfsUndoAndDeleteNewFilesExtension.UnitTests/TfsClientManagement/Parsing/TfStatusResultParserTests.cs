using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsUndoAndDeleteNewFilesExtension.TfsClientManagement;

namespace TfsUndoAndDeleteNewFilesExtension.UnitTests.TfsClientManagement.Parsing
{
    [TestClass]
    public class TfStatusResultParserTests
    {
        [TestMethod]
        public void Parse_WhenTwoItems_ReturnsResultWithTwoItems()
        {
            string cmdOutput = @"
$/some/path/someFile1.cs
  User       : Some User
  Date       : Friday, October 13, 2023 6:06:53 PM
  Lock       : none
  Change     : add
  Workspace  : SOMEWORKSPACE
  Local item : [SOMEWORKSPACE] C:\some\path\someFile1.cs
  File type  : utf-8

$/some/path/someFolder;C761745
  User       : Some User
  Date       : Friday, October 13, 2023 6:51:16 PM
  Lock       : none
  Change     : delete
  Workspace  : SOMEWORKSPACE
  Local item : [SOMEWORKSPACE] C:\some\path\someFolder

2 change(s)
";
            TfStatusResult result = new TfStatusResultParser().Parse(cmdOutput);
            Assert.AreEqual(2, result.PendingChanges.Count);
            Assert.AreEqual(ChangeType.Add, result.PendingChanges[0].ChangeType);
            Assert.AreEqual(ChangeType.Delete, result.PendingChanges[1].ChangeType);
            Assert.AreEqual("C:\\some\\path\\someFile1.cs", result.PendingChanges[0].LocalFullPath);
            Assert.AreEqual("C:\\some\\path\\someFolder", result.PendingChanges[1].LocalFullPath);
        }
    }
}
