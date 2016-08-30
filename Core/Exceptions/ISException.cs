using System;
using System.Runtime.Serialization;

namespace Iata.IS.Core.Exceptions
{
  /// <summary>
  /// Base Exception class for all IS exceptions.
  /// </summary>
  /// <remarks>
  /// Each exception will have <ref name="ErrorCode"/> associated with it.
  /// </remarks>
  [Serializable]
  public abstract class ISException : ApplicationException
  {
    #region Public Properties

    /// <summary>
    /// Gets or sets the error code for the exception.
    /// </summary>
    public string ErrorCode { get; set; }

    /// <summary>
    /// CMP622: Property to carry retry count.
    /// Gets or sets the error code for the exception.
    /// </summary>
    public int RetryCount { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the Instance of ISException class.
    /// </summary>
    /// <param name="errorCode">Error Code for the exception created.</param>
    protected ISException(string errorCode)
    {
      ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates the Instance of ISException class.
    /// </summary>
    /// <param name="errorCode">Error Code for the exception created.</param>
    /// <param name="message">The exception message.</param>
    protected ISException(string errorCode, string message)
      :base(message)
    {
      ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates the Instance of ISException class.
    /// </summary>
    /// <param name="errorCode">Error Code for the exception created.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="inner">Inner exception.</param>
    protected ISException(string errorCode, string message, Exception inner)
      :base(message, inner)
    {
      ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ISException"/> class.
    /// </summary>
    /// <param name="info">The object that holds the serialized object data.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    /// <remarks>
    /// This constructor is needed for serialization.
    /// </remarks>
    protected ISException(SerializationInfo info, StreamingContext context)
      :base(info, context)
    {
      // Set the generic exception code.
    }


    /// <summary>
    /// CMp622: overload to add retry count
    /// Creates the Instance of ISException class.
    /// </summary>
    /// <param name="errorCode">Error Code for the exception created.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="retryCount">set retry count</param>
    protected ISException(string errorCode, string message, int retryCount)
      : base(message)
    {
      ErrorCode = errorCode;
      RetryCount = retryCount;
    }
    #endregion

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("ErrorCode", ErrorCode);
    }
  }
}
