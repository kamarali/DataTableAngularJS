using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// Represents Format of file.
    /// </summary>

    [Serializable]
    public class FileFormat : MasterBase<int>
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The File format Description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the File Version.
        /// </summary>
        /// <value>The File Version.</value>
        public string FileVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [file downloadable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [file downloadable]; otherwise, <c>false</c>.
        /// </value>
        public bool FileDownloadable { get; set; }

        /// <summary>
        /// Gets or sets the search file downloadable.
        /// </summary>
        /// <value>
        /// The search file downloadable.
        /// </value>
        public string SearchFileDownloadable { get; set; }
    }
}
