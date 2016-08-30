using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
  public interface IExceptionCodeRepository : IRepository<ExceptionCode>
  {

    /// <summary>
    /// This will return ExceptionCodeId for respective exception code.
    /// </summary>
    /// <param name="exceptionCodeName">exceptionCodeName</param>
    /// <returns>int</returns>
    int GetExceptionCodeId(string exceptionCodeName);

    /// <summary>
    /// This will return ExceptionCode object for respective exception code.
    /// </summary>
    /// <param name="exceptionCodeName">exceptionCodeName</param>
    /// <returns>ExceptionCode</returns>
    ExceptionCode GetExceptionCode(string exceptionCodeName);
  }
}
