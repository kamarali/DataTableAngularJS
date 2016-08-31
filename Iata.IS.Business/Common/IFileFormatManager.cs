using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IFileFormatManager
    {
        /// <summary>
        /// Adds the file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <returns></returns>
        FileFormat AddFileFormat(FileFormat fileFormat);

        /// <summary>
        /// Updates the file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <returns></returns>
        FileFormat UpdateFileFormat(FileFormat fileFormat);

        /// <summary>
        /// Deletes the file format.
        /// </summary>
        /// <param name="fileFormatId">The file format id.</param>
        /// <returns></returns>
        bool DeleteFileFormat(int fileFormatId);

        /// <summary>
        /// Gets the file format details.
        /// </summary>
        /// <param name="fileFormatId">The file format id.</param>
        /// <returns></returns>
        FileFormat GetFileFormatDetails(int fileFormatId);

        /// <summary>
        /// Gets all file format list.
        /// </summary>
        /// <returns></returns>
        List<FileFormat> GetAllFileFormatList();


        /// <summary>
        /// Gets the file format list.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="IsFileDownloadable">The is file downloadable.</param>
        /// <returns></returns>
        List<FileFormat> GetFileFormatList(string description, string IsFileDownloadable);
    }
}
