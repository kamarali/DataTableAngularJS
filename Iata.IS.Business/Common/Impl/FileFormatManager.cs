using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class FileFormatManager : IFileFormatManager
    {
        /// <summary>
        /// Gets or sets the file format repository.
        /// </summary>
        /// <value>
        /// The file format repository.
        /// </value>
        public IRepository<FileFormat> FileFormatRepository { get; set; }

        /// <summary>
        /// Adds the file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <returns></returns>
        public FileFormat AddFileFormat(FileFormat fileFormat)
        {
            var fileFormatData = FileFormatRepository.Single(type => type.Id == fileFormat.Id);
            ////If FileFormat Code already exists, throw exception
            //if (fileFormatData != null)
            //{
            //    throw new ISBusinessException(ErrorCodes.InvalidCountryCode);
            //}
            //Call repository method for adding fileFormat
            if (fileFormatData == null)
            {
                FileFormatRepository.Add(fileFormat);

                UnitOfWork.CommitDefault();
            }
            return fileFormat;
        }

        /// <summary>
        /// Updates the file format.
        /// </summary>
        /// <param name="fileFormat">The file format.</param>
        /// <returns></returns>
        public FileFormat UpdateFileFormat(FileFormat fileFormat)
        {
            //var fileFormatData = FileFormatRepository.Single(type => type.Id == fileFormat.Id);
            var updatedfileFormat = FileFormatRepository.Single(type => type.Id == fileFormat.Id);
            if (updatedfileFormat != null)
            {
                updatedfileFormat = FileFormatRepository.Update(fileFormat);
                UnitOfWork.CommitDefault();
            }
            return updatedfileFormat;
        }

        /// <summary>
        /// Deletes the file format.
        /// </summary>
        /// <param name="fileFormatId">The file format id.</param>
        /// <returns></returns>
        public bool DeleteFileFormat(int fileFormatId)
        {
            bool delete = false;
            var fileFormatData = FileFormatRepository.Single(type => type.Id == fileFormatId);
            if (fileFormatData != null)
            {
                fileFormatData.IsActive = !(fileFormatData.IsActive);
                var updatedcountry = FileFormatRepository.Update(fileFormatData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the file format details.
        /// </summary>
        /// <param name="fileFormatId">The file format id.</param>
        /// <returns></returns>
        public FileFormat GetFileFormatDetails(int fileFormatId)
        {
            var fileFormat = FileFormatRepository.Single(type => type.Id == fileFormatId);
            return fileFormat;
        }

        /// <summary>
        /// Gets all file format list.
        /// </summary>
        /// <returns></returns>
        public List<FileFormat> GetAllFileFormatList()
        {
            var fileFormatList = FileFormatRepository.GetAll();
            return fileFormatList.ToList();
        }

        /// <summary>
        /// Gets the file format list.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="IsFileDownloadable"></param>
        /// <returns></returns>
        public List<FileFormat> GetFileFormatList(string description, string IsFileDownloadable)
        {
            var fileFormatList = new List<FileFormat>();
            fileFormatList = FileFormatRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(description))
            {
                fileFormatList = fileFormatList.Where(cl => cl.Description != null && cl.Description.Trim().ToLower().Contains(description.Trim().ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(IsFileDownloadable))
            {
                fileFormatList = fileFormatList.Where(cl => cl.FileDownloadable == Convert.ToBoolean(IsFileDownloadable)).ToList();
            }
            return fileFormatList.ToList();
        }
    }
}
