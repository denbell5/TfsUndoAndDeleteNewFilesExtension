using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace TfsUndoAndDeleteNewFilesExtension.CmdManagement
{
    public class CmdExecutor : ICmdExecutor
    {
        /// <summary>
        /// Full path or executable name to execute (for example - dotnet)
        /// </summary>
        public string Executable { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executable">Full path or executable name to execute (for example - dotnet)</param>
        /// <exception cref="ArgumentNullException"></exception>
        public CmdExecutor(string executable)
        {
            if (string.IsNullOrEmpty(executable))
                throw new ArgumentNullException(nameof(executable));

            Executable = executable;
        }

        public CmdResult ExecuteCommand(string processArguments, bool throwIfErrorCode = true)
        {
            Process process = new Process();

            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                FileName = Executable,
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = processArguments
            };

            process.StartInfo = processStartInfo;

            StringBuilder sb = new StringBuilder();

            process.OutputDataReceived += (sender, dataReceivedEventArgs) =>
            {
                if (!string.IsNullOrEmpty(dataReceivedEventArgs.Data))
                {
                    sb.AppendLine(dataReceivedEventArgs.Data);
                }
            };

            process.ErrorDataReceived += (sender, dataReceivedEventArgs) =>
            {
                if (!string.IsNullOrEmpty(dataReceivedEventArgs.Data))
                {
                    sb.AppendLine(dataReceivedEventArgs.Data);
                }
            };
            
            try
            {
                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
            catch (Win32Exception ex)
            {
                if (ex.Message == "The system cannot find the file specified")
                {
                    throw new CmdExecutableNotFoundException(Executable, ex);
                }
                
                throw;
            }

            string output = sb.ToString();

            if (throwIfErrorCode && process.ExitCode != 0)
            {
                throw new CmdExecutionException(
                    errorCode: process.ExitCode,
                    output: output,
                    command: $"{Executable} {processArguments}");
            }

            return new CmdResult
            {
                ExitCode = process.ExitCode,
                Output = output,
            };
        }
    }
}
