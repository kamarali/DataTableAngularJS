using System;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class ExpiredCorrespondence
  {
    public long? CorrespondenceNumber { get; set; }

    public DateTime CorrespondenceDate { get; set; }

    public int CorrespondenceStage { get; set; }

    public int FromMemberId { get; set; }

    public int ToMemberId { get; set; }

    public CorrespondenceStatus CorrespondenceStatus
    {
      get
      {
        return (CorrespondenceStatus)CorrespondenceStatusId;
      }
      set
      {
        CorrespondenceStatusId = Convert.ToInt32(value);
      }
    }

    public int CorrespondenceStatusId { get; set; }

    //public int CorrespondenceSubStatus { get; set; }
    public CorrespondenceSubStatus CorrespondenceSubStatus
    {
      get
      {
        return (CorrespondenceSubStatus)CorrespondenceSubStatusId;
      }
      set
      {
        CorrespondenceSubStatusId = Convert.ToInt32(value);
      }
    }

    public int CorrespondenceSubStatusId { get; set; }

    public bool AuthorityToBill { get; set; }

    public int AlertScenario { get; set; }
    public DateTime? CorExpiryDate { get; set; }
    public DateTime? BmExpiryDate { get; set; }

    #region CMP #657: Retention of Additional Email Addresses in Correspondences.
    // FRS Section: 2.4 Email Alerts on Expiry of Correspondences.
    /// <summary>
    /// Perpery for Additional E-Mail ID(s) pertaining to Initiator.
    /// </summary>
    public string AdditionalEmailInitiator { get; set; }

    /// <summary>
    /// Property for Additional E-Mail ID(s) pertaining to Non-Initiator.
    /// </summary>
    public string AdditionalEmailNonInitiator { get; set; }
    #endregion
  }
}
