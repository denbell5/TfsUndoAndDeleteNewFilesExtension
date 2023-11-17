using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TfsUndoAndDeleteNewFilesExtension.TfsClientManagement;

namespace TfsUndoAndDeleteNewFilesExtension
{
    [Command(PackageIds.UndoAndDeleteNewFilesCommand)]
    internal sealed class UndoAndDeleteNewFilesCommand : BaseCommand<UndoAndDeleteNewFilesCommand>
    {
        private OutputWindowPane _outputWindowPane;
        private ITfsClient _tfsClient;

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await InitOutputWindowPaneAsync();
            try
            {
                InitTfsClient();

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

                var undoneChanges = ResolveUndoneChanges(pendingChangesBefore, pendingChangesAfter);
                await LogAsync($"Found {undoneChanges.Length} undone 'adds'");

                foreach (var pendingChange in undoneChanges)
                {
                    if (File.Exists(pendingChange.LocalFullPath))
                    {
                        await LogAsync($"Deleting file {pendingChange.LocalFullPath}");
                        File.Delete(pendingChange.LocalFullPath);
                    }
                    else
                    {
                        await LogAsync($"File {pendingChange.LocalFullPath} not found, skipping deletion");
                    }
                }
            }
            catch (Exception ex)
            {
                await LogAsync($"Exception occured during execution of {this.GetType().Name}:" +
                    $"{Environment.NewLine}{ex}");
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
        private void InitTfsClient()
        {
            _tfsClient ??= new TfsClient();
        }

        private PendingChangeDto[] ResolveUndoneChanges(PendingChangeDto[] before, PendingChangeDto[] after)
        {
            var undone = new List<PendingChangeDto>();

            foreach (var change in before)
            {
                if (!after.Any(x => x.LocalFullPath == change.LocalFullPath))
                {
                    undone.Add(change);
                }
            }

            return undone.ToArray();
        }

        private PendingChangeDto[] GetPendingChangesOfTypeAdd()
        {
            TfStatusResult result = _tfsClient.ExecuteStatus();
            return result.PendingChanges
                .Where(x => x.ChangeType.HasFlag(ChangeType.Add))
                .ToArray();
        }
    }
}
