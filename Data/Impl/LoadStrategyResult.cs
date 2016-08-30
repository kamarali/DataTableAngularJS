using System;
using System.Collections.Generic;
using Devart.Data.Oracle;

namespace Iata.IS.Data.Impl
{
  public class LoadStrategyResult : IDisposable
  {
    private readonly List<LSResult> _results = new List<LSResult>();
    
    public void Add(string entityName, OracleParameter p)
    {
      _results.Add(new LSResult { EntityName = entityName, Parameter = p });
    }

    /// <summary>
    /// This method  will check whether given entity is loaded from the database or not
    /// </summary>
    public bool IsLoaded(string entityName)
    {
      if (_results != null)
      {
        return _results.Find(i => i.EntityName.CompareTo(entityName) == 0) != null ? true : false;
      }

      return false;
    }

    public OracleDataReader GetReader(string entityName)
    {
      var result = _results.Find(r => r.EntityName.Equals(entityName));
      
      if (result != null)
      {
        var cursor = result.Parameter.Value as OracleCursor;

        if (cursor != null)
        {
          return cursor.GetDataReader();
        }
      }

      return null;
    }

    /// <summary>
    /// Close all the open cursor
    /// </summary>
    public void Dispose()
    {
      //SCP0000: Nullifying any exception caught during Cursor close
      try
      {
        foreach (var lsResult in _results)
        {
            var cursor = lsResult.Parameter.Value as OracleCursor;

            if ((cursor != null) && (!cursor.IsClosed))
            {
                cursor.Close();
            }
        }
      }
      catch(Exception ex)
      {
        // Do Nothing - Suppress any errors  
      }
    }
  }

  public class LSResult
  {
    public string EntityName { get; set; }
    public OracleParameter Parameter { get; set; }
  }
}
