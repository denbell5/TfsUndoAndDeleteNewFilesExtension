using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TfsUndoAndDeleteNewFilesExtension
{
    [Command(PackageIds.UndoAndDeleteNewFilesCommand)]
    internal sealed class UndoAndDeleteNewFilesCommand : BaseCommand<UndoAndDeleteNewFilesCommand>
    {
        private VersionControlExt _versionControlExtension;
        private OutputWindowPane _outputWindowPane;

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await InitOutputWindowPaneAsync();
            await InitVersionControlExtAsync();

            await LogAsync($"Executing {this.GetType().Name} [{DateTime.Now}]");

            var pendingChangesBefore = GetPendingChangesOfTypeAdd();
            await LogAsync($"Found {pendingChangesBefore.Length} 'adds' before undo");

            string commandName = "TeamFoundationContextMenus.PendingChangesPageChangestoInclude.TfsContextPendingChangesPageUndo";
            await LogAsync($"Executing {commandName}");
            var executed = await VS.Commands.ExecuteAsync(commandName);

            if (!executed)
            {
                await LogAsync($"Execution failed, aborting");
                return;
            }
            else
                await LogAsync($"Execution succeeded");

            var pendingChangesAfter = GetPendingChangesOfTypeAdd();
            await LogAsync($"Found {pendingChangesAfter.Length} 'adds' after undo");

            var undoneChanges = pendingChangesBefore.Except(pendingChangesAfter).ToArray();
            await LogAsync($"Found {undoneChanges.Length} undone 'adds'");

            foreach (var pendingChange in undoneChanges)
            {
                if (File.Exists(pendingChange.LocalItem))
                {
                    await LogAsync($"Deleting file {pendingChange.LocalItem}");
                    File.Delete(pendingChange.LocalItem);
                }
                else
                {
                    await LogAsync($"File {pendingChange.LocalItem} not found, skipping deletion");
                }
            }
        }

        private async Task LogAsync(string message)
        {
            if (_outputWindowPane == null)
            {
                Trace.TraceError($"'{_outputWindowPane}' is null");
            }

            var lines = message.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.None);

            foreach (var line in lines)
            {
                await _outputWindowPane.WriteLineAsync(line);
            }
        }

        private async Task InitOutputWindowPaneAsync()
        {
            var tfsSourceControlOutputPaneGuid = new Guid("0EE13505-C0BC-40C3-8F1D-0F65A9F71616");
            _outputWindowPane = await VS.Windows.GetOutputWindowPaneAsync(tfsSourceControlOutputPaneGuid);
            if (_outputWindowPane == null)
            {
                // if can't get tfs pane, create our own
                _outputWindowPane = await VS.Windows.CreateOutputWindowPaneAsync("TfsUndoAndDeleteNewFilesExtension");
            }
            await _outputWindowPane.ActivateAsync();
        }

        private async Task InitVersionControlExtAsync()
        {
            // Warning VSTHRD010
            // Accessing "DTE" should only be done on the main thread. Await JoinableTaskFactory.SwitchToMainThreadAsync() first
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            EnvDTE.DTE dte = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            _versionControlExtension = dte.GetObject(typeof(VersionControlExt).FullName) as VersionControlExt;
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
