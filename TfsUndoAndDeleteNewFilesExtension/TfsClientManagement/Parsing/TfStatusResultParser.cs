using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement
{
    /// <summary>
    /// Parses output of 'tf status /format:Detailed' command
    /// </summary>
    public class TfStatusResultParser
    {
        /// <summary>
        /// Parses output of 'tf status /format:Detailed' command
        /// </summary>
        public TfStatusResult Parse(string cmdOutput)
        {
            string[] lines = cmdOutput.Split(
                new string[] { Environment.NewLine }, StringSplitOptions.None);

            var pendingChangeList = new List<PendingChangeDto>();

            var changeTypeParser = new ChangeTypeParser();
            var localFullPathParser = new LocalFullPathParser();

            PendingChangeDto currentPendingChange = null;
            
            foreach (string line in lines)
            {
                if (changeTypeParser.TryParse(line, out ChangeType changeType))
                {
                    // ChangeType comes very first of all properties that are parsed
                    // so it also initializes each pending change
                    currentPendingChange = new PendingChangeDto
                    {
                        ChangeType = changeType
                    };
                    pendingChangeList.Add(currentPendingChange);
                }
                else if (localFullPathParser.TryParse(line, out string localFullPath))
                {
                    currentPendingChange.LocalFullPath = localFullPath;
                }
            }

            return new TfStatusResult
            {
                PendingChanges = pendingChangeList,
            };
        }
    }
}
