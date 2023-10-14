using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsUndoAndDeleteNewFilesExtension.CmdManagement;

namespace TfsUndoAndDeleteNewFilesExtension.UnitTests.CmdManagement
{
    [TestClass]
    public class CmdExecutorTests
    {
        [TestMethod]
        public void ExecuteCommand_WhenCommandExists_ReturnsZeroCodeAndOutput()
        {
            var executor = new CmdExecutor("dotnet");
            CmdResult result = executor.ExecuteCommand("--help");
            Assert.AreEqual(0, result.ExitCode);
            Assert.IsTrue(result.Output.StartsWith("Usage: dotnet"));
        }

        [TestMethod]
        public void ExecuteCommand_WhenCommandDoesNotExist_ThrowsException()
        {
            var executor = new CmdExecutor("dotnet1");
            var exception = Assert.ThrowsException<CmdExecutableNotFoundException>(
                () => executor.ExecuteCommand("--help"));

            Assert.AreEqual("Command line could not found the executable: 'dotnet1'", exception.Message);
        }

        [TestMethod]
        public void ExecuteCommand_WhenCommandArgumentsAreInvalid_ReturnsErrorCodeAndOutput()
        {
            var executor = new CmdExecutor("dotnet");
            CmdResult result = executor.ExecuteCommand("aaa123");
            Assert.AreEqual(1, result.ExitCode);
            Assert.IsTrue(result.Output.Contains(
                "You intended to execute a .NET program, " +
                "but dotnet-aaa123 does not exist."));
        }
    }
}
