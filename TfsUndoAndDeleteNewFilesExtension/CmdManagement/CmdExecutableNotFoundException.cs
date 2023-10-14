namespace TfsUndoAndDeleteNewFilesExtension.CmdManagement
{
    public class CmdExecutableNotFoundException : Exception
    {
        public string Executable { get; set; }

        public CmdExecutableNotFoundException(string executable, Exception innerException = null)
            : base(
                  $"Command line could not found the executable: '{executable}'",
                  innerException)
        {
            Executable = executable;
        }
    }
}
