using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing
{
    /// <summary>
    /// Parses text line to a property
    /// </summary>
    public interface IPropertyParser<T>
    {
        public bool TryParse(string line, out T value);
    }
}
