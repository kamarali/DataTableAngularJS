using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using System.IO;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;


namespace Iata.IS.Business.Common.Impl
{
    public class LanguageManager : ILanguageManager
    {
        /// <summary>
        /// Gets or sets the language repository.
        /// </summary>
        /// <value>
        /// The language repository.
        /// </value>
        public ILanguageRepository LanguageRepository { get; set; }

        /// <summary>
        /// Adds the language code.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <returns></returns>
        public Language AddLanguageCode(Language language,string folderPath)
        {
            var languageData = LanguageRepository.Single(type => type.Language_Code.ToUpper() == language.Language_Code.ToUpper());
            //If Language Code already exists, throw exception
            if (languageData != null)
            {
                throw new ISBusinessException(ErrorCodes.DuplicateLanguage, languageData.Language_Code);
            }

            if (!IsLanguageDirectoryExist(language, folderPath))
            {
                throw new ISBusinessException(ErrorCodes.LanguageFolderNotFound, language.Language_Code);
            }
            //Call repository method for adding language
            LanguageRepository.Add(language);
            UnitOfWork.CommitDefault();
            return language;
   
        }

        private bool IsLanguageDirectoryExist(Language language, string folderPath)
        {
            if (language.IsReqForHelp)
            {
                var fullPath = Path.Combine(folderPath, language.Language_Code);
                var directoryInfo = new DirectoryInfo(fullPath);

                return directoryInfo.Exists;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates the reason code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        public Language UpdateLanguageCode(Language languageCode)
        {
            var languageData = LanguageRepository.Single(type => type.Language_Code != languageCode.Language_Code && type.Language_Desc == languageCode.Language_Desc);
            //If Language Code already exists, throw exception
            if (languageData != null)
            {
                throw new ISBusinessException(ErrorCodes.DuplicateLanguage);
            }
            languageData = LanguageRepository.Single(type => type.Language_Code == languageCode.Language_Code);
            var updatedlanguage = LanguageRepository.Update(languageCode);
            UnitOfWork.CommitDefault();
            return updatedlanguage;  
        }

        /// <summary>
        /// Deletes the language code.
        /// </summary>
        /// <param name="Language_Code">The language code.</param>
        /// <returns></returns>
        public bool DeleteLanguageCode(string Language_Code)
        {
            bool delete = false;
            var languageData = LanguageRepository.Single(type => type.Language_Code == Language_Code);
            if (languageData != null)
            {
                languageData.IsActive = !(languageData.IsActive);
                var updatedlanguage = LanguageRepository.Update(languageData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the Language Code details.
        /// </summary>
        /// <param name="Language_Code">The Language Code.</param>
        /// <returns></returns>
        public Language GetLanguageDetails(string Language_Code)
        {
            var language = LanguageRepository.Single(type => type.Language_Code == Language_Code);
            return language;
        }

        /// <summary>
        /// Gets all language code list.
        /// </summary>
        /// <returns></returns>
        public List<Language> GetAllLanguageList()
        {
            var languageList = LanguageRepository.GetAllLanguageCodes();
            return languageList.ToList();
        }

        /// <summary>
        /// Gets the language code list.
        /// </summary>
        /// <param name="Language_Code">The Language code.</param>
        /// <param name="Language_Desc">The Language Description.</param>
        /// <returns></returns>
        public List<Language> GetLanguageList(string Language_Code, string Language_Desc)
        {
            var languageList = new List<Language>();
            languageList = LanguageRepository.GetAllLanguageCodes().ToList();

            if (!string.IsNullOrEmpty(Language_Code))
            {
                languageList = languageList.Where(cl => cl.Language_Code.Contains(Language_Code.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Language_Desc))
            {
                languageList = languageList.Where(cl => cl.Language_Desc == Language_Desc).ToList();
            }
            return languageList.ToList();
        }
    }
}
