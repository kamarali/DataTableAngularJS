using System;
using System.Runtime.Serialization;

namespace Iata.IS.Core.Exceptions
{
  /// <summary>
  /// IS Data Exception class. This exception is thrown in following cases,
  /// 1. Calendar data not exists for given date or period.
  /// 2. For given date or period, no period open.
  /// </summary>
  [Serializable]
  public class ISCalendarDataNotFoundException : ISException
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ISCalendarDataNotFoundException"/> class.
    /// </summary>
    /// <param name="errorCode">Error Code for the exception created.</param>
    public ISCalendarDataNotFoundException(string errorCode)
      : base(errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ISCalendarDataNotFoundException"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message.</param>
    public ISCalendarDataNotFoundException(string errorCode, string message)
      : base(errorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ISCalendarDataNotFoundException"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="message">The message.</param>
    /// <param name="inner">The inner exception object.</param>
    public ISCalendarDataNotFoundException(string errorCode, string message, Exception inner)
      : base(errorCode, message, inner)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ISCalendarDataNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    /// <remarks>
    /// This constructor is needed for serialization.
    /// </remarks>
    protected ISCalendarDataNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // Set the generic exception code.
    }
  }
}
