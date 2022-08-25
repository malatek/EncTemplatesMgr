using System.Collections.Generic;

namespace EncTemplatesMgr.Interfaces
{
    public interface ICustomTemplate
    {
        /// <summary>
        /// Template Name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Template Description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// File path where template would reside in Encompass.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Field IDs and their associated values.
        /// </summary>
        Dictionary<string, string> FieldIDsAndValues { get; set; }
    }
}
