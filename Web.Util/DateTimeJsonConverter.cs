using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;

namespace Iata.IS.Web.Util
{
  public class DateTimeJsonConverter : JavaScriptConverter
  {
    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
    {
      string formattedDate = string.Empty;
      if (obj != null)
      {
        DateTime dt = Convert.ToDateTime(obj);
        formattedDate = dt.ToString("dd-MMM-yy");
      }

      return new Dictionary<string, object> { { "", formattedDate } };
    }

    public override IEnumerable<Type> SupportedTypes
    {
      //Define the ListItemCollection as a supported type.
      get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(DateTime) })); }
    }
  }
}
