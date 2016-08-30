using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Web.Util
{
   public static class ListEx
    {
       public static HttpEncoderEx htmlEncoder;
       static ListEx()
       {
           htmlEncoder = new HttpEncoderEx();
       }

       public static IList<T> ToSanitizeStringField<T>(this IList<T> objGridRecords)
        {
            foreach (var item in objGridRecords)
            {
                // Get an array of FieldInfo objects.
                var properties = item.GetType().GetProperties();
                foreach (var property in properties.Where(property => property.PropertyType == typeof(string)))
                {
                    if (property.CanWrite)
                    property.SetValue(item, htmlEncoder.GetSafeHtmlFragment(Convert.ToString(property.GetValue(item, null))), null);
                }
            }
            return objGridRecords;
        }

        public static IQueryable<T> ToSanitizeStringField<T>(this IQueryable<T> objGridRecords)
        {
            foreach (var objGridRecord in objGridRecords)
            {
                // Get an array of FieldInfo objects.
                var properties = objGridRecord.GetType().GetProperties();
                foreach (var property in properties.Where(property => property.PropertyType == typeof(string)))
                {
                    if (property.CanWrite)
                    property.SetValue(objGridRecord, htmlEncoder.GetSafeHtmlFragment(Convert.ToString(property.GetValue(objGridRecord, null))), null);
                }
            }
            return objGridRecords;
        }

        public static IEnumerable<T> ToSanitizeStringField<T>(this IEnumerable<T> objGridRecords)
        {
            foreach (var item in objGridRecords)
            {
                var properties = item.GetType().GetProperties();
                foreach (var property in properties.Where(property => property.PropertyType == typeof(string)))
                {
                    if (property.CanWrite)
                    property.SetValue(item, htmlEncoder.GetSafeHtmlFragment(Convert.ToString(property.GetValue(item, null))), null);
                }
            }
            return objGridRecords;
        }

    }
}
