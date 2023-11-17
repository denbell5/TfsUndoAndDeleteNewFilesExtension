namespace TfsUndoAndDeleteNewFilesExtension.CmdManagement
{
    public class CmdExecutionException : Exception
    {
        public int ErrorCode { get; set; }
        public string Output { get; set; }
        public string Command { get; set; }

        public CmdExecutionException(
            int errorCode,
            string output,
            string command,
            Exception innerException = null)
            : base(
                 BuildExceptionMessage(errorCode, output, command),
                 innerException)
        {
            ErrorCode = errorCode;
            Output = output;
            Command = command;
        }

        private static string BuildExceptionMessage(
            int errorCode,
            string output,
            string commandText)
        {
            return $"Execution of command '{commandText}' failed " +
                $"with code {errorCode} and output{Environment.NewLine}" +
                $"{output}";
        }
    }
}
