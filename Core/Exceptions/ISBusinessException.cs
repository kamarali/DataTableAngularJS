using System;
using System.Runtime.Serialization;

namespace Iata.IS.Core.Exceptions
{
  /// <summary>
  ///   IS Business Exception class. This exception will be thrown when exceptional 
  ///   condition is encountered at business layer.
  /// </summary>
  [Serializable]
  public class ISBusinessException : ISException
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ISBusinessException" /> class.
    /// </summary>
    /// <param name = "errorCode">Error Code for the exception created.</param>
    public ISBusinessException(string errorCode)
      : base(errorCode)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ISBusinessException" /> class.
    /// </summary>
    /// <param name = "errorCode">The error code.</param>
    /// <param name = "message">The message.</param>
    public ISBusinessException(string errorCode, string message)
      : base(errorCode, message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ISBusinessException" /> class.
    /// </summary>
    /// <param name = "errorCode">The error code.</param>
    /// <param name = "message">The message.</param>
    /// <param name = "inner">The inner.</param>
    public ISBusinessException(string errorCode, string message, Exception inner)
      : base(errorCode, message, inner)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ISBusinessException" /> class.
    /// </summary>
    /// <param name = "info">The object that holds the serialized object data.</param>
    /// <param name = "context">The contextual information about the source or destination.</param>
    /// <remarks>
    ///   This constructor is needed for serialization.
    /// </remarks>
    protected ISBusinessException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // Set the generic exception code.
    }
  }
}