namespace TfsUndoAndDeleteNewFilesExtension.CmdManagement
{
    public interface ICmdExecutor
    {
        CmdResult ExecuteCommand(string processArguments, bool throwIfErrorCode = true);
    }
}
