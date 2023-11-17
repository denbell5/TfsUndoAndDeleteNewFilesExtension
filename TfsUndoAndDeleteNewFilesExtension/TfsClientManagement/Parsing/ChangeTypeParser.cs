using Microsoft.TeamFoundation.VersionControl.Client;
using System.Linq;
using System.Text.RegularExpressions;

namespace TfsUndoAndDeleteNewFilesExtension.TfsClientManagement.Parsing
{
    /// <summary>
    /// Parses 'Change' line from 'tf status /format:Detailed' output
    /// </summary>
    public class ChangeTypeParser : IPropertyParser<ChangeType>
    {
        private readonly Regex _regex = new Regex(" *Change *: (.*)", RegexOptions.Compiled);

        /// <summary>
        /// Parses 'Change' line from 'tf status /format:Detailed' output
        /// </summary>
        public bool TryParse(string line, out ChangeType value)
        {
            Match match = _regex.Match(line);

            if (!match.Success)
            {
                value = default;
                return false;
            }

            // "edit, add, rollback"
            string csvChangeTypes = match.Groups[1].Value;

            // ["Edit", "Add", "Rollback"]
            string[] stringChangeTypes = csvChangeTypes
                .Split(new string[] { ", " }, StringSplitOptions.None)
                .Select(x => x[0].ToString().ToUpper() + x.Substring(1)) // capitalize
                .ToArray();

            Type enumType = typeof(ChangeType);
            ChangeType changeType = 0;
            foreach (string stringChangeType in stringChangeTypes)
            {
                ChangeType _changeType = (ChangeType)Enum.Parse(enumType, stringChangeType);
                changeType |= _changeType;
            }

            value = changeType;

            return true;
        }
    }
}
