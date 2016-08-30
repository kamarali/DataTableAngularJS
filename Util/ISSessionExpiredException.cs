using System;
using System.Runtime.Serialization;
using Iata.IS.Core.Exceptions;

namespace Iata.IS.Web.Util
{
  /// <summary>
  /// IS Session Expired Exception class. This exception will be thrown when the IS-WEB session expires.
  /// </summary>
  [Serializable]
  public class ISSessionExpiredException : ISException
  {
    private const string SessionExpiredCode = "SessionExpired";

    /// <summary>
    /// Initializes a new instance of the <see cref = "ISSessionExpiredException" /> class.
    /// </summary>
    public ISSessionExpiredException() : base(SessionExpiredCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "ISSessionExpiredException" /> class.
    /// </summary>
    /// <param name = "message">The message.</param>
    public ISSessionExpiredException(string message) : base(SessionExpiredCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "ISSessionExpiredException" /> class.
    /// </summary>
    /// <param name = "message">The message.</param>
    /// <param name = "inner">The inner.</param>
    public ISSessionExpiredException(string message, Exception inner) : base(SessionExpiredCode, message, inner)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref = "ISSessionExpiredException" /> class.
    /// </summary>
    /// <param name = "info">The object that holds the serialized object data.</param>
    /// <param name = "context">The contextual information about the source or destination.</param>
    /// <remarks>
    /// This constructor is needed for serialization.
    /// </remarks>
    protected ISSessionExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      // Set the generic exception code.
    }
  }
}