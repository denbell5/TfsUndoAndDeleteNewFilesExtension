using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing
{
    /// <summary>
    /// Parses 'Local item' line from 'tf status /format:Detailed' output
    /// </summary>
    public class LocalFullPathParser : IPropertyParser<string>
    {
        private readonly Regex _regex = new Regex(" *Local item *: \\[.*] (.*)", RegexOptions.Compiled);

        /// <summary>
        /// Parses 'Local item' line from 'tf stat /format:Detailed' output
        /// </summary>
        public bool TryParse(string line, out string value)
        {
            Match match = _regex.Match(line);
            
            if (!match.Success)
            {
                value = default;
                return false;
            }

            value = match.Groups[1].Value;
            return true;
        }
    }
}
