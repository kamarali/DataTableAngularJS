using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ILanguageManager
    {
        /// <summary>
        /// Adds the language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="folderPath">Path for help folder.</param>
        /// <returns></returns>
        Language AddLanguageCode(Language languageCode, string folderPath);

        /// <summary>
        /// Updates the language code.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns></returns>
        Language UpdateLanguageCode(Language languageCode);

        /// <summary>
        /// Deletes the language code.
        /// </summary>
        /// <param name="Language_Code">The Language Code.</param>
        /// <returns></returns>
        bool DeleteLanguageCode(string Language_Code);

        /// <summary>
        /// Gets the language details.
        /// </summary>
        /// <param name="Language_Code">The Language Code.</param>
        /// <returns></returns>
        Language GetLanguageDetails(string Language_Code);

        /// <summary>
        /// Gets all language list.
        /// </summary>
        /// <returns></returns>
        List<Language> GetAllLanguageList();

        /// <summary>
        /// Gets the language list.
        /// </summary>
        /// <param name="Language_Code">The Language Code.</param>
        /// <param name="Language_Desc">The Language Description.</param>
        /// <returns></returns>
        List<Language> GetLanguageList(string Language_Code, string Language_Desc);

        
    }
}
