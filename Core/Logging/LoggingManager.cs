namespace Iata.IS.Core.Logging
{
  /// <summary>
  /// The LoggingManager has helper methods to use the log4net framework.
  /// </summary>
  public static class LoggingManager
  {
    private const string HostName = "Host";
    private const string InstanceId = "InstanceId";
    
    /// <summary>
    /// Initializes the global context that will be used in all the log messages. The global context includes:
    ///   1. The host name.
    /// </summary>
    public static void InitializeGlobalContext()
    {
      log4net.GlobalContext.Properties[HostName] = System.Net.Dns.GetHostName();
    }

    public static void InitializeGlobalContextForProcessingUnit(string instanceId)
    {
        log4net.GlobalContext.Properties[InstanceId] = instanceId;
    }
  }
}