using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement
{
    public interface ITfsClient
    {
        /// <summary>
        /// Executes 'tf status /format:Detailed'
        /// </summary>
        TfStatusResult ExecuteStatus();
    }
}
