using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsUndoAndDeleteNewFilesExtension.CmdManagement;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement
{
    public class TfsClient : ITfsClient
    {
        private readonly ICmdExecutor _cmdExecutor;

        public TfsClient()
        {
            _cmdExecutor = new CmdExecutor(FindTfExePath());
        }

        public TfStatusResult ExecuteStatus()
        {
            CmdResult cmdResult = _cmdExecutor.ExecuteCommand("status /format:Detailed");
            var parser = new TfStatusResultParser();
            return parser.Parse(cmdResult.Output);
        }

        private static string FindTfExePath()
        {
            // try find 'tf' in the PATH
            var pathValues = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in pathValues.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, "TF.exe");
                if (File.Exists(fullPath))
                    return "tf";
            }

            // try find TF in default locations
            string[] possiblePaths = new string[]
            {
                @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe",
                @"C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe",
                @"C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe",
                @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe",
            };

            string existingPath = possiblePaths.FirstOrDefault(x => x != null && File.Exists(x));

            if (existingPath == null)
            {
                string searchedLocationsString = string.Join(Environment.NewLine, possiblePaths);
                throw new Exception(
                    $"Path to TF.exe was not found." +
                    $"Searched locations:{Environment.NewLine}{searchedLocationsString}" +
                    $"{Environment.NewLine}Please add path to the folder containing TF.exe to " +
                    $"your PATH environment variable (user).");
            }

            return existingPath;
        }
    }
}
