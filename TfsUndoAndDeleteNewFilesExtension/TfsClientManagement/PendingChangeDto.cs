using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement
{
    public class PendingChangeDto
    {
        public string LocalFullPath { get; set; }
        public ChangeType ChangeType { get; set; }
    }
}
