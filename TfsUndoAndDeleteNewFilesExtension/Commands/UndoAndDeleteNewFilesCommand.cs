using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System.IO;
using System.Linq;

namespace TfsUndoAndDeleteNewFilesExtension
{
    [Command(PackageIds.UndoAndDeleteNewFilesCommand)]
    internal sealed class UndoAndDeleteNewFilesCommand : BaseCommand<UndoAndDeleteNewFilesCommand>
    {
        private VersionControlExt _versionControlExtension;

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            // Warning VSTHRD010
            // Accessing "DTE" should only be done on the main thread. Await JoinableTaskFactory.SwitchToMainThreadAsync() first
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE.DTE dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            _versionControlExtension = dte.GetObject(typeof(VersionControlExt).FullName) as VersionControlExt;

            var pendingChangesBefore = GetPendingChangesOfTypeAdd();

            var executed = await VS.Commands.ExecuteAsync("TeamFoundationContextMenus.PendingChangesPageChangestoInclude.TfsContextPendingChangesPageUndo");

            if (!executed)
            {
                // log
                return;
            }

            var pendingChangesAfter = GetPendingChangesOfTypeAdd();

            var undoneChanges = pendingChangesBefore.Except(pendingChangesAfter).ToList();

            foreach (var pendingChange in undoneChanges)
            {
                if (File.Exists(pendingChange.LocalItem))
                {
                    File.Delete(pendingChange.LocalItem);
                }
            }
        }

        private PendingChange[] GetPendingChangesOfTypeAdd()
        {
            return _versionControlExtension.PendingChanges.IncludedChanges
                .Where(x => x.ChangeType.HasFlag(ChangeType.Add))
                .Concat(_versionControlExtension.PendingChanges.ExcludedChanges
                    .Where(x => x.ChangeType.HasFlag(ChangeType.Add)))
                .ToArray();
        }
    }
}
