using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Iata.IS.Web.Util
{
    public class SanitizeResult : ContentResult
    {

        public SanitizeResult(string value)
        {
            value = new HttpEncoderEx().GetSafeHtmlFragment(value);
            Content = TrimStringContent(value);

        }

        public SanitizeResult(string value, string contentType)
        {
            ContentType = contentType;

            Content = !string.IsNullOrWhiteSpace(contentType) && contentType.ToLower() == "text/xml" ? value : new HttpEncoderEx().GetSafeHtmlFragment(value);
        }
        
        /// <summary>
        /// ID 2015-SIS-005
        /// Cross-Site Scripting (XSS)
        /// </summary>
        /// <param name="sanitizedString"></param>
        /// <returns></returns>
        private static string TrimStringContent(string sanitizedString)
        {
            while (sanitizedString.IndexOf(" |") != -1)
            {
                sanitizedString = sanitizedString.Replace(" |", "|");
            }
            return sanitizedString;
        }
    }
}
