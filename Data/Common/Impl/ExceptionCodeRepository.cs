using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common.Impl
{
  public class ExceptionCodeRepository : Repository<ExceptionCode>, IExceptionCodeRepository
  {
    /// <summary>
    /// This will return ExceptionCodeId for respective exception code.
    /// </summary>
    /// <param name="exceptionCodeName">exceptionCodeName</param>
    /// <returns>int</returns>
    public int GetExceptionCodeId(string exceptionCodeName)
    {
      var exceptionCode = this.Single(i => i.Name.ToUpper().CompareTo(exceptionCodeName.ToUpper()) == 0);
      if(exceptionCode != null)
      {
        return exceptionCode.Id;
      }
      return 0;
    }

    /// <summary>
    /// This will return ExceptionCode object for respective exception code.
    /// </summary>
    /// <param name="exceptionCodeName">exceptionCodeName</param>
    /// <returns>ExceptionCode</returns>
    public ExceptionCode GetExceptionCode(string exceptionCodeName)
    {
      return this.Single(i => i.Name.ToUpper().CompareTo(exceptionCodeName.ToUpper()) == 0);
    }
  }
}
