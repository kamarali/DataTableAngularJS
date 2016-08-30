using System;
using System.Data;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Impl
{
  public static class DataExtn
  {
    /// <summary>
    /// Returns the value for a field in the given data record.
    /// </summary>
    /// <remarks>
    /// If this method is used within a Materializer expression, it will
    /// be optimized. If the 'name' argument is a literal, we replace it with
    /// the column ordinal. In addition, the optimizations described for
    /// Field(record, ordinal) are applied. If given column not exist in record, then returns default type
    /// Used this method as materializer fails if any column is missing in reader
    /// </remarks>
    /// <typeparam name="T">Expected type of the field.</typeparam>
    /// <param name="record">Record from which to retrieve field.</param>
    /// <param name="name">Name of field.</param>
    /// <returns>Value of field.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
    public static T TryGetField<T>(this IDataRecord record, string name)
    {
      if (null == record)
      {
        throw new ArgumentNullException("record");
      }
      if (null == name)
      {
        throw new ArgumentNullException("name");
      }
      int recordOrdinal;
      try
      {
        recordOrdinal = record.GetOrdinal(name);
      }
      catch(IndexOutOfRangeException)
      {
        return default(T);
      }
      return DataExtensions.Field<T>(record, recordOrdinal);
    }    
   } 

}
