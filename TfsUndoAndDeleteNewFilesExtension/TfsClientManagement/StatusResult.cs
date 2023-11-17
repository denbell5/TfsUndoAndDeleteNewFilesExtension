using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement
{
    public class TfStatusResult
    {
        public List<PendingChangeDto> PendingChanges { get; set; }
    }
}
