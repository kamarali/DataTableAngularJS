namespace Iata.IS.Business.MemberProfileDataFile
{
  public interface IMemberProfileDataFile
  {
    /// <summary>
    /// Create Member Profile Data Files
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <returns></returns>
    bool CreateMemberProfileDataFile(int billingPeriod, int billingMonth, int billingYear);
  }
}