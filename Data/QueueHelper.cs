using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Devart.Data.Oracle;
using Iata.IS.Core.DI;
using log4net;

namespace Iata.IS.Data
{
  /// <summary>
  /// This class provides the functionality required to perform operations on the Oracle AQ.
  /// </summary>
  public class QueueHelper : IDisposable
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Connection string.
    /// </summary>
    string _connectionString;

    /// <summary>
    /// Name of the connection string on which operations are to be performed.
    /// </summary>
    readonly string _queueName;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueHelper"/> class.
    /// </summary>
    /// <param name="queueName">Name of the Oracle Queue.</param>
    public QueueHelper(string queueName)
    {
      _queueName = queueName;
    }

      /// <summary>
      /// En-queues the message with the specified key value pairs as message data in queue.
      /// </summary>
      /// <param name="keyValuePairs">The attribute name and values to be included in queue message. Attribute names should be exactly same as oracleMessageType attributes.</param>
      /// <param name="priority">The priority value. Lower the value, higher is the priority. Message with lowest priority value will get de-queued first while processing.</param>
      /// <param name="delay">The delay value. Number of seconds after which message is ready for dequeuing.</param>
      /// <param name="fileWatcherLogCommand">File Watcher Logging Stastement to be Executed in DB. Added For SCP# 403403 - iiNET Production</param>
      public void Enqueue(IDictionary<string, string> keyValuePairs, int priority = 1, int delay = 0, string fileWatcherLogCommand = null)
    {
      // Get oracle connection.      
      _connectionString = Core.Configuration.ConnectionString.Instance.ServiceConnectionString;

      // Create connection to Oracle DB.
      using (var oracleConnection = new OracleConnection(_connectionString))
      {
        try
        {
          oracleConnection.Open();

          #region SCP# 403403 - iiNET Production
          try
          {
              if (!string.IsNullOrWhiteSpace(fileWatcherLogCommand))
              {
                  // Call Sp to log it in DB 
                  OracleCommand logCommand = new OracleCommand(fileWatcherLogCommand, oracleConnection);
                  logCommand.Parameters.Add("pFILE_NAME", OracleDbType.VarChar, ParameterDirection.Input).Value
                      = keyValuePairs["FILE_NAME"];
                  logCommand.ExecuteNonQuery();
              }
          }
          catch (Exception exception)
          {
              Logger.Error(exception.Message, exception);
              //Eat it, normal functionality of enqueuing in target queue should continue as is.
              //throw;
          } 
          #endregion

          // Create Oracle queue object using queue name. 
          using (var queue = new OracleQueue(_queueName, oracleConnection))
          {
            // Create Oracle object using keyValuePairs for en-queue.
            var oracleObject = new OracleObject(queue.ReadPayloadTypeName(), oracleConnection);
            try
            {
              foreach (var keyValue in keyValuePairs)
              {
                oracleObject[keyValue.Key] = keyValue.Value;
              }
            }
            catch (InvalidOperationException ex)
            {
              // This exception occurs when Attribute name of Oracle Type is invalid.
              Logger.Error(ex.Message, ex);
              throw;
            }

            // Create OracleQueueMessage to be en-queued.
            var message = new OracleQueueMessage(oracleObject) { MessageProperties = { Priority = priority, DeliveryMode = OracleQueueDeliveryMode.PersistentOrBuffered, Delay = delay} };

            // En-queue message to the queue.
            queue.Enqueue(message);
          }
        }
        catch (OracleException ex)
        {
          // Oracle Connection exceptions
          // 1017 - ORA-01017: invalid username/password; logon denied
          // 12532 - ORA-12532: TNS:invalid argument -- when network is not available
          // 12170 - ORA-12170: TNS:Connect timeout occurred

          // Oracle Queue related exceptions
          // 24010 - ORA-24010: QUEUE SCOTT.BULKLOADDER_QUEUE does not exist
          // 22303 - OCI-22303: type ""."SISMESSAGE1" not found -- Invalid oracle type

          Logger.Error(ex.Message, ex);
          throw;
        }
        finally
        {
          oracleConnection.Close();
        }
      }
    }

    public void Dispose()
    {
      // Nothing to dispose.
    }
  }
}
